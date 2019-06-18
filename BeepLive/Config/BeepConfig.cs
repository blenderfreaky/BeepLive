using System.Collections.Generic;

namespace BeepLive.Config
{
    public class BeepConfig
    {
        public MapConfig MapConfig;
        public List<TeamConfig> TeamConfigs;

        public BeepConfig()
        {
            TeamConfigs = new List<TeamConfig> {new TeamConfig()};
            MapConfig = new MapConfig();
        }
    }
}