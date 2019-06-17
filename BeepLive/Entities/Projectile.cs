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
            var voxel = Map.GetVoxel(Position);

            Velocity += Map.PhysicalEnvironment.Gravity;
            Velocity *= voxel.IsAir
                ? Map.PhysicalEnvironment.AirResistance
                : voxel.VoxelType.Resistance;

            var dist = MathF.Sqrt(Velocity.X * Velocity.X + Velocity.Y * Velocity.Y);

            if (dist < LowestSpeed ||
                LifeTime++ > MaxLifeTime ||
                !Map.EntityBoundary.Contains(Position) ||
                Map.Players.Any(p => p.Boundary.Contains(Position)))
                Die();

            var front = Velocity / dist;
            var left = new Vector2f(front.Y, -front.X);

            for (float x = 0; x < dist; x += .5f)
            for (var y = -Radius; y <= Radius; y++)
            {
                var position = Position + front * x + left * y;

                var chunk = Map.GetChunk(position, out var chunkPosition);
                if (chunk == null) continue;
                chunk[(uint) MathF.Floor(position.X - chunkPosition.X),
                    (uint) MathF.Floor(position.Y - chunkPosition.Y)] = new Voxel(Map);
            }

            Position += Velocity;
        }

        public virtual void Die()
        {
            Map.Entities.Remove(this);
        }
    }
}