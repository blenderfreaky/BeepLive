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

    public class BeepServer
    {
        public ILogger<BeepServer> Logger { get; }
        public List<ServerPlayer> Players { get; }
        public BeepConfig BeepConfig { get; }
        public StreamProtobuf StreamProtobuf { get; }
        public NetTcpServer GameServer { get; }

        public BeepServer()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", false, true)
                .Build();

            using var loggerFactory = LoggerFactory.Create(x => x
                .AddConfiguration(config.GetSection("Logging"))
                .AddConsole());

            Logger = loggerFactory.CreateLogger<BeepServer>();

            IConfigurationSection networkConfig = config.GetSection("Network");

            Players = new List<ServerPlayer>();

            IPAddress hostAddress = IPAddress.Parse("127.0.0.1");

            TcpListener listener = new TcpListener(hostAddress, networkConfig.GetValue<int>("TcpPort"));

            NetTcpServer server = new NetTcpServer(listener, new StreamProtobuf(PrefixStyle.Base128,
                typeof(SyncPacket),
                typeof(ServerFlowPacket),
                typeof(Packet),
                typeof(PlayerShotPacket),
                typeof(PlayerSpawnAtPacket),
                typeof(PlayerTeamJoinPacket),
                typeof(Vector2FSerializable),
                typeof(PlayerJumpPacket),
                typeof(PlayerFlowPacket),
                typeof(PlayerActionPacket)));

            server.PacketReceivedEvent += HandlePacket;

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

            _ = server.AcceptClients(
                (server, _) => server.Clients.Count < 20,
                server => server.Clients.Count < 20);

            _ = server.AcceptPackets();
        }

        public void HandlePacket(NetTcpServer server, NetTcpClient client, object packet)
        {
            switch (packet)
            {
                case PlayerShotPacket playerShotPacket:
                    PacketHandlers.ProcessPacket(CreatePacketContext(playerShotPacket, client));
                    break;
                case PlayerSpawnAtPacket playerSpawnAtPacket:
                    PacketHandlers.ProcessPacket(CreatePacketContext(playerSpawnAtPacket, client));
                    break;
                case PlayerTeamJoinPacket playerTeamJoinPacket:
                    PacketHandlers.ProcessPacket(CreatePacketContext(playerTeamJoinPacket, client));
                    break;
                case PlayerJumpPacket playerJumpPacket:
                    PacketHandlers.ProcessPacket(CreatePacketContext(playerJumpPacket, client));
                    break;
                case PlayerFlowPacket playerFlowPacket:
                    PacketHandlers.ProcessPacket(CreatePacketContext(playerFlowPacket, client));
                    break;
            }
        }

        private PacketContext<TPacket> CreatePacketContext<TPacket>(TPacket packet, NetTcpClient sender) where TPacket : Packet =>
            new PacketContext<TPacket>(packet, this, sender, Logger);

        public bool IsValid(PlayerActionPacket packet) =>
            string.Equals(
                Players.Find(p => string.Equals(
                    p.PlayerGuid,
                    packet.PlayerGuid,
                    StringComparison.Ordinal)).Secret,
                packet?.Secret,
                StringComparison.Ordinal);

        public void BroadcastWithoutSecret(PlayerActionPacket packet)
        {
            if (packet == null) throw new ArgumentNullException(nameof(packet));

            packet.Secret = null;
            GameServer.Broadcast(packet);
        }

        public bool AllPlayersInState(ServerPlayerState state, bool finished) => Players.TrueForAll(p => (p.State == state && p.Finished) || !finished);
    }
}