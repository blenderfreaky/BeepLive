﻿namespace BeepLive.Network
{
    using ProtoBuf;

    [ProtoContract]
    public class PlayerShotPacket : PlayerActionPacket
    {
        [ProtoMember(3)] public Vector2fSerializable Direction;

        [ProtoMember(1)] public int ShotConfigId;

        public override string ToString() => $"{base.ToString()}, {nameof(ShotConfigId)}: {ShotConfigId}, {nameof(Direction)}: {Direction}";
    }
}