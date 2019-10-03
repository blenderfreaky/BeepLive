namespace BeepLive.Config
{
    using System;
    using System.Xml.Serialization;

    public class ClusterShotConfig<TShotConfig> : ShotConfig
        where TShotConfig : ShotConfig
    {
        public int ChildCount;

        [XmlElement(nameof(ShotConfig), typeof(ShotConfig))]
        public TShotConfig ChildShotConfig;

        public float ExplosionPower;

        public ClusterShotConfig()
        {
            ChildShotConfig = Activator.CreateInstance<TShotConfig>();
        }
    }

    public class ClusterShotConfig : ClusterShotConfig<ShotConfig> { }
}