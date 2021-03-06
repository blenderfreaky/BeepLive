﻿namespace BeepLive.Network
{
    using ProtoBuf;
    using System;

    [ProtoContract]
    [ProtoInclude(100, typeof(PlayerActionPacket))]
    [ProtoInclude(200, typeof(ServerFlowPacket))]
    [ProtoInclude(300, typeof(SyncPacket))]
    public abstract class Packet
    {
        public static Type[] PacketTypes = new Type[]
        {
                typeof(SyncPacket),
                typeof(ServerFlowPacket),
                typeof(Packet),
                typeof(PlayerShotPacket),
                typeof(PlayerSpawnAtPacket),
                typeof(PlayerTeamJoinPacket),
                typeof(Vector2fSerializable),
                typeof(PlayerJumpPacket),
                typeof(PlayerFlowPacket),
                typeof(PlayerActionPacket)
        };

        [ProtoMember(1)] public Guid MessageGuid;
        [ProtoMember(2)] public DateTime TimeSent;

        protected Packet()
        {
            MessageGuid = Guid.NewGuid();
            TimeSent = DateTime.Now;
        }

        protected Packet(Guid messageGuid, DateTime timeSent)
        {
            MessageGuid = messageGuid;
            TimeSent = timeSent;
        }
    }
}