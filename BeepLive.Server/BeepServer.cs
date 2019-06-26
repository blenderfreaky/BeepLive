using System.Collections.Generic;
using System.IO;
using System.Threading;
using BeepLive.Client;
using BeepLive.Config;
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
        public static BeepConfig BeepConfig;

        static BeepServer()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", false, true)
                .Build();

            var networkerSettings = config.GetSection("Networker");

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

            const string beepConfigXml = "BeepConfig.xml";

            if (!File.Exists(beepConfigXml))
                File.WriteAllText(beepConfigXml, XmlHelper.ToXml(BeepConfig = new BeepConfig()));
            else BeepConfig = XmlHelper.LoadFromXmlString<BeepConfig>(File.ReadAllText(beepConfigXml));
        }

        public static IServer Server { get; set; }

        public static void Start()
        {
            Server.Start();

            BeepClient.BeepClientInstance = new BeepClient();
            BeepClient.BeepClientInstance.Start();

            while (Server.Information.IsRunning) Thread.Sleep(10000);
        }
    }
}