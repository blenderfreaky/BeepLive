#pragma warning disable 1998

namespace BeepLive.Client.PacketHandlers
{
    using BeepLive.Network;
    using Microsoft.Extensions.Logging;
    using Networker.Common;
    using Networker.Common.Abstractions;
    using System.Threading.Tasks;

    public class ClientTeamJoinPacketHandler : PacketHandlerBase<PlayerTeamJoinPacket>
    {
        private readonly ILogger<ClientTeamJoinPacketHandler> _logger;

        public ClientTeamJoinPacketHandler(ILogger<ClientTeamJoinPacketHandler> logger)
        {
            _logger = logger;
        }

        public override async Task Process(PlayerTeamJoinPacket packet, IPacketContext packetContext)
        {
            _logger.LogDebug("Received: " + packet);
            BeepClient.BeepClientInstance.BeepLiveSfml.HandlePacket(packet);
        }
    }
}