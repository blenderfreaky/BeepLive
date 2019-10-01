namespace BeepLive.Network
{
    using ProtoBuf;

    [ProtoContract]
    public class PlayerSpawnAtPacket : PlayerActionPacket
    {
        [ProtoMember(1)] public Vector2FSerializable Position;

        public override string ToString() => $"{base.ToString()}, {nameof(Position)}: {Position}";
    }
}