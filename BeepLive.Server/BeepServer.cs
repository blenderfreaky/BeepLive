namespace BeepLive.Server
{
    using BeepLive.Client;
    using BeepLive.Client.PacketHandlers;
    using BeepLive.Config;
    using BeepLive.Net;
    using BeepLive.Network;
    using BeepLive.Server.PacketHandlers;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Networker.Extensions.ProtobufNet;
    using Networker.Server;
    using Networker.Server.Abstractions;
    using ProtoBuf;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;

    public static class BeepServer
    {
        public static List<ServerPlayer> Players;
        public static BeepConfig BeepConfig;

        static BeepServer()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", false, true)
                .Build();

            IConfigurationSection networkerSettings = config.GetSection("Networker");

            Players = new List<ServerPlayer>();
            using TcpClient client = new TcpClient("localhost", networkerSettings.GetValue<int>("TcpPort"));
            var writer = new StreamProtobufWriter(client.GetStream(), PrefixStyle.Base128,
                typeof(SyncPacket));

            const string beepConfigXml = "BeepConfig.xml";

            if (!File.Exists(beepConfigXml))
                File.WriteAllText(beepConfigXml, XmlHelper.ToXml(BeepConfig = new BeepConfig()));
            else BeepConfig = XmlHelper.LoadFromXmlString<BeepConfig>(File.ReadAllText(beepConfigXml));
        }

        public static IServer GameServer { get; set; }

        public static void Start()
        {
            GameServer.Start();

            BeepClient.BeepClientInstance = new BeepClient();
            BeepClient.BeepClientInstance.Start();

            while (GameServer.Information.IsRunning) Thread.Sleep(10000);
        }

        public static bool IsValid(PlayerActionPacket packet) =>
            string.Equals(Players.Find(p => string.Equals(p.PlayerGuid, packet.PlayerGuid)).Secret, packet.Secret);

        public static void BroadcastWithoutSecret(PlayerActionPacket packet)
        {
            packet.Secret = null;
            GameServer.Broadcast(packet);
        }

        public static bool AllPlayersInState(ServerPlayerState state, bool finished) => Players.TrueForAll(p => (p.State == state && p.Finished) || !finished);
    }

    public class ServerPlayer
    {
        public string PlayerGuid, Secret;

        public ServerPlayerState State;
        public bool Finished;

        public void MoveToState(ServerPlayerState state)
        {
            State = state;
            Finished = false;
        }

        public override string ToString()
        {
            return $"{nameof(PlayerGuid)}: {PlayerGuid}, {nameof(Secret)}: {Secret}, {nameof(State)}: {State}, {nameof(Finished)}: {Finished}";
        }
    }

    public enum ServerPlayerState
    {
        InTeamSelection,
        InSpawning,
        InPlanning,
        InSimulation
    }
}