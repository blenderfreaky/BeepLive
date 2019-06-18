using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BeepLive.Config;
using BeepLive.Entities;
using BeepLive.World;

namespace BeepLive.Game
{
    public class BeepLiveGame
    {
        public BeepConfig BeepConfig;
        public Player LocalPlayer;
        public Map Map;
        public List<Team> Teams;

        public BeepLiveGame(BeepConfig beepConfig)
        {
            BeepConfig = beepConfig;

            Teams = new List<Team>(beepConfig.TeamConfigs.Count);
            Teams = beepConfig.TeamConfigs.Select(x => new Team(this, x));
        }

        public Timer Run()
        {
            return new Timer(_ => Map?.Step(), null, 1000, 1000 / 60);
        }
    }
}