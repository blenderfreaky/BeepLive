#pragma warning disable 1998

namespace BeepLive.Server.PacketHandlers
{
    using BeepLive.Network;
    using Microsoft.Extensions.Logging;
    using Networker.Common;
    using Networker.Common.Abstractions;
    using System.Threading.Tasks;

    public class ServerPlayerShotPacketHandler : PacketHandlerBase<PlayerShotPacket>
    {
        private readonly ILogger<ServerPlayerShotPacketHandler> _logger;

        public ServerPlayerShotPacketHandler(ILogger<ServerPlayerShotPacketHandler> logger)
        {
            _logger = logger;
        }

        public override async Task Process(PlayerShotPacket packet, IPacketContext packetContext)
        {
            _logger.LogDebug("Received: " + packet);

            if (!BeepServer.IsValid(packet))
            {
                _logger.LogWarning($"Received packet with invalid Secret: {packet}\nSent by: {packetContext.Sender.EndPoint}");
            }
        }
    }
}