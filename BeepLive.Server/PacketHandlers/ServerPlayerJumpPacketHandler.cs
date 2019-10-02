namespace BeepLive.Server
{
    using BeepLive.Network;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;

    public static partial class PacketHandlers
    {
        public static [void Process(PacketContext<PlayerJumpPacket> packetContext)
        {
            packetContext.Logger.LogDebug("Received: " + packetContext.Packet);

            if (!packetContext.Server.IsValid(packetContext.Packet))
            {
                packetContext.Logger.LogWarning($"Received packet with invalid Secret: {packetContext.Packet}\nSent by: {packetContext.Sender}");
            }
        }
    }
}