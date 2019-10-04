namespace BeepLive.Game
{
    using BeepLive.Config;
    using BeepLive.Entities;
    using BeepLive.World;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class BeepLiveGame
    {
        public BeepConfig BeepConfig;
        public Player LocalPlayer;
        public Team LocalTeam;
        public Map Map;
        public List<Team> Teams;
        public readonly Guid PlayerGuid;

        public BeepLiveGame(BeepConfig beepConfig, Guid playerGuid)
        {
            BeepConfig = beepConfig;
            PlayerGuid = playerGuid;

            Map = new Map(beepConfig.MapConfig);

            Teams = beepConfig.TeamConfigs.Select(x => new Team(this, x)).ToList();
        }

        public Timer Run()
        {
            Map.Config.PhysicalEnvironment.CalculateVoxelTypesByColor();

            return new Timer(_ => Map.Step(), null, 1000, 1000 / 60);
        }
    }
}