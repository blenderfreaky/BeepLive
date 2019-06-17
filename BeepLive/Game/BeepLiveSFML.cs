using System;
using System.Diagnostics;
using System.Linq;
using BeepLive.Entities;
using BeepLive.Network;
using BeepLive.World;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace BeepLive.Game
{
    public class BeepLiveSfml
    {
        public Action<PlayerActionPacket> SendAction;
        public RenderWindow Window;

        public BeepLiveSfml(BeepLiveGame beepLive)
        {
            var mode = new VideoMode(800, 600);
            Window = new RenderWindow(mode, "Map");

            _center = new Vector2f();
            _zoom = 1;
            _rotation = 0;

            _view = new View(_center, new Vector2f(Window.Size.X, Window.Size.Y));
            Window.SetView(_view);

            Window.KeyPressed += Window_KeyPressed;
            Window.MouseButtonPressed += Window_MousePressed;
            Window.Closed += Window_Closed;

            _random = new Random();
            _shakeTimer = new Stopwatch();
            BeepLiveGame = beepLive;
        }

        public BeepLiveGame BeepLiveGame { get; }

        public BeepLiveSfml Run()
        {
            using (BeepLiveGame.Run())
            {
                while (Window.IsOpen)
                {
                    Window.DispatchEvents();
                    Window.Clear(Color.Black);

                    const float interpolation = 0.5f;
                    _center = _center * interpolation +
                              (BeepLiveGame.LocalPlayer.Position + new Vector2f(BeepLiveGame.LocalPlayer.Size / 2f,
                                   BeepLiveGame.LocalPlayer.Size / 2f)) * (1 - interpolation);
                    ApplyShake();
                    _view.Size = new Vector2f(Window.Size.X * _zoom, Window.Size.Y * _zoom);
                    _view.Rotation = _rotation;
                    Window.SetView(_view);

                    GameLoop();

                    Window.Display();
                }
            }

            return this;
        }

        private void ApplyShake()
        {
            if (!_shakeTimer.IsRunning) return;

            // ReSharper disable once PossibleLossOfFraction
            float fulfillment = (float) (_shakeTimer.ElapsedMilliseconds / (double) _shakeDuration);
            if (fulfillment < 1f)
            {
                var direction = new Vector2f((float) (_random.NextDouble() * 2 - 1),
                    (float) (_random.NextDouble() * 2 - 1));
                direction /= MathF.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
                _view.Center = _center + direction * _shakeMagnitude * (1f - fulfillment);
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

        private void Window_KeyPressed(object sender, KeyEventArgs e)
        {
            var window = (Window) sender;
            if (e.Code == Keyboard.Key.Escape) window.Close();
            //TriggerShake(10f, 1000);
        }

        private void Window_MousePressed(object sender, MouseButtonEventArgs e)
        {
            var position = new Vector2f(20, 20);
            BeepLiveGame.Map.AddClusterProjectile(position, new Vector2f(e.X - position.X, e.Y - position.Y) / 10, 4,
                10, 300, 200,
                2, 10, 5, 200);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Window.Close();
        }

        private void GameLoop()
        {
            for (int chunkI = 0; chunkI < BeepLiveGame.Map.Config.MapWidth; chunkI++)
            for (int chunkJ = 0; chunkJ < BeepLiveGame.Map.Config.MapHeight; chunkJ++)
            {
                var chunk = BeepLiveGame.Map.Chunks[chunkI, chunkJ];
                chunk.Update();
                Window.Draw(chunk.Sprite);
            }

            Entity[] entities;
            lock (BeepLiveGame.Map.Entities)
            {
                entities = BeepLiveGame.Map.Entities.Where(e => !(e is null)).ToArray();
            }

            foreach (var entity in entities.Where(entity => !entity.Alive)) Window.Draw(entity.Shape);
        }

        #region Camera

        private readonly View _view;
        private Vector2f _center;
        private readonly float _zoom = 1;
        private readonly float _rotation;

        #region Shake

        private readonly Stopwatch _shakeTimer;
        private long _shakeDuration; // In milliseconds
        private float _shakeMagnitude;
        private readonly Random _random;

        #endregion

        #endregion

        #region Fluent API

        public BeepLiveSfml AddMap(Func<Map, Map> mapMaker)
        {
            BeepLiveGame.Map = mapMaker(new Map());

            return this;
        }

        public BeepLiveSfml AddTeam(Func<Team, Team> teamMaker)
        {
            BeepLiveGame.Teams.Add(teamMaker(new Team(BeepLiveGame)));

            return this;
        }

        public BeepLiveSfml SetLocalPlayer(Player localPlayer)
        {
            BeepLiveGame.LocalPlayer = localPlayer;

            return this;
        }

        #endregion
    }
}