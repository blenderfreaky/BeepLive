using System;
using SFML.System;

namespace BeepLive.World
{
    public class Chunk
    {
        public Map Map;
        public Voxel[,] Voxels;

        public Chunk(Map map)
        {
            Map = map;
            Voxels = new Voxel[map.ChunkSize, map.ChunkSize];
        }

        public Vector2i GetVoxelIndex(Vector2f unscaledPosition) =>
            new Vector2i((int)Math.Floor(unscaledPosition.X), (int)MathF.Floor(unscaledPosition.Y));

        public Voxel GetVoxel(Vector2f unscaledPosition) =>
            Voxels[(int)Math.Floor(unscaledPosition.X), (int)MathF.Floor(unscaledPosition.Y)];
    }
}