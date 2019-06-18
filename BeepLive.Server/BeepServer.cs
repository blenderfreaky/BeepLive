using System;
using System.Collections.Generic;
using System.Threading;
using BeepLive.Client;
using BeepLive.Network;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Networker.Extensions.ProtobufNet;
using Networker.Server;
using Networker.Server.Abstractions;

namespace BeepLive.Server
{
    public class BeepServer
    {
        public Dictionary<Guid, Guid> PlayerSecrets;

        public BeepServer()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", false, true)
                .Build();

            IConfigurationSection networkerSettings = config.GetSection("Networker");

            PlayerSecrets = new Dictionary<Guid, Guid>();

            Server = new ServerBuilder()
                .UseTcp(networkerSettings.GetValue<int>("TcpPort"))
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.AddConfiguration(config.GetSection("Logging"));
                    loggingBuilder.AddConsole();
                })
                .UseProtobufNet()
                .RegisterPacketHandler<PlayerShotPacket, PlayerShotPacketHandler>()
                .RegisterPacketHandler<PlayerJumpPacket, PlayerJumpPacketHandler>()
                .RegisterPacketHandler<PlayerFlowPacket, PlayerFlowPacketHandler>()
                .Build();
        }

        public IServer Server { get; set; }

        public void Start()
        {
            Server.Start();

            var client = new BeepClient();
            //client.BeepLiveSfml.BeepGameState.InputsAllowed = true;
            client.Start();

            while (Server.Information.IsRunning) Thread.Sleep(10000);
        }
    }
}