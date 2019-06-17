using System;
using System.Collections.Generic;
using BeepLive.Entities;
using SFML.Graphics;

namespace BeepLive
{
    public class Team
    {
        public Color TeamColor;
        public List<Player> Players;

        public Team()
        {
            Players = new List<Player>();
        }

        #region Fluent API

        public Team AddPlayer(Func<Player, Player> playerMaker)
        {
            Players.Add(playerMaker(new Player()));

            return this;
        }

        #endregion
    }
}