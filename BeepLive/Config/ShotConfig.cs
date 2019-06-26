using System;
using System.Xml.Serialization;
using BeepLive.World;

namespace BeepLive.Config
{
    public class ShotConfig
    {
        public float LowestSpeed;
        public int MaxLifeTime;
        public float Radius;

        public float FriendlyResistanceFactor, NeutralResistanceFactor, HostileResistanceFactor;
        public TeamRelation Damages;

        public bool Neutral;
        public bool Destructive;
    }

    public class ClusterShotConfig<TShotConfig> : ShotConfig
        where TShotConfig : ShotConfig
    {
        public int ChildCount;
        public float ExplosionPower;

        public TShotConfig ChildShotConfig;
    }
}