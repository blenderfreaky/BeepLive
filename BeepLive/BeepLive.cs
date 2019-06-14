using System;
using System.Collections.Generic;
using System.Text;
using SFML.Graphics;
using SFML.System;
using UltimateQuadTree;

namespace BeepLive
{
    public class BeepLive
    {
        public RenderWindow Window;
        private readonly Map _map;

        public BeepLive()
        {
            var mode = new SFML.Window.VideoMode(800, 600);
            this.Window = new RenderWindow(mode, "SFML works!");

            Window.KeyPressed += Window_KeyPressed;


            var circle = new CircleShape(100f)
            {
                FillColor = Color.Blue
            };
            var voxel = new RectangleShape(new Vector2f(10, 10));

            // Start the game loop
            while (Window.IsOpen)
            {
                // Process events
                Window.DispatchEvents();
                Window.Draw(circle);

                // Finally, display the rendered frame on screen
                Window.Display();
            }

            _map = new Map();
        }


        /// <summary>
        /// Function called when a key is pressed
        /// </summary>
        private void Window_KeyPressed(object sender, SFML.Window.KeyEventArgs e)
        {
            var window = (SFML.Window.Window)sender;
            if (e.Code == SFML.Window.Keyboard.Key.Escape)
            {
                window.Close();
            }
        }
    }
}