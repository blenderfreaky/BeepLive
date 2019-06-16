using System;
using System.Collections.Generic;
using System.IO;
using BeepLive.Entities;
using BeepLive.World;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Threading;

namespace BeepLive
{
    public class BeepLive
    {
        public RenderWindow Window;
        public Map Map;
        public List<Team> Teams;

        public BeepLive()
        {
            var mode = new VideoMode(800, 600);
            Window = new RenderWindow(mode, "Map");

            Window.KeyPressed += Window_KeyPressed;
            Window.MouseButtonPressed += Window_MousePressed;
            Window.Closed += Window_Closed;

            Map = new Map()
                .SetAirResistance(0.99f)
                .SetGravity(0, 1)
                .SetSize(8, 4)
                .SetChunkSize(128)
                .SetCollisionResponseMode(CollisionResponseMode.Raise)
                .SetBackgroundColor(new Color(0, 0, 0))
                .GenerateMap(new VoxelType
                {
                    Color = new Color(127, 127, 127),
                    Resistance = 0.99f
                }, 100, 0.01f, 20f)
                .AddPlayer(new Vector2f(50, 10), 10)
                .AddProjectile(new Vector2f(100, 50), new Vector2f(0, 0), 2);

            using var physicsTimer = new Timer(_ => Map.Step(), null, 1000, 1000 / 60);

            while (Window.IsOpen)
            {
                Window.DispatchEvents();
                Window.Clear(Color.Black);

                GameLoop();

                Window.Display();
            }
        }

        private void GameLoop()
        {
            for (int chunkI = 0; chunkI < Map.MapWidth; chunkI++)
            {
                for (int chunkJ = 0; chunkJ < Map.MapHeight; chunkJ++)
                {
                    Chunk chunk = Map.Chunks[chunkI, chunkJ];
                    chunk.Update();
                    Window.Draw(chunk.Sprite);
                }
            }

            foreach (Entity entity in Map.Entities)
            {
                Window.Draw(entity.Shape);
            }
        }

        private static void Window_KeyPressed(object sender, KeyEventArgs e)
        {
            var window = (Window)sender;
            if (e.Code == Keyboard.Key.Escape)
            {
                window.Close();
            }
        }

        private void Window_MousePressed(object sender, MouseButtonEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Window.Close();
        }
    }
}