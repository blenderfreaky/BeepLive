using System;
using BeepLive.Game;
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

        public TeamRelation GetTeamRelation(Team team)
        {
            return VoxelType.OwnerTeam == null
                ? TeamRelation.Neutral
                : VoxelType == null
                    ? TeamRelation.Air
                    : VoxelType.OwnerTeam == team
                        ? TeamRelation.Friendly
                        : TeamRelation.Hostile;
        }
    }

    [Flags]
    public enum TeamRelation : sbyte
    {
        Air = 0,
        Friendly = 1 << 0,
        Neutral = 1 << 1,
        Hostile = 1 << 2
    }
}