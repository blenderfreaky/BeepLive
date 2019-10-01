namespace BeepLive.Config
{
    using System;

    public class ClusterShotConfig<TShotConfig> : ShotConfig
        where TShotConfig : ShotConfig
    {
        public int ChildCount;

        public TShotConfig ChildShotConfig;
        public float ExplosionPower;

        public ClusterShotConfig()
        {
            ChildShotConfig = Activator.CreateInstance<TShotConfig>();
        }
    }

    public class ClusterShotConfig : ClusterShotConfig<ShotConfig> { }
}