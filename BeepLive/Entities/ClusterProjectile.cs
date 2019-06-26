using System;
using BeepLive.Config;
using BeepLive.World;
using SFML.System;

namespace BeepLive.Entities
{
    public class ClusterProjectile<TProjectile, TShotConfig> : Projectile<ClusterShotConfig<TShotConfig>>
        where TProjectile : Projectile<TShotConfig>
        where TShotConfig : ShotConfig
    {
        public delegate void OnExplode();


        public ClusterProjectile(Map map, Vector2f position, Vector2f velocity,
            ClusterShotConfig<TShotConfig> shotConfig,
            Player owner = null) : base(map, position, velocity, shotConfig, owner)
        {
        }

        public event OnExplode OnExplodeEvent;

        public override void Die()
        {
            base.Die();

            Explode();
        }

        public void Explode()
        {
            for (int i = 0; i < ShotConfig.ChildCount; i++)
            {
                var direction = new Vector2f((float) (Map.Random.NextDouble() * 2 - 1) * ShotConfig.ExplosionPower,
                    (float) (Map.Random.NextDouble() * 2 - 1) * ShotConfig.ExplosionPower);
                Map.Entities.Add(Activator.CreateInstance(typeof(TProjectile), Map, Position, Velocity + direction,
                    ShotConfig.ChildShotConfig) as TProjectile);
            }

            OnExplodeEvent?.Invoke();
        }
    }
}