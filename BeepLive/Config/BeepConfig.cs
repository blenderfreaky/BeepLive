using System.Collections.Generic;

namespace BeepLive.Config
{
    public class BeepConfig
    {
        public MapConfig MapConfig;
        public List<TeamConfig> TeamConfigs;
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