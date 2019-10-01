namespace BeepLive.Config
{
    using BeepLive.World;
    using System.Xml.Serialization;

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
    }
}