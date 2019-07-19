using ProtoBuf;

namespace BeepLive.Network
{
    [ProtoContract]
    public class PlayerTeamJoinPacket : PlayerActionPacket
    {
        [ProtoMember(1)] public int TeamIndex;

        public override string ToString() => $"{base.ToString()}, {nameof(TeamIndex)}: {TeamIndex}";
    }
}