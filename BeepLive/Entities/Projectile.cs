using BeepLive.World;
using SFML.System;

namespace BeepLive.Entities
{
    public class Projectile : Entity
    {
        public Projectile(Vector2f position, Vector2f velocity)
        {
            Position = position;
            Velocity = velocity;
        }

        public override void Step()
        {
            Chunk chunk = Map.GetChunk(Position);
            Vector2i index = chunk.GetVoxelIndex(Position / Map.VoxelScale);
            Voxel voxel = chunk.Voxels[index.X, index.Y];

            chunk.Voxels[index.X, index.Y] = new Voxel(voxel.Position, voxel.VoxelType, Map.VoxelScale);

            Velocity += Map.PhysicalEnvironment.Gravity;
            Velocity *= voxel.IsAir
                ? Map.PhysicalEnvironment.AirResistance
                : voxel.VoxelType.Resistance;

            Position += Velocity;
        }
    }
}