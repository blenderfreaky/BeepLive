﻿using System.Threading.Tasks;
using BeepLive.Config;
using BeepLive.Network;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;

namespace BeepLive.Server
{
    public class PlayerShotPacketHandler : PacketHandlerBase<PlayerShotPacket>
    {
        private readonly ILogger<PlayerShotPacketHandler> _logger;

        public PlayerShotPacketHandler(ILogger<PlayerShotPacketHandler> logger)
        {
            _logger = logger;
        }

#pragma warning disable 1998
        public override async Task Process(PlayerShotPacket packet, IPacketContext packetContext)
#pragma warning restore 1998
        {
            _logger.LogDebug("I received the packet: " + XmlHelper.ToXml(packet));
        }
    }

    public class PlayerJumpPacketHandler : PacketHandlerBase<PlayerJumpPacket>
    {
        private readonly ILogger<PlayerJumpPacketHandler> _logger;

        public PlayerJumpPacketHandler(ILogger<PlayerJumpPacketHandler> logger)
        {
            _logger = logger;
        }

#pragma warning disable 1998
        public override async Task Process(PlayerJumpPacket packet, IPacketContext packetContext)
#pragma warning restore 1998
        {
            _logger.LogDebug("I received the packet: " + XmlHelper.ToXml(packet));
        }
    }

    public class PlayerFlowPacketHandler : PacketHandlerBase<PlayerFlowPacket>
    {
        private readonly ILogger<PlayerFlowPacketHandler> _logger;

        public PlayerFlowPacketHandler(ILogger<PlayerFlowPacketHandler> logger)
        {
            _logger = logger;
        }

#pragma warning disable 1998
        public override async Task Process(PlayerFlowPacket packet, IPacketContext packetContext)
#pragma warning restore 1998
        {
            _logger.LogDebug("I received the packet: " + XmlHelper.ToXml(packet));
        }
    }
}