using BeepLive.Client.PacketHandlers;
using BeepLive.Game;
using BeepLive.Network;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Networker.Client;
using Networker.Client.Abstractions;
using Networker.Extensions.ProtobufNet;

namespace BeepLive.Client
{
    public class BeepClient
    {
        public static BeepClient BeepClientInstance;
        public BeepLiveSfml BeepLiveSfml;

        public BeepClient()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("clientSettings.json", false, true)
                .Build();

            IConfigurationSection networkerSettings = config.GetSection("Networker");
            
            Client = new ClientBuilder()
                .UseIp(networkerSettings.GetValue<string>("Address"))
                .UseTcp(networkerSettings.GetValue<int>("TcpPort"))
                .UseUdp(networkerSettings.GetValue<int>("UdpPort"))
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.AddConfiguration(config.GetSection("Logging"));
                    loggingBuilder.AddConsole();
                })
                .UseProtobufNet()
                .RegisterPacketHandler<PlayerShotPacket, ClientPlayerShotPacketHandler>()
                .RegisterPacketHandler<PlayerJumpPacket, ClientPlayerJumpPacketHandler>()
                .RegisterPacketHandler<PlayerSpawnAtPacket, ClientPlayerSpawnAtHandler>()
                .RegisterPacketHandler<PlayerTeamJoinPacket, ClientTeamJoinPacketHandler>()
                .RegisterPacketHandler<ServerFlowPacket, ClientServerFlowPacketHandler>()
                .RegisterPacketHandler<SyncPacket, ClientSyncPacketHandler>()
                .Build();
        }

        public IClient Client { get; set; }

        public void Start()
        {
            Client.Connect();
            BeepLiveSfml = new BeepLiveSfml(new MessageSender(Client));
            BeepLiveSfml.Run();
        }

        private class MessageSender : IMessageSender
        {
            private readonly IClient _client;

            public MessageSender(IClient client)
            {
                _client = client;
            }

            public void SendMessage<T>(T message) => _client.Send(message);
        }
    }
}