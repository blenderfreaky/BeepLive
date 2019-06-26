using System.Threading.Tasks;
using BeepLive.Network;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;
#pragma warning disable 1998

namespace BeepLive.Client
{
    public class ClientPlayerJumpPacketHandler : PacketHandlerBase<PlayerJumpPacket>
    {
        private readonly ILogger<ClientPlayerJumpPacketHandler> _logger;

        public ClientPlayerJumpPacketHandler(ILogger<ClientPlayerJumpPacketHandler> logger)
        {
            _logger = logger;
        }

        public override async Task Process(PlayerJumpPacket packet, IPacketContext packetContext)
        {
            _logger.LogDebug("Received: " + packet);
            BeepClient.BeepClientInstance.BeepLiveSfml.HandlePlayerActionPacket(packet);
        }
    }
}