using System.Threading.Tasks;
using BeepLive.Network;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;

#pragma warning disable 1998

namespace BeepLive.Server.PacketHandlers
{
    public class PlayerFlowPacketHandler : PacketHandlerBase<PlayerFlowPacket>
    {
        private readonly ILogger<PlayerFlowPacketHandler> _logger;

        public PlayerFlowPacketHandler(ILogger<PlayerFlowPacketHandler> logger)
        {
            _logger = logger;
        }

        public override async Task Process(PlayerFlowPacket packet, IPacketContext packetContext)
        {
            _logger.LogDebug("Received: " + packet);

            if (packet.Type == PlayerFlowPacket.PlayerFlowType.Join)
            {
                BeepServer.PlayerSecrets[packet.PlayerGuid] = packet.Secret;

                packetContext.Sender.Send(new SyncPacket(BeepServer.BeepConfig));

                packetContext.Sender.Send(new ServerFlowPacket(ServerFlowType.StartTeamSelection));
            }
        }
    }
}