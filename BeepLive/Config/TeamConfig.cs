using SFML.Graphics;

namespace BeepLive.Config
{
    public class TeamConfig
    {
        public Color Color;

        public ShotConfig DestructiveShotConfig, GenerativeShotConfig;
        public int MaxPlayers;

        public float TerritoryResistance;
        public int PlayerSize;

        public TeamConfig()
        {
            DestructiveShotConfig = new ShotConfig();
            GenerativeShotConfig = new ShotConfig();
        }
    }
}