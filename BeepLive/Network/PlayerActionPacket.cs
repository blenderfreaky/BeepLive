using ProtoBuf;

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

        public override string ToString()
        {
            return
                $"{nameof(MessageGuid)}: {MessageGuid}, {nameof(PlayerGuid)}: {PlayerGuid}, {nameof(Secret)}: {Secret}";
        }
    }

    [ProtoContract]
    public class PlayerShotPacket : PlayerActionPacket
    {
        [ProtoMember(2)] public bool Destructive;

        [ProtoMember(3)] public Vector2FSerializable Direction;

        [ProtoMember(1)] public bool Neutral;

        public override string ToString()
        {
            return
                $"{base.ToString()}, {nameof(Destructive)}: {Destructive}, {nameof(Direction)}: {Direction}, {nameof(Neutral)}: {Neutral}";
        }
    }

    [ProtoContract]
    public class PlayerJumpPacket : PlayerActionPacket
    {
        [ProtoMember(1)] public Vector2FSerializable Direction;

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(Direction)}: {Direction}";
        }
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

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(Type)}: {Type}";
        }
    }
}