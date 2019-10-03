﻿namespace BeepLive.World
{
    using BeepLive.Config;
    using BeepLive.Entities;
    using SFML.System;
    using SimplexNoise;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

    public class Map
    {
        public Chunk[,] Chunks;

        public MapConfig Config;
        public List<Entity> Entities;
        public List<Player> Players;
        public Random Random;

        public Map()
        {
            Entities = new List<Entity>();
            Players = new List<Player>();

            Random = new Random();

            _stepTimes = new ConcurrentQueue<DateTime>();

            _stepTimeClearer = new Timer(map =>
            {
                ConcurrentQueue<DateTime> stepTimes = ((Map)map)._stepTimes;
                while (stepTimes.TryPeek(out var time))
                {
                    if (DateTime.UtcNow.Subtract(time).TotalSeconds > 1)
                    {
                        _ = stepTimes.TryDequeue(out _);
                    }
                }
            }, this, 1000, 1000);
        }

        public Map(MapConfig config) : this()
        {
            Config = config;

            GenerateMap();
        }

        public bool Simulating;
        public delegate void OnSimulationStopEventHandler();
        public event OnSimulationStopEventHandler OnSimulationStop;
        public int StepsQueued, StepsFinished;

        private readonly ConcurrentQueue<DateTime> _stepTimes;
        private readonly Timer _stepTimeClearer;

        public float ActualFrameRate => _stepTimes.Count;

        public int FramesSinceMovementTresholdMet;

        public void Step()
        {
            if (!Simulating) return;

            StepsQueued++;

            lock (Entities)
            {
                _stepTimes.Enqueue(DateTime.UtcNow);
                // Make array to avoid concurrent modification exception; Make temporary clone to be able to modify the original
                Entity[] entities = Entities.ToArray();

                entities.ForEach(e => e.Step());

                float maxVelocity = entities.Length > 0
                    ? entities.Max(e => (e.Velocity.X * e.Velocity.X) + (e.Velocity.Y * e.Velocity.Y))
                    : 0;

                StepsFinished++;

                if (!Entities.TrueForAll(x => x is Player)) return;
                if (maxVelocity < Config.PhysicalEnvironment.MovementThreshold) FramesSinceMovementTresholdMet++;
                else FramesSinceMovementTresholdMet = 0;
                if (FramesSinceMovementTresholdMet < Config.PhysicalEnvironment.MovementThresholdMinDuration) return;

                Simulating = false;
                OnSimulationStop?.Invoke();
            }
        }

        public static Vector2i GetChunkIndex(Vector2f position)
        {
            return new Vector2i((int)MathF.Floor(position.X), (int)MathF.Floor(position.Y));
        }

        public Chunk GetChunk(Vector2f position, out Vector2f chunkPosition)
        {
            int i = (int)MathF.Floor(position.X / Config.ChunkSize);
            int j = (int)MathF.Floor(position.Y / Config.ChunkSize);
            chunkPosition = new Vector2f(i * Config.ChunkSize, j * Config.ChunkSize);
            return i < 0 || j < 0 || i >= Config.MapWidth || j >= Config.MapHeight ? null : Chunks[i, j];
        }

        public Voxel GetVoxel(Vector2f position)
        {
            return GetChunk(position, out Vector2f chunkPosition)?.GetVoxel(position - chunkPosition) ??
                   new Voxel(this);
        }

        public Map LoadConfig(string path)
        {
            Config = XmlHelper.LoadFromXmlString<MapConfig>(File.ReadAllText(path));

            return this;
        }

        public Map GenerateMap()
        {
            Config.PhysicalEnvironment.VoxelTypes.Add(Config.GroundVoxelType);

            Chunks = new Chunk[Config.MapWidth, Config.MapHeight];

            for (uint chunkI = 0; chunkI < Config.MapWidth; chunkI++)
            {
                for (uint chunkJ = 0; chunkJ < Config.MapHeight; chunkJ++)
                {
                    Chunk chunk = Chunks[chunkI, chunkJ] =
                        new Chunk(this, new Vector2f(chunkI * Config.ChunkSize, chunkJ * Config.ChunkSize));

                    for (uint voxelI = 0; voxelI < Config.ChunkSize; voxelI++)
                    {
                        float height = Noise.CalcPixel1D(
                                           (int)((chunkI * Config.ChunkSize) + voxelI),
                                           Config.HorizontalNoiseScale) * (Config.VerticalNoiseScale / 128f);

                        for (uint voxelJ = 0; voxelJ < Config.ChunkSize; voxelJ++)
                        {
                            bool isGround = (chunkJ * Config.ChunkSize) + voxelJ > height + Config.GroundLevel;

                            bool isFloating = Noise.CalcPixel2D(
                                                  (int)((chunkI * Config.ChunkSize) + voxelI),
                                                  (int)((chunkJ * Config.ChunkSize) + voxelJ),
                                                  Config.FloatingNoiseScale) / 128f
                                              < Config.FloatingNoiseThreshold;

                            chunk[voxelI, voxelJ] =
                                isGround || isFloating
                                    ? new Voxel(this, Config.GroundVoxelType)
                                    : new Voxel(this);
                        }
                    }
                }
            }

            return this;
        }
    }
}