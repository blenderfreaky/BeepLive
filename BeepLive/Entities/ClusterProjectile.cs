using System;
using BeepLive.Config;
using BeepLive.World;
using SFML.System;

namespace BeepLive.Entities
{
    public class ClusterProjectile<TProjectile> : Projectile
        where TProjectile : Projectile
    {
        public int ChildCount;
        public float ChildLowestSpeed;
        public int ChildMaxLifeTime;
        public float ChildRadius;
        public float ExplosionPower;
        public delegate void OnExplode();
        public event OnExplode OnExplodeEvent;

        public ClusterProjectile(Map map, Vector2f position, Vector2f velocity, float radius, float lowestSpeed,
            int maxLifeTime, int childCount, float childRadius, float explosionPower, float childLowestSpeed,
            int childMaxLifeTime) : base(map, position,
            velocity, radius, lowestSpeed, maxLifeTime)
        {
            ChildCount = childCount;
            ChildRadius = childRadius;
            ExplosionPower = explosionPower;
            ChildLowestSpeed = childLowestSpeed;
            ChildMaxLifeTime = childMaxLifeTime;
        }

        public ClusterProjectile(Map map, Vector2f position, Vector2f velocity, ShotConfig shotConfig)
            : this(map, position, velocity, shotConfig.Radius, shotConfig.LowestSpeed, shotConfig.MaxLifeTime,
                shotConfig.ChildCount, shotConfig.ChildRadius, shotConfig.ExplosionPower, shotConfig.ChildLowestSpeed,
                shotConfig.ChildMaxLifeTime)
        {
        }

        public override void Die()
        {
            base.Die();

            Explode();
        }

        public void Explode()
        {
            for (var i = 0; i < ChildCount; i++)
            {
                var direction = new Vector2f((float) (Map.Random.NextDouble() * 2 - 1) * ExplosionPower,
                    (float) (Map.Random.NextDouble() * 2 - 1) * ExplosionPower);
                Map.Entities.Add(Activator.CreateInstance(typeof(TProjectile), Map, Position, Velocity + direction,
                    ChildRadius, ChildLowestSpeed, ChildMaxLifeTime) as TProjectile);
            }

            OnExplodeEvent?.Invoke();
        }
    }
}