﻿namespace BeepLive.Client
{
    using BeepLive.Game;
    using BeepLive.Net;
    using BeepLive.Network;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using ProtoBuf;
    using System.Net;
    using System.Net.Sockets;

    public class BeepClient
    {
        public BeepLiveSfml BeepLiveSfml { get; }
        public ILogger Logger { get; }

        public BeepClient()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("clientSettings.json", false, true)
                .Build();

            using var loggerFactory = LoggerFactory.Create(x => x
                .AddConfiguration(config.GetSection("Logging"))
                .AddConsole());

            Logger = loggerFactory.CreateLogger<BeepClient>();

            IConfigurationSection networkConfig = config.GetSection("Network");

            IPAddress hostAddress = IPAddress.Parse(networkConfig.GetValue<string>("Address"));
            int port = networkConfig.GetValue<int>("TcpPort");

            //TcpClient tcpClient = new TcpClient(new IPEndPoint(hostAddress, port));
            TcpClient tcpClient = new TcpClient(networkConfig.GetValue<string>("Address"), port);

            NetTcpClient client = new NetTcpClient(tcpClient, new StreamProtobuf(PrefixStyle.Base128, Packet.PacketTypes));

            client.PacketReceivedEvent += HandlePacket;

            BeepLiveSfml = new BeepLiveSfml(new MessageSender(client));

            _ = client.AcceptPackets();

            BeepLiveSfml.Run();
        }

        public void HandlePacket(NetTcpClient client, object packet)
        {
            switch (packet)
            {
                case PlayerJumpPacket _:
                case PlayerShotPacket _:
                case PlayerSpawnAtPacket _:
                case PlayerTeamJoinPacket _:
                case ServerFlowPacket _:
                case SyncPacket _:
                    BeepLiveSfml.HandlePacket((Packet)packet);
                    break;
            }
        }

        private class MessageSender : IMessageSender
        {
            private readonly NetTcpClient _client;

            public MessageSender(NetTcpClient client)
            {
                _client = client;
            }

            public void SendMessage<T>(T message) => _client.SendToStream(message);
        }
    }
}