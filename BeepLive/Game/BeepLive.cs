using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using BeepLive.Entities;
using BeepLive.World;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace BeepLive.Game
{
    public class BeepLive
    {
        public Player LocalPlayer;
        public Map Map;
        public List<Team> Teams;
        public RenderWindow Window;

        #region Camera

        private readonly View _view;
        private Vector2f _center;
        private float _zoom = 1;
        private float _rotation;

        #region Shake

        private readonly Stopwatch _shakeTimer;
        private long _shakeDuration; // In milliseconds
        private float _shakeMagnitude;
        private readonly Random _random;

        #endregion

        #endregion

        public BeepLive()
        {
            VideoMode mode = new VideoMode(800, 600);
            Window = new RenderWindow(mode, "Map");

            _view = new View(new Vector2f(), new Vector2f(Window.Size.X, Window.Size.Y));
            Window.SetView(_view);

            Window.KeyPressed += Window_KeyPressed;
            Window.MouseButtonPressed += Window_MousePressed;
            Window.Closed += Window_Closed;

            Teams = new List<Team>();

            _random = new Random();
            _shakeTimer = new Stopwatch();
        }

        public BeepLive Run()
        {
            using Timer physicsTimer = new Timer(_ => Map.Step(), null, 1000, 1000 / 60);

            while (Window.IsOpen)
            {
                Window.DispatchEvents();
                Window.Clear(Color.Black);

                ApplyShake();
                _view.Size = new Vector2f(Window.Size.X * _zoom, Window.Size.Y * _zoom);
                _view.Rotation = _rotation;
                Window.SetView(_view);

                GameLoop();

                Window.Display();
            }

            return this;
        }

        private void ApplyShake()
        {
            if (!_shakeTimer.IsRunning) return;
            
            // ReSharper disable once PossibleLossOfFraction
            float fulfillment = _shakeTimer.ElapsedMilliseconds / _shakeDuration;
            if (fulfillment < 1f)
            {
                Vector2f direction = new Vector2f((float) (_random.NextDouble() * 2 - 1),
                    (float) (_random.NextDouble() * 2 - 1));
                direction /= MathF.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
                _view.Center = _center + direction * _shakeMagnitude * (1 - fulfillment);
            }
            else
            {
                _shakeTimer.Reset();
                _view.Center = _center;
            }
        }

        private void TriggerShake(float magnitude, long duration)
        {
            _shakeDuration += duration;
            _shakeMagnitude += magnitude;
            if (!_shakeTimer.IsRunning) _shakeTimer.Start();
        }

        private void GameLoop()
        {
            for (var chunkI = 0; chunkI < Map.Config.MapWidth; chunkI++)
            for (var chunkJ = 0; chunkJ < Map.Config.MapHeight; chunkJ++)
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

            foreach (var entity in entities.Where(entity => !entity.Alive)) Window.Draw(entity.Shape);
        }

        private static void Window_KeyPressed(object sender, KeyEventArgs e)
        {
            Window window = (Window) sender;
            if (e.Code == Keyboard.Key.Escape) window.Close();
        }

        private void Window_MousePressed(object sender, MouseButtonEventArgs e)
        {
            Vector2f position = new Vector2f(20, 20);
            Map.AddClusterProjectile(position, new Vector2f(e.X - position.X, e.Y - position.Y) / 10, 4, 10, 300, 200,
                2, 10, 5, 200);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Window.Close();
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
    }
}