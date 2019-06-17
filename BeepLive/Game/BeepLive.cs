using System.Collections.Generic;
using System.Threading;
using BeepLive.Entities;
using BeepLive.World;

namespace BeepLive.Game
{
    public class BeepLive
    {
        public Player LocalPlayer;
        public Map Map;
        public List<Team> Teams;

        public BeepLive()
        {
            Teams = new List<Team>();
        }

        public Timer Run() => new Timer(_ => Map.Step(), null, 1000, 1000 / 60);
    }
}