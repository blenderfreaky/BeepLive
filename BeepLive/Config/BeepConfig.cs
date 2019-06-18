using System.Collections.Generic;
using SFML.Graphics;

namespace BeepLive.Config
{
    public class BeepConfig
    {
        public List<TeamConfig> TeamConfigs;
    }

    public class TeamConfig
    {
        public Color Color;

        public ShotConfig DestructiveShotConfig, GenerativeShotConfig;
        public int MaxPlayers;

        public float TerritoryResistance;
    }

    public class ShotConfig
    {
        public int ChildCount;
        public float ChildLowestSpeed;
        public int ChildMaxLifeTime;

        public float ChildRadius;
        public float ExplosionPower;
        public float LowestSpeed;
        public int MaxLifeTime;
        public float Radius;
    }
}