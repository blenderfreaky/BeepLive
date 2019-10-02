namespace BeepLive.Network
{
    using ProtoBuf;

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
}