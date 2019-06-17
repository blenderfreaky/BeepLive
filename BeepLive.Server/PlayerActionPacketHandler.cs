using System.Threading.Tasks;
using BeepLive.Config;
using BeepLive.Network;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;

namespace BeepLive.Server
{
    public class PlayerActionPacketHandler : PacketHandlerBase<PlayerActionPacket>
    {
        private readonly ILogger<PlayerActionPacketHandler> _logger;

        public PlayerActionPacketHandler(ILogger<PlayerActionPacketHandler> logger)
        {
            _logger = logger;
        }

        public override async Task Process(PlayerActionPacket packet, IPacketContext packetContext)
        {
            _logger.LogDebug("I received the chat action: " + XmlHelper.ToXml(packet));
        }
    }
}