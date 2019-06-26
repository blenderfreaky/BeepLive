using System.Threading.Tasks;
using BeepLive.Network;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;

#pragma warning disable 1998

namespace BeepLive.Server.PacketHandlers
{
    public class PlayerShotPacketHandler : PacketHandlerBase<PlayerShotPacket>
    {
        private readonly ILogger<PlayerShotPacketHandler> _logger;

        public PlayerShotPacketHandler(ILogger<PlayerShotPacketHandler> logger)
        {
            _logger = logger;
        }

        public override async Task Process(PlayerShotPacket packet, IPacketContext packetContext)
        {
            _logger.LogDebug("Received: " + packet);

            if (BeepServer.PlayerSecrets[packet.PlayerGuid] == packet.Secret)
            {
            }
        }
    }
}