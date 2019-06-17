using BeepLive.World;
using SFML.Graphics;
using SFML.System;
using System;

namespace BeepLive.Entities
{
    public abstract class Entity
    {
        public Map Map;
        public Drawable Shape;
        public virtual Vector2f Position { get; set; }
        public Vector2f Velocity { get; set; }

        public abstract void Step();

        public Voxel GetVoxel(float x, float y)
        {
            return Map.GetVoxel(Position + new Vector2f(x, y));
        }

        public Voxel GetVoxelUnscaled(float x, float y)
        {
            return Map.GetVoxel(Position + new Vector2f(x, y));
        }

        protected bool disposed = false;
        public bool Disposed { get => disposed; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed) disposed = true;
        }
        ~Entity()
        {
            Dispose(false);
        }
    }
}