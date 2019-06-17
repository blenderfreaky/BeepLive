using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;
using SFML.System;

namespace BeepLive.Network
{
    [ProtoContract]
    public abstract class PlayerActionPacket
    {
        [ProtoMember(1)]
        public Guid PlayerGuid;
        [ProtoMember(2)]
        public Guid Secret;

        [ProtoMember(3)]
        public Guid MessageGuid;
    }

    [ProtoContract]
    public class PlayerShot : PlayerActionPacket
    {
        [ProtoMember(1)]
        public bool Neutral;
        [ProtoMember(2)]
        public bool Destructive;

        [ProtoMember(3)]
        public Vector2f Direction;
    }

    [ProtoContract]
    public class PlayerJump : PlayerActionPacket
    {
        [ProtoMember(1)]
        public Vector2f Direction;
    }

    [ProtoContract]
    public class PlayerFlow : PlayerActionPacket
    {
        public enum PlayerFlowType
        {
            Join,
            ReadyForSimulation,
            FinishedSimulation,
        }

        [ProtoMember(1)]
        public PlayerFlowType Type;
    }
}
