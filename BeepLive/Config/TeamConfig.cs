using SFML.Graphics;

namespace BeepLive.Config
{
    public class TeamConfig
    {
        public Color Color;

        public ShotConfig DestructiveShotConfig, GenerativeShotConfig;
        public int MaxPlayers;
        public int PlayerSize;

        public float TerritoryResistance;

        public TeamConfig()
        {
            DestructiveShotConfig = new ShotConfig();
            GenerativeShotConfig = new ShotConfig();
        }
    }
}