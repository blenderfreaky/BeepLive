#pragma warning disable 1998

namespace BeepLive.Client.PacketHandlers
{
    using BeepLive.Network;
    using Microsoft.Extensions.Logging;
    using Networker.Common;
    using Networker.Common.Abstractions;
    using System.Threading.Tasks;

    public class ClientServerFlowPacketHandler : PacketHandlerBase<ServerFlowPacket>
    {
        private readonly ILogger<ClientServerFlowPacketHandler> _logger;

        public ClientServerFlowPacketHandler(ILogger<ClientServerFlowPacketHandler> logger)
        {
            _logger = logger;
        }

        public override async Task Process(ServerFlowPacket packet, IPacketContext packetContext)
        {
            _logger.LogDebug("Received: " + packet);
            BeepClient.BeepClientInstance.BeepLiveSfml.HandlePacket(packet);
        }
    }
}