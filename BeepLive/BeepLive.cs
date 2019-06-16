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

            Map = new Map()
                .SetAirResistance(0.99f)
                .SetGravity(0, 1)
                .GenerateMap(new VoxelType(), 100, 0.01f)
                .AddPlayer(new Vector2f(50, 10), 10)
                .AddProjectile( new Vector2f(100, 50), new Vector2f(0, 0));

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
                    Window.Draw(Map.Chunks[chunkI, chunkJ].Sprite);
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
    }
}