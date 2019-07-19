using System.Threading.Tasks;
using BeepLive.Network;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;

#pragma warning disable 1998

namespace BeepLive.Client.PacketHandlers
{
    public class ClientPlayerShotPacketHandler : PacketHandlerBase<PlayerShotPacket>
    {
        private readonly ILogger<ClientPlayerShotPacketHandler> _logger;

        public ClientPlayerShotPacketHandler(ILogger<ClientPlayerShotPacketHandler> logger)
        {
            _logger = logger;
        }

        public override async Task Process(PlayerShotPacket packet, IPacketContext packetContext)
        {
            _logger.LogDebug("Received: " + packet);
            BeepClient.BeepClientInstance.BeepLiveSfml.HandlePacket(packet);
        }
    }
}