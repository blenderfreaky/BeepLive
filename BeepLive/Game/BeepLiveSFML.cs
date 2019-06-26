using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

            _center = new Vector2f(Window.Size.X / 2f, Window.Size.Y / 2f);
            _zoom = 1;
            _rotation = 0;

            _view = new View(_center, new Vector2f(Window.Size.X, Window.Size.Y));
            Window.SetView(_view);

            Window.KeyPressed += Window_KeyPressed;
            Window.MouseButtonPressed += Window_MousePressed;
            Window.Closed += Window_Closed;
            Window.MouseWheelScrolled += Window_MouseWheelScrolled;

            _random = new Random();
            _shakeTimer = new Stopwatch();
            BeepLiveGame = beepLive;

            BeepGameState = new GameState();

            _font = new Font(Path.GetFullPath("BioRhymeExpanded-ExtraBold.ttf"));
            _text = new Text("Connecting to Server", _font)
            {
                Position = _center
            };

            QueuedPlayerActionPackets = new List<PlayerActionPacket>();
        }

        public BeepLiveGame BeepLiveGame { get; }

        public GameState BeepGameState { get; set; }

        private void Window_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            _zoom *= (float)Math.Pow(2, e.Delta);
        }

        public BeepLiveSfml Run()
        {
            using (BeepLiveGame.Run())
            {
                while (Window.IsOpen)
                {
                    Window.DispatchEvents();

                    if (BeepGameState.Drawing)
                    {
                        Window.Clear(Color.Black);

                        const float interpolation = 0.5f;
                        _center = _center * interpolation +
                                  (BeepLiveGame.LocalPlayer?.Position != null
                                      ? BeepLiveGame.LocalPlayer.Position + new Vector2f(
                                            BeepLiveGame.LocalPlayer.Size / 2f,
                                            BeepLiveGame.LocalPlayer.Size / 2f)
                                      : new Vector2f(Window.Size.X / 2f, Window.Size.Y / 2f)) * (1 - interpolation);
                        ApplyShake();
                        _view.Size = new Vector2f(Window.Size.X * _zoom, Window.Size.Y * _zoom);
                        _view.Rotation = _rotation;
                        Window.SetView(_view);

                        GameLoop();
                    }

                    if (BeepGameState.SelectingTeams)
                    {
                        //Window.Draw();
                    }

                    if (BeepGameState.Connecting) Window.Draw(_text);

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

            switch (e.Code)
            {
                case Keyboard.Key.Q:
                    Shoot();
                    break;
                default:
                    break;
            }
        }

        private void Shoot()
        {
            
        }

        private void Window_MousePressed(object sender, MouseButtonEventArgs e)
        {
            if (BeepGameState.Spawning)
            {
            }
            else if (BeepGameState.InputsAllowed)
            {
                Vector2f localPlayerPosition = BeepLiveGame.LocalPlayer.Position;
            }
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
                Chunk chunk = BeepLiveGame.Map.Chunks[chunkI, chunkJ];
                chunk.Update();
                Window.Draw(chunk.Sprite);
            }

            Entity[] entities;
            lock (BeepLiveGame.Map.Entities)
            {
                entities = BeepLiveGame.Map.Entities.Where(e => !(e is null)).ToArray();
            }

            foreach (Entity entity in entities.Where(entity => !entity.Alive)) Window.Draw(entity.Shape);
        }



        public void HandlePlayerActionPacket(PlayerActionPacket packet)
        {
            QueuedPlayerActionPackets.Add(packet);
        }

        public readonly List<PlayerActionPacket> QueuedPlayerActionPackets;
        public void HandleServerFlowPacket(ServerFlowPacket packet)
        {
            BeepGameState.Connecting = false;

            switch (packet.Type)
            {
                case ServerFlowType.StartTeamSelection:
                    BeepGameState.SelectingTeams = true;
                    BeepGameState.Drawing = false;
                    break;
                case ServerFlowType.StopTeamSelection:
                    BeepGameState.SelectingTeams = false;
                    BeepGameState.Drawing = false;
                    break;
                case ServerFlowType.StartSpawning:
                    BeepGameState.Spawning = true;
                    BeepGameState.Drawing = true;
                    break;
                case ServerFlowType.StopSpawning:
                    BeepGameState.Spawning = false;
                    BeepGameState.Drawing = true;
                    break;
                case ServerFlowType.StartSimulation:
                    ExecutePlayerActionPackets();

                    BeepGameState.Simulating = true;
                    BeepGameState.Drawing = true;
                    break;
                case ServerFlowType.StopSimulation:
                    BeepGameState.Simulating = false;
                    BeepGameState.Drawing = true;
                    break;
                case ServerFlowType.StartPlanning:
                    BeepGameState.InputsAllowed = true;
                    BeepGameState.Drawing = true;
                    break;
                case ServerFlowType.StopPlanning:
                    BeepGameState.InputsAllowed = false;
                    BeepGameState.Drawing = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ExecutePlayerActionPackets() =>
            QueuedPlayerActionPackets.OrderBy(x => x.MessageGuid).ThenBy(x => x.PlayerGuid).ForEach(ExecutePlayerActionPacket);

        private void ExecutePlayerActionPacket(PlayerActionPacket packet)
        {
            Player player = BeepLiveGame.Map.Players.Find(p => string.Equals(p.Guid, packet.PlayerGuid));

            switch (packet)
            {
                case PlayerJumpPacket jumpPacket:
                    player.Velocity += jumpPacket.Direction;
                    break;
                case PlayerShotPacket shotPacket:
                    //player.Shoot(shotPacket.);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void HandleSyncPacket(SyncPacket packet)
        {

        }

        public class GameState
        {
            public bool Connecting;
            public bool Drawing;
            public bool InputsAllowed;
            public bool SelectingTeams;
            public bool Simulating;
            public bool Spawning;
        }

        #region Camera

        private readonly View _view;
        private Vector2f _center;
        private float _zoom = 1;
        private readonly float _rotation;

        #region Shake

        private readonly Stopwatch _shakeTimer;
        private long _shakeDuration; // In milliseconds
        private float _shakeMagnitude;
        private readonly Random _random;
        private readonly Font _font;
        private readonly Text _text;

        #endregion

        #endregion
    }
}