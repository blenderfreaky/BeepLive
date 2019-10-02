namespace BeepLive.Server
{
    using BeepLive.Network;
    using Microsoft.Extensions.Logging;

    internal static partial class PacketHandlers
    {
        internal static void ProcessPacket(PacketContext<PlayerJumpPacket> packetContext)
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