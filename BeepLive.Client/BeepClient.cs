using System;
using BeepLive.Config;
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
        public static BeepConfig BeepConfig;
        public BeepLiveSfml BeepLiveSfml;
        public Guid MyPlayer, MySecret;

        public BeepClient()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("clientSettings.json", false, true)
                .Build();

            IConfigurationSection networkerSettings = config.GetSection("Networker");

            MyPlayer = Guid.NewGuid();
            MySecret = Guid.NewGuid();

            Client = new ClientBuilder()
                .UseIp(networkerSettings.GetValue<string>("Address"))
                .UseTcp(networkerSettings.GetValue<int>("TcpPort"))
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.AddConfiguration(config.GetSection("Logging"));
                    loggingBuilder.AddConsole();
                })
                .UseProtobufNet()
                .RegisterPacketHandler<PlayerShotPacket, ClientPlayerShotPacketHandler>()
                .RegisterPacketHandler<PlayerJumpPacket, ClientPlayerJumpPacketHandler>()
                .RegisterPacketHandler<ServerFlowPacket, ClientServerFlowPacketHandler>()
                .RegisterPacketHandler<SyncPacket, ClientSyncPacketHandler>()
                .Build();
        }

        public IClient Client { get; set; }

        public void Start()
        {
            Client.Connect();

            var playerFlowPacket = new PlayerFlowPacket
            {
                PlayerGuid = MyPlayer.ToString(), Secret = MySecret.ToString(), MessageGuid = Guid.NewGuid().ToString(),
                Type = PlayerFlowPacket.PlayerFlowType.Join
            };
            Client.Send(playerFlowPacket);

            while (BeepConfig == null)
            {
            }

            BeepLiveSfml = new BeepLiveSfml(new BeepLiveGame(BeepConfig)) {BeepGameState = {Drawing = true}};
            BeepLiveSfml.Run();
        }
    }
}