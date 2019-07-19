using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BeepLive.Config;
using BeepLive.Entities;
using BeepLive.World;
using SFML.System;

namespace BeepLive.Game
{
    public class BeepLiveGame
    {
        public BeepConfig BeepConfig;
        public Player LocalPlayer;
        public Team LocalTeam;
        public Map Map;
        public List<Team> Teams;
        public readonly string PlayerGuid;

        public BeepLiveGame(BeepConfig beepConfig, string playerGuid)
        {
            BeepConfig = beepConfig;
            PlayerGuid = playerGuid;

            Map = new Map(beepConfig.MapConfig);

            Teams = beepConfig.TeamConfigs.Select(x => new Team(this, x)).ToList();
        }

        public Timer Run() => new Timer(_ => Map?.Step(), null, 1000, 1000 / 60);

        public void JoinTeam(Team team)
        {
            if (team.Players.Count >= team.TeamConfig.MaxPlayers) throw new InvalidOperationException("Can't join full team");

            LocalPlayer = new Player(Map, new Vector2f(), team.TeamConfig.PlayerSize, team, PlayerGuid);

            team.Players.Add(LocalPlayer);
        }
    }
}