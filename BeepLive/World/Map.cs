using System;
using System.Collections.Generic;
using BeepLive.Entities;
using SFML.Graphics;
using SFML.System;

namespace BeepLive.World
{
    public class Map
    {
        public PhysicalEnvironment PhysicalEnvironment;
        public List<Entity> Entities;
        public int ChunkSize;
        public int MapWidth, MapHeight;
        public float VoxelScale;
        public Chunk[,] Chunks;

        public Map(int chunkSize, int mapWidth, int mapHeight, float voxelScale)
        {
            if (mapWidth <= 0) throw new ArgumentOutOfRangeException(nameof(mapWidth));
            MapWidth = mapWidth;
            if (mapHeight <= 0) throw new ArgumentOutOfRangeException(nameof(mapHeight));
            MapHeight = mapHeight;
            Chunks = new Chunk[mapWidth, mapHeight];

            if (chunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSize));
            ChunkSize = chunkSize;

            VoxelScale = voxelScale;

            for (int chunkI = 0; chunkI < MapWidth; chunkI++)
            {
                for (int chunkJ = 0; chunkJ < MapHeight; chunkJ++)
                {
                    Chunk chunk = Chunks[chunkI, chunkJ] = new Chunk(this);

                    for (int voxelI = 0; voxelI < ChunkSize; voxelI++)
                    {
                        for (int voxelJ = 0; voxelJ < ChunkSize; voxelJ++)
                        {
                            chunk.Voxels[voxelI, voxelJ] = 
                                 new Voxel(new Vector2f(chunkI * ChunkSize + voxelI,
                                    chunkJ * ChunkSize + voxelJ) * VoxelScale,
                                null, VoxelScale);
                        }
                    }
                }
            }

            Entities = new List<Entity>();
        }

        public void Step()
        {
            Entities.ForEach(e => e.Step());
        }

        public Vector2i GetChunkIndex(Vector2f position) =>
            new Vector2i((int)MathF.Floor(position.X / VoxelScale), (int)MathF.Floor(position.Y / VoxelScale));

        public Chunk GetChunk(Vector2f position) =>
            Chunks[(int)MathF.Floor(position.X / VoxelScale), (int)MathF.Floor(position.Y / VoxelScale)];

        public Voxel GetVoxel(Vector2f position) =>
            Chunks[(int)MathF.Floor(position.X / VoxelScale), (int)MathF.Floor(position.Y / VoxelScale)]
                .GetVoxel(position / VoxelScale);
    }
}