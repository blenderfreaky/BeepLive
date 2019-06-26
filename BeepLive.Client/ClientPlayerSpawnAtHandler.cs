using System.Threading.Tasks;
using BeepLive.Network;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;
#pragma warning disable 1998

namespace BeepLive.Client
{
    public class ClientPlayerSpawnAtHandler : PacketHandlerBase<PlayerSpawnAtPacket>
    {
        private readonly ILogger<ClientPlayerSpawnAtHandler> _logger;

        public ClientPlayerSpawnAtHandler(ILogger<ClientPlayerSpawnAtHandler> logger)
        {
            _logger = logger;
        }

        public override async Task Process(PlayerSpawnAtPacket packet, IPacketContext packetContext)
        {
            _logger.LogDebug("Received: " + packet);
            BeepClient.BeepClientInstance.BeepLiveSfml.HandlePlayerSpawnAtPacket(packet);
        }
    }
}