namespace BeepLive.Server
{
    using BeepLive.Network;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;

    internal static partial class PacketHandlers
    {
        internal static void Process(PacketContext<PlayerSpawnAtPacket> packetContext)
        {
            packetContext.Logger.LogDebug("Received: " + packetContext.Packet);

            if (!packetContext.Server.IsValid(packetContext.Packet))
            {
                packetContext.Logger.LogWarning($"Received packet with invalid Secret: {packetContext.Packet}\nSent by: {packetContext.Sender}");
                return;
            }

            packetContext.Server.BroadcastWithoutSecret(packetContext.Packet);
        }
    }
}