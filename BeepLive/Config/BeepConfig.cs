using System.Collections.Generic;
using System.ComponentModel;

namespace BeepLive.Config
{
    public class BeepConfig
    {
        public List<TeamConfig> TeamConfigs;
        public MapConfig MapConfig;

        public BeepConfig()
        {
            TeamConfigs = new List<TeamConfig> {new TeamConfig()};
            MapConfig = new MapConfig();
        }
    }
}