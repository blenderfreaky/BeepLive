using System;
using System.Collections.Generic;
using System.Linq;
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
        public Map Map;
        public List<Team> Teams;
        public Player LocalPlayer;
        public RenderWindow Window;

        public BeepLive()
        {
            VideoMode mode = new VideoMode(800, 600);
            Window = new RenderWindow(mode, "Map");

            Window.KeyPressed += Window_KeyPressed;
            Window.MouseButtonPressed += Window_MousePressed;
            Window.Closed += Window_Closed;

            Teams = new List<Team>();
        }

        #region Fluent API

        public BeepLive AddMap(Func<Map, Map> mapMaker)
        {
            Map = mapMaker(new Map());

            return this;
        }

        public BeepLive AddTeam(Func<Team, Team> teamMaker)
        {
            Teams.Add(teamMaker(new Team(this)));

            return this;
        }

        public BeepLive SetLocalPlayer(Player localPlayer)
        {
            LocalPlayer = localPlayer;

            return this;
        }

        #endregion

        public BeepLive Run()
        {
            using Timer physicsTimer = new Timer(_ => Map.Step(), null, 1000, 1000 / 60);

            while (Window.IsOpen)
            {
                Window.DispatchEvents();
                Window.Clear(Color.Black);

                GameLoop();

                Window.Display();
            }

            return this;
        }

        private void GameLoop()
        {
            for (int chunkI = 0; chunkI < Map.MapWidth; chunkI++)
            for (int chunkJ = 0; chunkJ < Map.MapHeight; chunkJ++)
            {
                Chunk chunk = Map.Chunks[chunkI, chunkJ];
                chunk.Update();
                Window.Draw(chunk.Sprite);
            }

            Entity[] entities;
            lock (Map.Entities)
            {
                entities = Map.Entities.Where(e => !(e is null)).ToArray();
            }

            foreach (Entity entity in entities) Window.Draw(entity.Shape);
        }

        private static void Window_KeyPressed(object sender, KeyEventArgs e)
        {
            Window window = (Window) sender;
            if (e.Code == Keyboard.Key.Escape) window.Close();
        }

        private void Window_MousePressed(object sender, MouseButtonEventArgs e)
        {
            Vector2f position = new Vector2f(20, 20);
            Map.AddClusterProjectile(position, new Vector2f(e.X - position.X, e.Y - position.Y) / 10, 4, 10, 300, 200, 2, 10, 5, 200);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Window.Close();
        }
    }
}