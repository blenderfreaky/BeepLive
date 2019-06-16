using System.Security.Cryptography;
using BeepLive.World;
using SFML.Graphics;
using SFML.System;

namespace BeepLive.Entities
{
    public class Projectile : Entity
    {
        public CircleShape CircleShape
        {
            get => (CircleShape)Shape;
            set => Shape = value;
        }
        public sealed override Vector2f Position
        {
            get => CircleShape.Position;
            set => CircleShape.Position = value;
        }

        public Projectile(Map map, Vector2f position, Vector2f velocity)
        {
            Shape = new CircleShape
            {
                Position = position,
                Radius = 10,
                FillColor = Color.Yellow,
            };

            Map = map;
            Position = position;
            Velocity = velocity;
        }

        public override void Step()
        {
            Chunk chunk = Map.GetChunk(Position);

            if (chunk != null)
            {
                Vector2u index = chunk.GetVoxelIndex(Position);
                Voxel voxel = chunk[index.X, index.Y];

                chunk[index.X, index.Y] = new Voxel(Map, voxel.VoxelType);

                Velocity += Map.PhysicalEnvironment.Gravity;
                Velocity *= voxel.IsAir
                    ? Map.PhysicalEnvironment.AirResistance
                    : voxel.VoxelType.Resistance;
            }

            Velocity += Map.PhysicalEnvironment.Gravity;
            Velocity *= Map.PhysicalEnvironment.AirResistance;

            Position += Velocity;
        }
    }
}