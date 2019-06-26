using System.Threading.Tasks;
using BeepLive.Network;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;

#pragma warning disable 1998

namespace BeepLive.Server.PacketHandlers
{
    public class PlayerJumpPacketHandler : PacketHandlerBase<PlayerJumpPacket>
    {
        private readonly ILogger<PlayerJumpPacketHandler> _logger;

        public PlayerJumpPacketHandler(ILogger<PlayerJumpPacketHandler> logger)
        {
            _logger = logger;
        }

        public override async Task Process(PlayerJumpPacket packet, IPacketContext packetContext)
        {
            _logger.LogDebug("Received: " + packet);

            if (BeepServer.PlayerSecrets[packet.PlayerGuid] == packet.Secret)
            {
            }
        }
    }
}