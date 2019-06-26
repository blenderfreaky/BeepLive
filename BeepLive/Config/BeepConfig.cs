using System.Collections.Generic;

namespace BeepLive.Config
{
    public class BeepConfig
    {
        public MapConfig MapConfig;
        public List<TeamConfig> TeamConfigs;
        public List<ShotConfig> ShotConfigs;

        public BeepConfig()
        {
            MapConfig = new MapConfig();
            TeamConfigs = new List<TeamConfig> { new TeamConfig() };
            ShotConfigs = new List<ShotConfig> { new ShotConfig(), new ClusterShotConfig() };
        }
    }
}