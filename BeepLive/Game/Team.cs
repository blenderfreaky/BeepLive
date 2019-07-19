using BeepLive.Config;
using BeepLive.Entities;
using BeepLive.World;
using System.Collections.Generic;

namespace BeepLive.Game
{
    public class Team
    {
        public BeepLiveGame BeepLiveGame;
        public List<Player> Players;
        public TeamConfig TeamConfig;
        public VoxelType VoxelType;

        public Team(BeepLiveGame beepLiveGame, TeamConfig teamConfig)
        {
            BeepLiveGame = beepLiveGame;
            TeamConfig = teamConfig;
            Players = new List<Player>(teamConfig.MaxPlayers);
            VoxelType = new VoxelType
            { OwnerTeam = this, Color = teamConfig.Color, Resistance = teamConfig.TerritoryResistance };
        }
    }
}