using System;
using System.Collections.Generic;
using System.IO;
using BeepLive.Config;
using BeepLive.Entities;
using SFML.System;
using SimplexNoise;

namespace BeepLive.World
{
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
        }

        public Map(MapConfig config) : this()
        {
            Config = config;

            GenerateMap();
        }

        public void Step()
        {
            lock (Entities)
            {
                // Make array to avoid concurrent modification exception; Make temporary clone to be able to modify the original
                Entities.ToArray().ForEach(e => e.Step());
            }
        }

        public Vector2i GetChunkIndex(Vector2f position)
        {
            return new Vector2i((int) MathF.Floor(position.X), (int) MathF.Floor(position.Y));
        }

        public Chunk GetChunk(Vector2f position, out Vector2f chunkPosition)
        {
            int i = (int) MathF.Floor(position.X / Config.ChunkSize);
            int j = (int) MathF.Floor(position.Y / Config.ChunkSize);
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
            for (uint chunkJ = 0; chunkJ < Config.MapHeight; chunkJ++)
            {
                Chunk chunk = Chunks[chunkI, chunkJ] =
                    new Chunk(this, new Vector2f(chunkI * Config.ChunkSize, chunkJ * Config.ChunkSize));

                for (uint voxelI = 0; voxelI < Config.ChunkSize; voxelI++)
                {
                    float height = Noise.CalcPixel1D(
                                       (int) (chunkI * Config.ChunkSize + voxelI),
                                       Config.HorizontalNoiseScale) * (Config.VerticalNoiseScale / 128f);

                    for (uint voxelJ = 0; voxelJ < Config.ChunkSize; voxelJ++)
                    {
                        bool isGround = chunkJ * Config.ChunkSize + voxelJ > height + Config.GroundLevel;

                        bool isFloating = Noise.CalcPixel2D(
                                              (int) (chunkI * Config.ChunkSize + voxelI),
                                              (int) (chunkJ * Config.ChunkSize + voxelJ),
                                              Config.FloatingNoiseScale) / 128f
                                          < Config.FloatingNoiseThreshold;

                        chunk[voxelI, voxelJ] =
                            isGround || isFloating
                                ? new Voxel(this, Config.GroundVoxelType)
                                : new Voxel(this);
                    }
                }
            }

            return this;
        }
    }
}