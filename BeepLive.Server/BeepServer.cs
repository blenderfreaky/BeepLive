using System.Net;
using System.Net.Sockets;
using System;
namespace BeepLive.Server
{
    using BeepLive.Client;
    using BeepLive.Config;
    using BeepLive.Net;
    using BeepLive.Network;
    using Microsoft.Extensions.Configuration;
    using Networker.Server.Abstractions;
    using ProtoBuf;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;
    using System;
    using Microsoft.Extensions.Logging;
    using System.Net;

    public class BeepServer : IDisposable
    {
        private readonly ILogger<BeepServer> _logger;
        public List<ServerPlayer> Players;
        public BeepConfig BeepConfig;
        public StreamProtobuf StreamProtobuf;

        public BeepServer()
        {
            IPAddress hostAddress = IPAddress.Parse("127.0.0.1");

            _logger = LoggerFactory.Create(x => x.AddConsole()).CreateLogger<BeepServer>();

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", false, true)
                .Build();

            IConfigurationSection networkerSettings = config.GetSection("Networker");

            Players = new List<ServerPlayer>();

            TcpListener listener = new Tcpl;
            
            using TcpClient client = new TcpClient("localhost", networkerSettings.GetValue<int>("TcpPort"));
            _disposable = client.GetStream();
            StreamProtobuf = new StreamProtobuf(_disposable, PrefixStyle.Base128,
                typeof(SyncPacket),
                typeof(ServerFlowPacket),
                typeof(Packet),
                typeof(PlayerShotPacket),
                typeof(PlayerSpawnAtPacket),
                typeof(PlayerTeamJoinPacket),
                typeof(Vector2FSerializable),
                typeof(PlayerJumpPacket),
                typeof(PlayerFlowPacket),
                typeof(PlayerActionPacket));
            
            const string beepConfigXml = "BeepConfig.xml";

            if (!File.Exists(beepConfigXml))
            {
                BeepConfig = new BeepConfig();
                File.WriteAllText(beepConfigXml, XmlHelper.ToXml(BeepConfig));
            }
            else
            {
                BeepConfig = XmlHelper.LoadFromXmlString<BeepConfig>(File.ReadAllText(beepConfigXml));
            }
        }

        public static void Start()
        {
            BeepClient.BeepClientInstance = new BeepClient();
            BeepClient.BeepClientInstance.Start();
        }

        public void HandlePacket(object packet)
        {
            switch (packet)
            {
                case SyncPacket syncPacket:
                case ServerFlowPacket serverFlowPacket:
                case PlayerShotPacket playerShotPacket:
                case PlayerSpawnAtPacket playerSpawnAtPacket:
                case PlayerTeamJoinPacket playerTeamJoinPacket:
                case PlayerJumpPacket playerJumpPacket:
                case PlayerFlowPacket playerFlowPacket:
                    break;
            }
        }

        public bool IsValid(PlayerActionPacket packet) =>
            string.Equals(Players.Find(p => string.Equals(p.PlayerGuid, packet.PlayerGuid)).Secret, packet.Secret);

        public void BroadcastWithoutSecret(PlayerActionPacket packet)
        {
            packet.Secret = null;
            GameServer.Broadcast(packet);
        }

        public static bool AllPlayersInState(ServerPlayerState state, bool finished) => Players.TrueForAll(p => (p.State == state && p.Finished) || !finished);

        private NetworkStream _disposable;

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }

    public class ServerPlayer
    {
        public string PlayerGuid, Secret;

        public ServerPlayerState State;
        public bool Finished;

        public void MoveToState(ServerPlayerState state)
        {
            State = state;
            Finished = false;
        }

        public override string ToString()
        {
            return $"{nameof(PlayerGuid)}: {PlayerGuid}, {nameof(Secret)}: {Secret}, {nameof(State)}: {State}, {nameof(Finished)}: {Finished}";
        }
    }

    public enum ServerPlayerState
    {
        InTeamSelection,
        InSpawning,
        InPlanning,
        InSimulation
    }
}