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
        public BeepClient()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("clientSettings.json", false, true)
                .Build();

            var networkerSettings = config.GetSection("Networker");

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
    }
}