using System.Diagnostics;
using System.Linq;
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
            IsAir = color == map.Config.BackgroundColor;
            VoxelType = IsAir
                ? null
                : map.Config.PhysicalEnvironment.VoxelTypes.Find(t => t.Color == color);
        }

        public Voxel(Map map, VoxelType voxelType = null)
        {
            Map = map;
            IsAir = voxelType == null;
            VoxelType = voxelType;
        }

        public Color Color => VoxelType?.Color ?? Map.Config.BackgroundColor;
    }
}