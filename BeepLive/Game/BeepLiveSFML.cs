namespace BeepLive.Game
{
    using BeepLive.Config;
    using BeepLive.Entities;
    using BeepLive.Network;
    using BeepLive.World;
    using SFML.Graphics;
    using SFML.System;
    using SFML.Window;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class BeepLiveSfml
    {
        public readonly List<PlayerActionPacket> QueuedPlayerActionPackets;
        public readonly List<Packet> QueuedPackets;
        public RenderWindow Window;

        public IMessageSender MessageSender;

        public readonly Guid PlayerGuid, Secret;

        public List<TeamMock> TeamMocks;
        public List<PlayerMock> PlayerMocks;

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

            PlayerGuid = Guid.NewGuid();
            Secret = Guid.NewGuid();

            Flow(PlayerFlowPacket.FlowType.Join);

            BeepGameState = new BeepLiveGameState
            {
                Connecting = true
            };

            _font = new Font(Path.GetFullPath("BioRhymeExpanded-ExtraBold.ttf"));
            _connectingText = new Text("Connecting to Server", _font)
            {
                Position = _center
            };
            _debugText = new Text("", _font, 10)
            {
                Position = new Vector2f(10, 10)
            };

            QueuedPlayerActionPackets = new List<PlayerActionPacket>();
            QueuedPackets = new List<Packet>();
        }

        public BeepLiveGame BeepLiveGame;

        public BeepLiveGameState BeepGameState;

        private void Window_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e) => _zoom *= (float)Math.Pow(2, e.Delta);

        public void Run()
        {
            while (Window.IsOpen)
            {
                Window.DispatchEvents();

                Window.Clear(Color.Black);

                ExecutePackets();

                if (BeepGameState.Drawing)
                {
                    const float interpolation = 0.5f;
                    _center = (_center * interpolation) +
                              ((BeepLiveGame.LocalPlayer?.Position != null
                                  ? BeepLiveGame.LocalPlayer.Position + new Vector2f(
                                        BeepLiveGame.LocalPlayer.Size / 2f,
                                        BeepLiveGame.LocalPlayer.Size / 2f)
                                  : new Vector2f(Window.Size.X / 2f, Window.Size.Y / 2f)) * (1 - interpolation));
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

                _debugText.DisplayedString = $"Physics Step Rate: {BeepLiveGame?.Map.ActualFrameRate} / {60}\n" +
                                             $"Steps Queued: {BeepLiveGame?.Map.StepsQueued} Steps Finished: {BeepLiveGame?.Map.StepsFinished}";
                Window.Draw(_debugText);

                Window.Display();
            }

            _physicsTimer?.Dispose();
        }

        private void ApplyShake()
        {
            if (!_shakeTimer.IsRunning) return;

            float fulfillment = (float)(_shakeTimer.ElapsedMilliseconds / (double)_shakeDuration);
            if (fulfillment < 1f)
            {
                Vector2f direction = new Vector2f((float)((_random.NextDouble() * 2) - 1),
                    (float)((_random.NextDouble() * 2) - 1));
                direction /= direction.Magnitude();
                _view.Center = _center + (direction * _shakeMagnitude * (1f - fulfillment));
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
            if (e.Code == Keyboard.Key.Escape) ((Window)sender).Close();

            if (Window != sender) throw new InvalidOperationException();
            if (BeepLiveGame == null) return;//throw new InvalidOperationException();

            Vector2f mouse = Window.MapPixelToCoords(Mouse.GetPosition(Window));
            Vector2f direction = (mouse
                - (BeepLiveGame.LocalPlayer?.Position ?? new Vector2f())
                - new Vector2f(BeepLiveGame.LocalPlayer?.Size / 2f ?? 0,
                               BeepLiveGame.LocalPlayer?.Size / 2f ?? 0))
                / 10f;

            switch (e.Code)
            {
                case Keyboard.Key.Num1:
                    JoinTeam("Team 0", "Player 0");
                    Flow(PlayerFlowPacket.FlowType.LockInTeam);
                    break;

                case Keyboard.Key.Q:
                    Shoot(0, direction);
                    break;

                case Keyboard.Key.W:
                    Jump(direction);
                    break;

                case Keyboard.Key.Enter:
                    if (BeepGameState.Spawning) Flow(PlayerFlowPacket.FlowType.Spawn);
                    else Flow(PlayerFlowPacket.FlowType.ReadyForSimulation);
                    break;
            }
        }

        private void Window_MousePressed(object sender, MouseButtonEventArgs e)
        {
            if (BeepGameState.Spawning) Flow(PlayerFlowPacket.FlowType.FinishedSimulation);
        }

        private void Window_Closed(object sender, EventArgs e) => Window.Close();

        private void SendPlayerAction<T>(T action)
            where T : PlayerActionPacket
        {
            action.PlayerGuid = PlayerGuid;
            action.Secret = Secret;

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

        private void Flow(PlayerFlowPacket.FlowType flowType) =>
            SendPlayerAction(new PlayerFlowPacket
            {
                Type = flowType
            });

        private void JoinTeam(string teamGuid, string userName) =>
            SendPlayerAction(new PlayerTeamJoinPacket
            {
                TeamGuid = teamGuid,
                UserName = userName
            });

        private void SpawnAt(Vector2f position) =>
            SendPlayerAction(new PlayerSpawnAtPacket
            {
                Position = position
            });

        private void DrawMap()
        {
            for (int chunkI = 0; chunkI < BeepLiveGame.Map.Config.MapWidth; chunkI++)
            {
                for (int chunkJ = 0; chunkJ < BeepLiveGame.Map.Config.MapHeight; chunkJ++)
                {
                    Chunk chunk = BeepLiveGame.Map.Chunks[chunkI, chunkJ];
                    chunk.Update();
                    Window.Draw(chunk.Sprite);
                }
            }

            Entity[] entities;
            lock (BeepLiveGame.Map.Entities) entities = BeepLiveGame.Map.Entities.Where(e => !(e is null)).ToArray();

            foreach (Entity entity in entities.Where(entity => !entity.Alive)) Window.Draw(entity.Shape);
        }

        public void HandlePlayerActionPacket(PlayerActionPacket packet)
        {
            lock (QueuedPlayerActionPackets) QueuedPlayerActionPackets.Add(packet);
        }

        public void HandleServerFlowPacket(ServerFlowPacket packet)
        {
            BeepGameState.Connecting = false;

            switch (packet.Type)
            {
                case ServerFlowType.StartTeamSelection:
                    BeepGameState.SelectingTeams = true;
                    BeepGameState.Drawing = false;
                    break;

                case ServerFlowType.StartSpawning:
                    while (!BeepLiveGame.Teams.TrueForAll(t => t.Players.Count == t.TeamConfig.MaxPlayers))
                    {
                        foreach(Team team in BeepLiveGame.Teams)
                        {
                            team.Players = TeamMocks
                                .Find(tm =>
                                    string.Equals(
                                        team.TeamConfig.TeamGuid,
                                        tm.TeamGuid, StringComparison.Ordinal))
                                .Players
                                .Select(p =>
                                {
                                    Player player = new Player(
                                        BeepLiveGame.Map,
                                        new Vector2f(0, 0),
                                        team.TeamConfig.PlayerSize,
                                        team,
                                        p.PlayerGuid,
                                        p.UserName);
                                    BeepLiveGame.Map.Entities.Add(player);
                                    BeepLiveGame.Map.Players.Add(player);
                                    return player;
                                }).ToList();
                        }
                    }

                    BeepLiveGame.LocalPlayer = BeepLiveGame.Map.Players.Find(p => p.Guid == PlayerGuid);
                    BeepLiveGame.LocalTeam = BeepLiveGame.LocalPlayer.Team;

                    BeepGameState.SelectingTeams = false;
                    BeepGameState.Spawning = true;
                    BeepGameState.Drawing = true;
                    break;

                case ServerFlowType.StartSimulation:
                    ExecutePlayerActionPackets();

                    BeepGameState.InputsAllowed = false;
                    BeepGameState.Spawning = false;
                    BeepLiveGame.Map.Simulating = true;
                    BeepGameState.Simulating = true;
                    BeepGameState.Drawing = true;
                    break;

                case ServerFlowType.StartPlanning:
                    //Debug.Assert(BeepLiveGame.Map.Simulating);

                    BeepGameState.Simulating = false;
                    BeepGameState.InputsAllowed = true;
                    BeepGameState.Drawing = true;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ExecutePackets()
        {
            Packet[] packets;
            lock (QueuedPackets) packets = QueuedPackets.OrderBy(p => p.TimeSent).ToArray();

            foreach (var packet in packets)
            {
                switch (packet)
                {
                    case SyncPacket sync:
                        HandleSyncPacket(sync);
                        break;

                    case ServerFlowPacket flow:
                        HandleServerFlowPacket(flow);
                        break;

                    case PlayerTeamJoinPacket teamJoin:
                        HandleClientTeamJoinPacket(teamJoin);
                        break;

                    case PlayerSpawnAtPacket spawnAt:
                        HandlePlayerSpawnAtPacket(spawnAt);
                        break;

                    case PlayerActionPacket action:
                        HandlePlayerActionPacket(action);
                        break;

                    default:
                        throw new InvalidOperationException($"Invalid packet: {packet}");
                }
            }

            lock (QueuedPackets) QueuedPackets.Clear();
        }

        private void ExecutePlayerActionPackets()
        {
            PlayerActionPacket[] packets;
            lock (QueuedPlayerActionPackets) packets = QueuedPlayerActionPackets.OrderBy(x => x.MessageGuid).ThenBy(x => x.PlayerGuid).ToArray();

            foreach (var packet in packets) ExecutePlayerActionPacket(packet);

            lock (QueuedPlayerActionPackets) QueuedPlayerActionPackets.Clear();
        }

        private void ExecutePlayerActionPacket(PlayerActionPacket packet)
        {
            Player player = BeepLiveGame.Map.Players.Find(p => p.Guid == packet.PlayerGuid);

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
                                _ => TriggerShake(clusterShotConfig.ExplosionPower, 1000);
                            break;

                        default:
                            player.Shoot(shotConfig, shotPacket.Direction);
                            break;
                    }
                    break;

                default:
                    HandlePacket(packet);
                    break;
            }
        }

        public void HandlePlayerSpawnAtPacket(PlayerSpawnAtPacket packet)
        {
            Player player = BeepLiveGame.Map.Players.Find(p => p.Guid == packet.PlayerGuid);

            player.Position = packet.Position;
        }

        public void HandleSyncPacket(SyncPacket packet)
        {
            BeepLiveGame = new BeepLiveGame(packet.BeepConfig, PlayerGuid);

            BeepLiveGame.Map.OnSimulationStop += () => 
            Flow(PlayerFlowPacket.FlowType.FinishedSimulation);

            BeepGameState.Connecting = false;

            _physicsTimer = BeepLiveGame.Run(); // Simulating is false, so this doesn't run immediately

            TeamMocks = BeepLiveGame.Teams.Select(t => new TeamMock
            {
                TeamColor = t.TeamConfig.Color,
                MaxPlayers = t.TeamConfig.MaxPlayers,
                TeamGuid = t.TeamConfig.TeamGuid
            }).ToList();
            PlayerMocks = new List<PlayerMock>();
        }

        public void HandleClientTeamJoinPacket(PlayerTeamJoinPacket packet)
        {
            PlayerMock playerMock =
                PlayerMocks.Find(x =>
                    x.PlayerGuid
                        == packet.PlayerGuid)
                ?? new PlayerMock
                {
                    PlayerGuid = packet.PlayerGuid,
                    UserName = packet.UserName
                };

            playerMock.Team?.Players.Remove(playerMock);

            TeamMock teamMock =
                TeamMocks.Find(x =>
                    string.Equals(x.TeamGuid,
                        packet.TeamGuid, StringComparison.Ordinal));

            playerMock.Team = teamMock;
            teamMock.Players.Add(playerMock);
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

        #endregion Shake

        private readonly Font _font;
        private readonly Text _connectingText;
        private readonly Text _debugText;
        private Timer _physicsTimer;

        #endregion Camera

        public void HandlePacket(Packet packet)
        {
            if (packet is PlayerShotPacket shot) HandlePlayerActionPacket(shot);
            else lock (QueuedPackets) QueuedPackets.Add(packet);
        }
    }
}