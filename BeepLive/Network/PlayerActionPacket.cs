using System;
using ProtoBuf;
using SFML.System;

namespace BeepLive.Network
{
    [ProtoContract]
    public abstract class PlayerActionPacket
    {
        [ProtoMember(3)] public Guid MessageGuid;

        [ProtoMember(1)] public Guid PlayerGuid;

        [ProtoMember(2)] public Guid Secret;
    }

    [ProtoContract]
    public class PlayerShot : PlayerActionPacket
    {
        [ProtoMember(2)] public bool Destructive;

        [ProtoMember(3)] public Vector2f Direction;

        [ProtoMember(1)] public bool Neutral;
    }

    [ProtoContract]
    public class PlayerJump : PlayerActionPacket
    {
        [ProtoMember(1)] public Vector2f Direction;
    }

    [ProtoContract]
    public class PlayerFlow : PlayerActionPacket
    {
        public enum PlayerFlowType
        {
            Join,
            ReadyForSimulation,
            FinishedSimulation
        }

        [ProtoMember(1)] public PlayerFlowType Type;
    }
}