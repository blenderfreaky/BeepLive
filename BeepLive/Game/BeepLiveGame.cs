using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BeepLive.Config;
using BeepLive.Entities;
using BeepLive.Network;
using BeepLive.World;
using SFML.System;

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

            Map = new Map(beepConfig.MapConfig);

            Teams = beepConfig.TeamConfigs.Select(x => new Team(this, x)).ToList();
        }

        public Timer Run()
        {
            return new Timer(_ => Map?.Step(), null, 1000, 1000 / 60);
        }

        public void JoinTeam(Team team)
        {
            if (team.Players.Count < team.TeamConfig.MaxPlayers)
            {
                LocalPlayer = new Player(Map, new Vector2f(), team.TeamConfig.PlayerSize, team);

                team.Players.Add(LocalPlayer);
            }
            else
            {
                throw new InvalidOperationException("Can't join full team");
            }
        }
    }
}