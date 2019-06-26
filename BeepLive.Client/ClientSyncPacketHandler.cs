using System.Threading.Tasks;
using BeepLive.Network;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;

#pragma warning disable 1998

namespace BeepLive.Client
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
            BeepClient.BeepConfig = packet.BeepConfig;
            BeepClient.BeepClientInstance.BeepLiveSfml.HandleSyncPacket(packet);
        }
    }
}