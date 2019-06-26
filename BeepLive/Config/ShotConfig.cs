using BeepLive.World;

namespace BeepLive.Config
{
    public class ShotConfig
    {
        public TeamRelation Damages;
        public bool Destructive;

        public float FriendlyResistanceFactor, NeutralResistanceFactor, HostileResistanceFactor;
        public float LowestSpeed;
        public int MaxLifeTime;

        public bool Neutral;
        public float Radius;
    }

    public class ClusterShotConfig<TShotConfig> : ShotConfig
        where TShotConfig : ShotConfig
    {
        public int ChildCount;

        public TShotConfig ChildShotConfig;
        public float ExplosionPower;
    }
}