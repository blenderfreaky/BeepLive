using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using BeepLive.Entities;
using BeepLive.World;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace BeepLive
{
    public class BeepLive
    {
        public RenderWindow Window;
        public Map Map;

        public BeepLive()
        {
            var mode = new VideoMode(800, 600);
            Window = new RenderWindow(mode, "BeepLive");

            Window.KeyPressed += Window_KeyPressed;
            Window.MouseButtonPressed += Window_MousePressed;

            Map = new Map(128, 8, 4, 1)
            {
                PhysicalEnvironment = new PhysicalEnvironment
                {
                    AirResistance = 0.99f,
                    Gravity = new Vector2f(0, -1f),
                    CollisionResponseMode = CollisionResponseMode.LeastResistance,
                }
            };
            Map.Entities.Add(new Player(Map, new Vector2f(50, 50), 10));
            Map.Entities.Add(new Projectile(new Vector2f(100, 50), new Vector2f(0, 0)));

            using var physicsTimer = new Timer(_ => Map.Step(), null, 0, 1000/60);

            while (Window.IsOpen)
            {
                Window.DispatchEvents();

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
                    for (int voxelI = 0; voxelI < Map.ChunkSize; voxelI++)
                    {
                        for (int voxelJ = 0; voxelJ < Map.ChunkSize; voxelJ++)
                        {
                            Window.Draw(Map.Chunks[chunkI, chunkJ].Voxels[voxelI, voxelJ].Shape);
                        }
                    }
                }
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