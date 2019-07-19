using BeepLive.Network;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;
using System.Threading.Tasks;

#pragma warning disable 1998

namespace BeepLive.Server.PacketHandlers
{
    public class ServerPlayerJumpPacketHandler : PacketHandlerBase<PlayerJumpPacket>
    {
        private readonly ILogger<ServerPlayerJumpPacketHandler> _logger;

        public ServerPlayerJumpPacketHandler(ILogger<ServerPlayerJumpPacketHandler> logger)
        {
            _logger = logger;
        }

        public override async Task Process(PlayerJumpPacket packet, IPacketContext packetContext)
        {
            _logger.LogDebug("Received: " + packet);

            if (!BeepServer.IsValid(packet))
            {
                _logger.LogWarning($"Received packet with invalid Secret: {packet}\nSent by: {packetContext.Sender.EndPoint}");
            }
        }
    }
}