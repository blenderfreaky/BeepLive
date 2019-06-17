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

#pragma warning disable 1998
        public override async Task Process(PlayerActionPacket packet, IPacketContext packetContext)
#pragma warning restore 1998
        {
            _logger.LogDebug("Client received the response packet: " + XmlHelper.ToXml(packet));
        }
    }
}