using BeepLive.Network;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;
using System.Threading.Tasks;

#pragma warning disable 1998

namespace BeepLive.Server.PacketHandlers
{
    public class ServerPlayerTeamJoinPacketHandler : PacketHandlerBase<PlayerTeamJoinPacket>
    {
        private readonly ILogger<ServerPlayerTeamJoinPacketHandler> _logger;

        public ServerPlayerTeamJoinPacketHandler(ILogger<ServerPlayerTeamJoinPacketHandler> logger)
        {
            _logger = logger;
        }

        public override async Task Process(PlayerTeamJoinPacket packet, IPacketContext packetContext)
        {
            _logger.LogDebug("Received: " + packet);

            if (!BeepServer.IsValid(packet))
            {
                _logger.LogWarning($"Received packet with invalid Secret: {packet}\nSent by: {packetContext.Sender.EndPoint}");
                BeepServer.BroadcastWithoutSecret(packet);
            }
        }
    }
}