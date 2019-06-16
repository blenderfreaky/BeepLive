using SFML.Graphics;

namespace BeepLive.World
{
    public struct Voxel
    {
        public Map Map;
        public VoxelType VoxelType;
        public bool IsAir;

        public Voxel(Map map, Color color)
        {
            Map = map;
            IsAir = color == map.BackgroundColor;
            VoxelType = IsAir
                ? null
                : map.PhysicalEnvironment.VoxelTypes.Find(t => t.Color == color);
        }

        public Voxel(Map map, VoxelType voxelType = null)
        {
            Map = map;
            IsAir = voxelType == null;
            VoxelType = voxelType;
        }

        public Color Color => VoxelType?.Color ?? Map.BackgroundColor;
    }
}