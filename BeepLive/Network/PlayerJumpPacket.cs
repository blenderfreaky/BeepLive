﻿using ProtoBuf;

namespace BeepLive.Network
{
    [ProtoContract]
    public class PlayerJumpPacket : PlayerActionPacket
    {
        [ProtoMember(1)] public Vector2FSerializable Direction;

        public override string ToString() => $"{base.ToString()}, {nameof(Direction)}: {Direction}";
    }
}