using BeepLive.Network;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;
using System.Threading.Tasks;

#pragma warning disable 1998

namespace BeepLive.Client.PacketHandlers
{
    public class ClientSyncPacketHandler : PacketHandlerBase<SyncPacket>
    {
        private readonly ILogger<ClientSyncPacketHandler> _logger;

        public ClientSyncPacketHandler(ILogger<ClientSyncPacketHandler> logger)
        {
            _logger = logger;
        }

        public override async Task Process(SyncPacket packet, IPacketContext packetContext)
        {
            _logger.LogDebug("Received: " + packet);

            BeepClient.BeepClientInstance.BeepLiveSfml.HandlePacket(packet);
        }
    }
}