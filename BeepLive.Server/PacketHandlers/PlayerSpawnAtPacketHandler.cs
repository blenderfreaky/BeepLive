using System.Threading.Tasks;
using BeepLive.Network;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;

#pragma warning disable 1998

namespace BeepLive.Server.PacketHandlers
{
    public class PlayerSpawnAtPacketHandler : PacketHandlerBase<PlayerSpawnAtPacket>
    {
        private readonly ILogger<PlayerSpawnAtPacketHandler> _logger;

        public PlayerSpawnAtPacketHandler(ILogger<PlayerSpawnAtPacketHandler> logger)
        {
            _logger = logger;
        }

        public override async Task Process(PlayerSpawnAtPacket packet, IPacketContext packetContext)
        {
            _logger.LogDebug("Received: " + packet);

            if (BeepServer.PlayerSecrets[packet.PlayerGuid] == packet.Secret)
            {
            }
        }
    }
}