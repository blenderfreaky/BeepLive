﻿namespace BeepLive.Server
{
    using BeepLive.Config;
    using BeepLive.Net;
    using BeepLive.Network;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using ProtoBuf;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;

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

            TcpListener tcpListener = new TcpListener(hostAddress, networkConfig.GetValue<int>("TcpPort"));

            GameServer = new NetTcpServer(tcpListener, new StreamProtobuf(PrefixStyle.Base128, Packet.PacketTypes));

            GameServer.PacketReceivedEvent += HandlePacket;

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

            tcpListener.Start();

            _ = GameServer.AcceptClients(
                (server, _) => server.Clients.Count < 20,
                server => server.Clients.Count < 20);

            _ = GameServer.AcceptPackets();
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
            Players.Find(p =>
                p.PlayerGuid
                == packet.PlayerGuid).Secret
            == packet?.Secret;

        public void BroadcastWithoutSecret(PlayerActionPacket packet)
        {
            if (packet == null) throw new ArgumentNullException(nameof(packet));

            packet.Secret = default;
            GameServer.Broadcast(packet);
        }

        public bool AllPlayersInState(ServerPlayerState state, bool finished) => Players.TrueForAll(p => p.State == state && (p.Finished || !finished));
    }
}