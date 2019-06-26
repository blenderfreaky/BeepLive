using System.Xml.Serialization;
using BeepLive.World;

namespace BeepLive.Config
{
    [XmlInclude(typeof(ClusterShotConfig))]
    public class ShotConfig
    {
        public TeamRelation Damages;
        public bool Destructive;

        public float FriendlyResistanceFactor, NeutralResistanceFactor, HostileResistanceFactor;
        public float LowestSpeed;
        public int MaxLifeTime;

        public bool Neutral;
        public float Radius;

        public ShotConfig()
        {}
    }
}