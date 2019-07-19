using System;
using BeepLive.World;
using SFML.Graphics;
using SFML.System;

namespace BeepLive.Entities
{
    public abstract class Entity : IDisposable
    {
        protected bool Disposed;
        public Map Map;
        public Drawable Shape;
        public virtual Vector2f Position { get; set; }
        public Vector2f Velocity { get; set; }
        public bool Alive
        {
            get => Disposed;
            set { if (!value) Die(); }
        }

        public abstract void Step();

        public Voxel GetVoxel(float x, float y)
        {
            return Map.GetVoxel(Position + new Vector2f(x, y));
        }

        public Voxel GetVoxelUnscaled(float x, float y)
        {
            return Map.GetVoxel(Position + new Vector2f(x, y));
        }

        public virtual void Die()
        {
            Map.Entities.Remove(this);
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (this) if (!Disposed) Disposed = true;
        }

        ~Entity()
        {
            Dispose(false);
        }
    }
}