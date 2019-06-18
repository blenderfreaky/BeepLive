using System.Threading.Tasks;
using BeepLive.Network;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;

namespace BeepLive.Client
{
    public class ClientPlayerShotPacketHandler : PacketHandlerBase<PlayerShotPacket>
    {
        private readonly ILogger<ClientPlayerShotPacketHandler> _logger;

        public ClientPlayerShotPacketHandler(ILogger<ClientPlayerShotPacketHandler> logger)
        {
            _logger = logger;
        }

#pragma warning disable 1998
        public override async Task Process(PlayerShotPacket packet, IPacketContext packetContext)
#pragma warning restore 1998
        {
            _logger.LogDebug("Received: " + packet);
        }
    }

    public class ClientPlayerJumpPacketHandler : PacketHandlerBase<PlayerJumpPacket>
    {
        private readonly ILogger<ClientPlayerJumpPacketHandler> _logger;

        public ClientPlayerJumpPacketHandler(ILogger<ClientPlayerJumpPacketHandler> logger)
        {
            _logger = logger;
        }

#pragma warning disable 1998
        public override async Task Process(PlayerJumpPacket packet, IPacketContext packetContext)
#pragma warning restore 1998
        {
            _logger.LogDebug("Received: " + packet);
        }
    }

    public class ClientServerFlowPacketHandler : PacketHandlerBase<ServerFlowPacket>
    {
        private readonly ILogger<ClientServerFlowPacketHandler> _logger;

        public ClientServerFlowPacketHandler(ILogger<ClientServerFlowPacketHandler> logger)
        {
            _logger = logger;
        }

#pragma warning disable 1998
        public override async Task Process(ServerFlowPacket packet, IPacketContext packetContext)
#pragma warning restore 1998
        {
            _logger.LogDebug("Received: " + packet);
        }
    }

    public class ClientSyncPacketHandler : PacketHandlerBase<SyncPacket>
    {
        private readonly ILogger<ClientSyncPacketHandler> _logger;

        public ClientSyncPacketHandler(ILogger<ClientSyncPacketHandler> logger)
        {
            _logger = logger;
        }

#pragma warning disable 1998
        public override async Task Process(SyncPacket packet, IPacketContext packetContext)
#pragma warning restore 1998
        {
            _logger.LogDebug("Received: " + packet);
            BeepClient.BeepConfig = packet.BeepConfig;
        }
    }
}