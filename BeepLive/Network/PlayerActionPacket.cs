using ProtoBuf;
using System;

namespace BeepLive.Network
{
    [ProtoContract]
    [ProtoInclude(100, typeof(PlayerShotPacket))]
    [ProtoInclude(200, typeof(PlayerJumpPacket))]
    [ProtoInclude(300, typeof(PlayerFlowPacket))]
    [ProtoInclude(400, typeof(PlayerTeamJoinPacket))]
    [ProtoInclude(500, typeof(PlayerSpawnAtPacket))]
    public abstract class PlayerActionPacket : Packet
    {

        [ProtoMember(1)] public string PlayerGuid;

        [ProtoMember(2)] public string Secret;
    }

    [ProtoContract]
    [ProtoInclude(100, typeof(PlayerActionPacket))]
    [ProtoInclude(200, typeof(ServerFlowPacket))]
    [ProtoInclude(300, typeof(SyncPacket))]
    public class Packet
    {
        [ProtoMember(1)] public string MessageGuid;
        [ProtoMember(2)] public DateTime TimeSent;

        public Packet()
        {
            MessageGuid = Guid.NewGuid().ToString();
            TimeSent = DateTime.Now;
        }

        public Packet(string messageGuid, DateTime timeSent)
        {
            MessageGuid = messageGuid;
            TimeSent = timeSent;
        }
    }
}