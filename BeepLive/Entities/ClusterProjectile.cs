using System;
using BeepLive.World;
using SFML.System;

namespace BeepLive.Entities
{
    public class ClusterProjectile<TProjectile> : Projectile
        where TProjectile : Projectile
    {
        public int ChildCount;
        public float ChildRadius;
        public float ExplosionPower;
        public float ExplosionProneness;
        public float ChildLowestSpeed;
        
        public ClusterProjectile(Map map, Vector2f position, Vector2f velocity, float radius, float lowestSpeed,
            int childCount,
            float childRadius, float explosionPower, float childLowestSpeed) : base(map, position, velocity, radius, lowestSpeed)
        {
            ChildCount = childCount;
            ChildRadius = childRadius;
            ExplosionPower = explosionPower;
            ChildLowestSpeed = childLowestSpeed;
            ExplosionProneness = lowestSpeed;
        }

        public override void Step()
        {
            base.Step();

            float dist = MathF.Sqrt(Velocity.X * Velocity.X + Velocity.Y * Velocity.Y);

            if (dist <= ExplosionProneness) Die();
        }

        public override void Die()
        {
            base.Die();

            Explode();
        }

        public void Explode()
        {
            for (int i = 0; i < ChildCount; i++)
            {
                var direction = new Vector2f((float) Map.Random.NextDouble() * ExplosionPower,
                    (float) Map.Random.NextDouble() * ExplosionPower);
                Map.Entities.Add(Activator.CreateInstance(typeof(TProjectile), Map, Position, Velocity + direction,
                    ChildRadius, ChildLowestSpeed) as TProjectile);
            }
        }
    }
}