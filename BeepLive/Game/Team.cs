using System;
using System.Collections.Generic;
using BeepLive.Entities;
using BeepLive.World;
using SFML.Graphics;

namespace BeepLive.Game
{
    public class Team
    {
        public BeepLiveSfml BeepLiveSfml;
        public List<Player> Players;
        public VoxelType VoxelType;

        public Team(BeepLiveSfml beepLiveSfml)
        {
            BeepLiveSfml = beepLiveSfml;
            Players = new List<Player>();
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
            player = playerMaker(new Player(BeepLiveSfml.BeepLive.Map));
            player.GenerateShape();
            Players.Add(player);

            return this;
        }

        public Team AddPlayer(Func<Player, Player> playerMaker)
        {
            Player player = playerMaker(new Player(BeepLiveSfml.BeepLive.Map));
            player.GenerateShape();
            Players.Add(player);

            return this;
        }

        #endregion
    }
}