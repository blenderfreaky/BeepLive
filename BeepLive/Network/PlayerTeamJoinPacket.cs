namespace BeepLive.Network
{
    using ProtoBuf;

    [ProtoContract]
    public class PlayerTeamJoinPacket : PlayerActionPacket
    {
        [ProtoMember(1)] public string TeamGuid;
        [ProtoMember(2)] public string UserName;

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(TeamGuid)}: {TeamGuid}, {nameof(UserName)}: {UserName}";
        }
    }
}