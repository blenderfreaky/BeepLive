using System;
using ProtoBuf;
using SFML.System;

namespace BeepLive.Network
{
    [ProtoContract]
    [ProtoInclude(100, typeof(PlayerShotPacket))]
    [ProtoInclude(200, typeof(PlayerJumpPacket))]
    [ProtoInclude(300, typeof(PlayerFlowPacket))]
    public abstract class PlayerActionPacket
    {
        [ProtoMember(3)] public string MessageGuid;

        [ProtoMember(1)] public string PlayerGuid;

        [ProtoMember(2)] public string Secret;
    }

    [ProtoContract]
    public class PlayerShotPacket : PlayerActionPacket
    {
        [ProtoMember(2)] public bool Destructive;

        [ProtoMember(3)] public Vector2FSerializable Direction;

        [ProtoMember(1)] public bool Neutral;
    }

    [ProtoContract]
    public class PlayerJumpPacket : PlayerActionPacket
    {
        [ProtoMember(1)] public Vector2FSerializable Direction;
    }

    [ProtoContract]
    public class PlayerFlowPacket : PlayerActionPacket
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