using BeepLive.World;
using SFML.Graphics;
using SFML.System;

namespace BeepLive
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            BeepLive beepLive = new BeepLive()
                .AddMap(map => map
                    .SetAirResistance(0.99f)
                    .SetGravity(0, 1)
                    .SetSize(8, 4)
                    .SetChunkSize(128)
                    .SetCollisionResponseMode(CollisionResponseMode.Raise)
                    .SetBackgroundColor(new Color(0, 0, 0))
                    .SetEntityBoundaryAroundChunks(new Vector2f(-100, -100), new Vector2f(100, 100))
                    .GenerateMap(new VoxelType
                    {
                        Color = new Color(127, 127, 127),
                        Resistance = 0.002f
                    }, 100, 0.01f, 20f))
                .AddTeam(team => team)
            ;

            beepLive.Run();
        }
    }
}