namespace BeepLive.Server
{
    using BeepLive.Network;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;

    public static partial class PacketHandlers
            public override async Task Process(PlayerSpawnAtPacket packet, IPacketContext packetContext)
    {
        _logger.LogDebug("Received: " + packet);

        if (!BeepServer.IsValid(packet))
        {
            _logger.LogWarning($"Received packet with invalid Secret: {packet}\nSent by: {packetContext.Sender.EndPoint}");
        }
    }
}
}