using System.Threading.Tasks;
using BeepLive.Config;
using BeepLive.Network;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;

namespace BeepLive.Client
{
    public class ClientPlayerActionPacketHandler : PacketHandlerBase<PlayerActionPacket>
    {
        private readonly ILogger<ClientPlayerActionPacketHandler> _logger;

        public ClientPlayerActionPacketHandler(ILogger<ClientPlayerActionPacketHandler> logger)
        {
            _logger = logger;
        }

        public override async Task Process(PlayerActionPacket packet, IPacketContext packetContext)
        {
            _logger.LogDebug("Client received the response packet: " + XmlHelper.ToXml(packet));
        }
    }
}