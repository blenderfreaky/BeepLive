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
    public static class BeepServer
    {
        public static Dictionary<string, string> PlayerSecrets;

        static BeepServer()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", false, true)
                .Build();

            IConfigurationSection networkerSettings = config.GetSection("Networker");

            PlayerSecrets = new Dictionary<string, string>();

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

        public static IServer Server { get; set; }

        public static void Start()
        {
            Server.Start();

            var client = new BeepClient();
            //client.BeepLiveSfml.BeepGameState.InputsAllowed = true;
            client.Start();

            while (Server.Information.IsRunning) Thread.Sleep(10000);
        }
    }
}