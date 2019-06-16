using BeepLive.World;
using SFML.Graphics;
using SFML.System;

namespace BeepLive.Entities
{
    public abstract class Entity
    {
        public Map Map;
        public virtual Vector2f Position { get; set; }
        public Vector2f Velocity { get; set; }
        public Drawable Shape;

        public abstract void Step();

        public Voxel GetVoxel(float x, float y) => Map.GetVoxel(Position + new Vector2f(x, y));
        public Voxel GetVoxelUnscaled(float x, float y) => Map.GetVoxel(Position + new Vector2f(x, y));
    }
}