using System.Threading.Tasks;
using BeepLive.Network;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;

#pragma warning disable 1998

namespace BeepLive.Server.PacketHandlers
{
    public class ServerPlayerSpawnAtPacketHandler : PacketHandlerBase<PlayerSpawnAtPacket>
    {
        private readonly ILogger<ServerPlayerSpawnAtPacketHandler> _logger;

        public ServerPlayerSpawnAtPacketHandler(ILogger<ServerPlayerSpawnAtPacketHandler> logger)
        {
            _logger = logger;
        }

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