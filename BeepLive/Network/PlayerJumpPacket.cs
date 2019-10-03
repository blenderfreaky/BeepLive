namespace BeepLive.Network
{
    using ProtoBuf;

    [ProtoContract]
    public class PlayerJumpPacket : PlayerActionPacket
    {
        [ProtoMember(1)] public Vector2fSerializable Direction;

        public override string ToString() => $"{base.ToString()}, {nameof(Direction)}: {Direction}";
    }
}