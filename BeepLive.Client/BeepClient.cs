using System;
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
        public BeepLiveSfml BeepLiveSfml;
        public Guid MyPlayer, MySecret;

        public BeepClient()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("clientSettings.json", false, true)
                .Build();

            var networkerSettings = config.GetSection("Networker");

            MyPlayer = new Guid();
            MySecret = new Guid();

            BeepLiveSfml = new BeepLiveSfml(new BeepLiveGame());

            Client = new ClientBuilder()
                .UseIp(networkerSettings.GetValue<string>("Address"))
                .UseTcp(networkerSettings.GetValue<int>("TcpPort"))
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.AddConfiguration(config.GetSection("Logging"));
                    loggingBuilder.AddConsole();
                })
                .UseProtobufNet()
                .RegisterPacketHandler<PlayerActionPacket, ClientPlayerActionPacketHandler>()
                .Build();
        }

        public IClient Client { get; set; }

        public void Start()
        {
            Client.Connect();

            Client.Send(new PlayerFlowPacket
            {
                PlayerGuid = MyPlayer, Secret = MySecret, MessageGuid = new Guid(),
                Type = PlayerFlowPacket.PlayerFlowType.Join
            });
        }
    }
}