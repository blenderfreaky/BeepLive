using BeepLive.Entities;
using SFML.System;
using System;
using System.Collections.Generic;
using SFML.Graphics;
using SimplexNoise;

namespace BeepLive.World
{
    public class Map
    {
        public PhysicalEnvironment PhysicalEnvironment;
        public List<Entity> Entities;
        public uint ChunkSize;
        public int MapWidth, MapHeight;
        public Chunk[,] Chunks;
        public Color BackgroundColor;

        public Map()
        {
            PhysicalEnvironment = new PhysicalEnvironment
            {
                VoxelTypes = new List<VoxelType>(),
            };

            Entities = new List<Entity>();
        }

        #region Fluent API
        private Map SetSize(int mapWidth, int mapHeight)
        {
            if (mapWidth <= 0) throw new ArgumentOutOfRangeException(nameof(mapWidth));
            MapWidth = mapWidth;
            if (mapHeight <= 0) throw new ArgumentOutOfRangeException(nameof(mapHeight));
            MapHeight = mapHeight;
            Chunks = new Chunk[mapWidth, mapHeight];

            return this;
        }

        private Map SetChunkSize(uint chunkSize)
        {
            if (chunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSize));
            ChunkSize = chunkSize;

            return this;
        }

        public Map SetAirResistance(float airResistance)
        {
            PhysicalEnvironment.AirResistance = airResistance;

            return this;
        }

        public Map SetGravity(float x, float y)
        {
            PhysicalEnvironment.Gravity = new Vector2f(x, y);

            return this;
        }

        public Map SetCollisionResponseMode(CollisionResponseMode collisionResponseMode)
        {
            PhysicalEnvironment.CollisionResponseMode = collisionResponseMode;

            return this;
        }

        public Map GenerateMap(VoxelType ground, int groundLevel, float scale)
        {
            for (uint chunkI = 0; chunkI < MapWidth; chunkI++)
            {
                for (uint chunkJ = 0; chunkJ < MapHeight; chunkJ++)
                {
                    Chunk chunk = Chunks[chunkI, chunkJ] = new Chunk(this);

                    for (uint voxelI = 0; voxelI < ChunkSize; voxelI++)
                    {
                        for (uint voxelJ = 0; voxelJ < ChunkSize; voxelJ++)
                        {
                            bool isAir = chunkJ * ChunkSize + voxelJ > groundLevel - Noise.CalcPixel1D(
                                         (int) (chunkI * ChunkSize + voxelI),
                                         scale);

                            chunk[voxelI, voxelJ] = 
                                isAir
                                ? new Voxel(this, ground)
                                : new Voxel(this);
                        }
                    }
                }
            }

            return this;
        }

        public Map AddPlayer(Vector2f position, int size)
        {
            Entities.Add(new Player(this, position, size));

            return this;
        }

        public Map AddProjectile(Vector2f position, Vector2f velocity)
        {
            Entities.Add(new Projectile(this, position, velocity));

            return this;
        }
        #endregion

        public void Step() => Entities.ForEach(e => e.Step());

        public Vector2i GetChunkIndex(Vector2f position) =>
            new Vector2i((int)MathF.Floor(position.X), (int)MathF.Floor(position.Y));

        public Chunk GetChunk(Vector2f position)
        {
            int i = (int)MathF.Floor(position.X);
            int j = (int)MathF.Floor(position.Y);
            return i < 0 || j < 0 || i >= MapWidth || j >= MapHeight ? null : Chunks[i, j];
        }

        public Voxel GetVoxel(Vector2f position) => GetChunk(position)?.GetVoxel(position) ?? new Voxel(this, null);
    }
}