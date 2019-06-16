using SFML.Graphics;
using SFML.System;

namespace BeepLive.World
{
    public struct Voxel
    {
        public Vector2f Position;
        public VoxelType VoxelType;
        public bool IsAir;
        public RectangleShape Shape;

        public Voxel(Vector2f position, VoxelType voxelType, float voxelScale)
        {
            Position = position;
            VoxelType = voxelType;
            IsAir = voxelType == null;

            Shape = new RectangleShape(new Vector2f(voxelScale, voxelScale))
            {
                Position = position,
                FillColor = Color.Blue,
            };
        }
    }
}