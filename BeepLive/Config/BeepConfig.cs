namespace BeepLive.Config
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class BeepConfig
    {
        public MapConfig MapConfig;

        [XmlArray]
        [XmlArrayItem(nameof(TeamConfig), typeof(TeamConfig))]
        public List<TeamConfig> TeamConfigs;

        [XmlArray]
        [XmlArrayItem(nameof(ShotConfig), typeof(ShotConfig))]
        [XmlArrayItem(nameof(ClusterShotConfig), typeof(ClusterShotConfig))]
        public List<ShotConfig> ShotConfigs;

        public bool AllowTimelyReLock; // If true players can lock in another action after already having locked in, as long as it is still allowed to do so

        public BeepConfig()
        {
            MapConfig = new MapConfig();
            TeamConfigs = new List<TeamConfig>();
            ShotConfigs = new List<ShotConfig>();
        }
    }
}