namespace BeepLive.Game
{
    using SFML.Graphics;
    using System.Collections.Generic;

    public class TeamMock
    {
        public string TeamGuid;
        public List<PlayerMock> Players;
        public Color TeamColor;
        public int MaxPlayers;

        public TeamMock()
        {
            Players = new List<PlayerMock>();
        }
    }
}