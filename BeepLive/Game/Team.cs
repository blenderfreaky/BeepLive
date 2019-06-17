using System;
using System.Collections.Generic;
using BeepLive.Entities;
using SFML.Graphics;

namespace BeepLive.Game
{
    public class Team
    {
        public BeepLive BeepLive;
        public List<Player> Players;
        public Color TeamColor;

        public Team(BeepLive beepLive)
        {
            BeepLive = beepLive;
            Players = new List<Player>();
        }

        #region Fluent API

        public Team SetTeamColor(Color teamColor)
        {
            TeamColor = teamColor;

            return this;
        }

        public Team AddPlayer(Func<Player, Player> playerMaker, out Player player)
        {
            player = playerMaker(new Player(BeepLive.Map));
            player.GenerateShape();
            Players.Add(player);

            return this;
        }

        public Team AddPlayer(Func<Player, Player> playerMaker)
        {
            Player player = playerMaker(new Player(BeepLive.Map));
            player.GenerateShape();
            Players.Add(player);

            return this;
        }

        #endregion
    }
}