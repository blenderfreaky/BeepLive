using System.Threading.Tasks;
using BeepLive.Network;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;
#pragma warning disable 1998

namespace BeepLive.Server
{
    public class PlayerTeamJoinPacketHandler : PacketHandlerBase<PlayerTeamJoinPacket>
    {
        private readonly ILogger<PlayerTeamJoinPacketHandler> _logger;

        public PlayerTeamJoinPacketHandler(ILogger<PlayerTeamJoinPacketHandler> logger)
        {
            _logger = logger;
        }

        public override async Task Process(PlayerTeamJoinPacket packet, IPacketContext packetContext)
        {
            _logger.LogDebug("Received: " + packet);

            if (BeepServer.PlayerSecrets[packet.PlayerGuid] == packet.Secret)
            {
            }
        }
    }
}