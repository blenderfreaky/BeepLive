using ProtoBuf;

namespace BeepLive.Network
{
    [ProtoContract]
    [ProtoInclude(100, typeof(PlayerShotPacket))]
    [ProtoInclude(200, typeof(PlayerJumpPacket))]
    [ProtoInclude(300, typeof(PlayerFlowPacket))]
    [ProtoInclude(400, typeof(PlayerTeamJoinPacket))]
    [ProtoInclude(500, typeof(PlayerSpawnAtPacket))]
    public abstract class PlayerActionPacket
    {
        [ProtoMember(3)] public string MessageGuid;

        [ProtoMember(1)] public string PlayerGuid;

        [ProtoMember(2)] public string Secret;

        public override string ToString() => $"{nameof(MessageGuid)}: {MessageGuid}, {nameof(PlayerGuid)}: {PlayerGuid}, {nameof(Secret)}: {Secret}";
    }
}