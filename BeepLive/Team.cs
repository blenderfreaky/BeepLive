using System;
using System.Collections.Generic;
using BeepLive.Entities;
using BeepLive.World;
using SFML.Graphics;
using SFML.System;

namespace BeepLive
{
    public class Team
    {
        public BeepLive BeepLive;
        public Color TeamColor;
        public List<Player> Players;

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