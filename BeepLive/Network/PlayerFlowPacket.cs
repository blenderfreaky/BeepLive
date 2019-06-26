using ProtoBuf;

namespace BeepLive.Network
{
    [ProtoContract]
    public class PlayerFlowPacket : PlayerActionPacket
    {
        public enum PlayerFlowType
        {
            Join,
            Spawn,
            ReadyForSimulation,
            FinishedSimulation,
        }

        [ProtoMember(1)] public PlayerFlowType Type;

        public override string ToString() => $"{base.ToString()}, {nameof(Type)}: {Type}";
    }
}