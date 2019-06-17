using System;
using System.Linq;
using BeepLive.World;
using SFML.Graphics;
using SFML.System;

namespace BeepLive.Entities
{
    public class Projectile : Entity
    {
        public int LifeTime, MaxLifeTime;
        public float LowestSpeed;

        public float Radius;

        public Projectile(Map map, Vector2f position, Vector2f velocity, float radius, float lowestSpeed,
            int maxLifeTime)
        {
            Shape = new CircleShape
            {
                Position = position,
                Radius = radius,
                FillColor = Color.Yellow
            };

            Map = map;
            Position = position;
            Velocity = velocity;
            Radius = radius;
            LowestSpeed = lowestSpeed;
            MaxLifeTime = maxLifeTime;
        }

        public CircleShape CircleShape
        {
            get => (CircleShape) Shape;
            set => Shape = value;
        }

        public sealed override Vector2f Position
        {
            get => CircleShape.Position;
            set => CircleShape.Position = value;
        }

        public override void Step()
        {
            Voxel voxel = Map.GetVoxel(Position);

            Velocity += Map.Config.PhysicalEnvironment.Gravity;
            Velocity *= voxel.IsAir
                ? Map.Config.PhysicalEnvironment.AirResistance
                : voxel.VoxelType.Resistance;

            float dist = MathF.Sqrt(Velocity.X * Velocity.X + Velocity.Y * Velocity.Y);

            if (dist < LowestSpeed ||
                LifeTime++ > MaxLifeTime ||
                !Map.Config.EntityBoundary.Contains(Position) ||
                Map.Players.Any(p => p.Boundary.Contains(Position)))
                Die();

            Vector2f front = Velocity / dist;
            Vector2f left = new Vector2f(front.Y, -front.X);

            for (float x = 0; x < dist; x += .5f)
            for (float y = -Radius; y <= Radius; y++)
            {
                Vector2f position = Position + front * x + left * y;

                Chunk chunk = Map.GetChunk(position, out Vector2f chunkPosition);
                if (chunk == null) continue;
                chunk[(uint) MathF.Floor(position.X - chunkPosition.X),
                    (uint) MathF.Floor(position.Y - chunkPosition.Y)] = new Voxel(Map);
            }

            Position += Velocity;
        }

        public virtual void Die()
        {
            Map.Entities.Remove(this);
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (!Disposed) CircleShape.Dispose();
            base.Dispose(disposing);
        }
    }
}