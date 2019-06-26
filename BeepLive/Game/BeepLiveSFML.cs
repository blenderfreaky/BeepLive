using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using BeepLive.Config;
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
        public readonly List<PlayerActionPacket> QueuedPlayerActionPackets;
        public RenderWindow Window;

        public IMessageSender MessageSender;

        public readonly string PlayerGuid, Secret;

        public BeepLiveSfml(IMessageSender messageSender)
        {
            VideoMode mode = new VideoMode(800, 600);
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

            MessageSender = messageSender;

            PlayerGuid = Guid.NewGuid().ToString();
            Secret = Guid.NewGuid().ToString();

            Flow(PlayerFlowPacket.PlayerFlowType.Join);

            BeepGameState = new GameState
            {
                Connecting = true
            };

            _font = new Font(Path.GetFullPath("BioRhymeExpanded-ExtraBold.ttf"));
            _connectingText = new Text("Connecting to Server", _font)
            {
                Position = _center
            };

            QueuedPlayerActionPackets = new List<PlayerActionPacket>();
        }

        public BeepLiveGame BeepLiveGame;

        public GameState BeepGameState;

        private void Window_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e) => _zoom *= (float) Math.Pow(2, e.Delta);

        public void Run()
        {
            while (Window.IsOpen)
            {
                Window.DispatchEvents();

                Window.Clear(Color.Black);

                if (BeepGameState.Drawing)
                {
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

                    DrawMap();
                }

                if (BeepGameState.SelectingTeams)
                {
                    //Window.Draw();
                }

                if (BeepGameState.Spawning)
                {
                    Vector2f mouse = Window.MapPixelToCoords(Mouse.GetPosition(Window));
                    Vector2f position = mouse + new Vector2f(BeepLiveGame.LocalTeam.TeamConfig.PlayerSize / 2f, BeepLiveGame.LocalTeam.TeamConfig.PlayerSize / 2f);
                    SpawnAt(position);
                }

                if (BeepGameState.Connecting) Window.Draw(_connectingText);

                Window.Display();
            }

            _physicsTimer?.Dispose();
        }

        private void ApplyShake()
        {
            if (!_shakeTimer.IsRunning) return;

            // ReSharper disable once PossibleLossOfFraction
            float fulfillment = (float) (_shakeTimer.ElapsedMilliseconds / (double) _shakeDuration);
            if (fulfillment < 1f)
            {
                Vector2f direction = new Vector2f((float) (_random.NextDouble() * 2 - 1),
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

        [SuppressMessage("ReSharper", "SwitchStatementMissingSomeCases")]
        private void Window_KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape) ((Window)sender).Close();

            if (Window != sender) throw new InvalidOperationException();
            if (BeepLiveGame == null) return;//throw new InvalidOperationException();

            Vector2f mouse = Window.MapPixelToCoords(Mouse.GetPosition(Window));
            Vector2f direction = mouse - (BeepLiveGame.LocalPlayer?.Position ?? new Vector2f()) - new Vector2f(BeepLiveGame.LocalPlayer?.Size/2f ?? 0, BeepLiveGame.LocalPlayer?.Size/2f ?? 0);

            switch (e.Code)
            {
                case Keyboard.Key.Num1:
                    JoinTeam(0);
                    break;
                case Keyboard.Key.Q:
                    Shoot(0, direction);
                    break;
                case Keyboard.Key.W:
                    Jump(direction);
                    break;
                case Keyboard.Key.Enter:
                    Flow(PlayerFlowPacket.PlayerFlowType.ReadyForSimulation);
                    break;
            }
        }

        private void Window_MousePressed(object sender, MouseButtonEventArgs e)
        {
            if (BeepGameState.Spawning) Flow(PlayerFlowPacket.PlayerFlowType.FinishedSimulation);
        }

        private void Window_Closed(object sender, EventArgs e) => Window.Close();

        private void SendPlayerAction<T>(T action)
            where T : PlayerActionPacket
        {
            action.PlayerGuid = PlayerGuid;
            action.Secret = Secret;
            action.MessageGuid = Guid.NewGuid().ToString();

            MessageSender.SendMessage(action);
        }

        private void Shoot(int shotConfigIndex, Vector2f direction) =>
            SendPlayerAction(new PlayerShotPacket
            {
                ShotConfigId = shotConfigIndex,
                Direction = direction
            });

        private void Jump(Vector2f direction) =>
            SendPlayerAction(new PlayerJumpPacket
            {
                Direction = direction
            });

        private void Flow(PlayerFlowPacket.PlayerFlowType flowType) =>
            SendPlayerAction(new PlayerFlowPacket
            {
                Type = flowType
            });

        private void JoinTeam(int teamIndex) => SendPlayerAction(new PlayerTeamJoinPacket
        {
            TeamIndex = teamIndex,
        });

        private void SpawnAt(Vector2f position) => SendPlayerAction(new PlayerSpawnAtPacket
        {
            Position = position
        });

        private void DrawMap()
        {
            for (int chunkI = 0; chunkI < BeepLiveGame.Map.Config.MapWidth; chunkI++)
            for (int chunkJ = 0; chunkJ < BeepLiveGame.Map.Config.MapHeight; chunkJ++)
            {
                Chunk chunk = BeepLiveGame.Map.Chunks[chunkI, chunkJ];
                chunk.Update();
                Window.Draw(chunk.Sprite);
            }

            Entity[] entities;
            lock (BeepLiveGame.Map.Entities) entities = BeepLiveGame.Map.Entities.Where(e => !(e is null)).ToArray();

            foreach (Entity entity in entities.Where(entity => !entity.Alive)) Window.Draw(entity.Shape);
        }

        public void HandlePlayerActionPacket(PlayerActionPacket packet) => QueuedPlayerActionPackets.Add(packet);

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

                    BeepLiveGame.Map.Simulating = true;
                    BeepGameState.Simulating = true;
                    BeepGameState.Drawing = true;
                    break;
                case ServerFlowType.StopSimulation:
                    Debug.Assert(!BeepLiveGame.Map.Simulating);

                    BeepGameState.Simulating = false;
                    BeepGameState.Drawing = true;
                    break;
                case ServerFlowType.StartPlanning:
                    Debug.Assert(!BeepLiveGame.Map.Simulating);

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
            QueuedPlayerActionPackets.OrderBy(x => x.MessageGuid).ThenBy(x => x.PlayerGuid)
                .ForEach(ExecutePlayerActionPacket);

        private void ExecutePlayerActionPacket(PlayerActionPacket packet)
        {
            Player player = BeepLiveGame.Map.Players.Find(p => string.Equals(p.Guid, packet.PlayerGuid));

            switch (packet)
            {
                case PlayerJumpPacket jumpPacket:
                    player.Velocity += jumpPacket.Direction;
                    break;
                case PlayerShotPacket shotPacket:
                    var shotConfig = BeepLiveGame.BeepConfig.ShotConfigs[shotPacket.ShotConfigId];
                    switch (shotConfig)
                    {
                        case ClusterShotConfig clusterShotConfig:
                            player.Shoot(clusterShotConfig, shotPacket.Direction).OnExplodeEvent +=
                                p => TriggerShake(clusterShotConfig.ExplosionPower, 1000);
                            break;
                        default:
                            player.Shoot(shotConfig, shotPacket.Direction);
                            break;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void HandlePlayerSpawnAtPacket(PlayerSpawnAtPacket packet)
        {
            Player player = BeepLiveGame.Map.Players.Find(p => string.Equals(p.Guid, packet.PlayerGuid));

            player.Position = packet.Position;
        }

        public void HandleSyncPacket(SyncPacket packet)
        {
            BeepLiveGame = new BeepLiveGame(packet.BeepConfig, PlayerGuid) 
            {
                Map = {OnSimulationStop = () => Flow(PlayerFlowPacket.PlayerFlowType.FinishedSimulation)}
            };

            BeepGameState.Connecting = false;

            _physicsTimer = BeepLiveGame.Run();
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
        private readonly Text _connectingText;
        private Timer _physicsTimer;

        #endregion

        #endregion
    }
}