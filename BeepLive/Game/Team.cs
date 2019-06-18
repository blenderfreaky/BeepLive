using System;
using System.Collections.Generic;
using BeepLive.Config;
using BeepLive.Entities;
using BeepLive.World;

namespace BeepLive.Game
{
    public class Team
    {
        public BeepLiveGame BeepLiveGame;
        public List<Player> Players;
        public VoxelType VoxelType;

        public Team(BeepLiveGame beepLiveGame, TeamConfig teamConfig)
        {
            BeepLiveGame = beepLiveGame;
            Players = new List<Player>(teamConfig.MaxPlayers);
            VoxelType = new VoxelType
                {OwnerTeam = this, Color = teamConfig.Color, Resistance = teamConfig.TerritoryResistance};
        }

        #region Fluent API

        public Team SetVoxelType(VoxelType voxelType)
        {
            voxelType.OwnerTeam = this;
            VoxelType = voxelType;

            return this;
        }

        public Team AddPlayer(Func<Player, Player> playerMaker, out Player player)
        {
            player = playerMaker(new Player(BeepLiveGame.Map));
            player.GenerateShape();
            Players.Add(player);

            return this;
        }

        public Team AddPlayer(Func<Player, Player> playerMaker)
        {
            Player player = playerMaker(new Player(BeepLiveGame.Map));
            player.GenerateShape();
            Players.Add(player);

            return this;
        }

        #endregion
    }
}