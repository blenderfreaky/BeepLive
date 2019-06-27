using ProtoBuf;

namespace BeepLive.Network
{
    [ProtoContract]
    public class PlayerFlowPacket : PlayerActionPacket
    {
        public enum FlowType
        {
            Join,
            Leave,
            LockInTeam,
            Spawn,
            ReadyForSimulation,
            FinishedSimulation
        }

        [ProtoMember(1)] public FlowType Type;

        public override string ToString() => $"{base.ToString()}, {nameof(Type)}: {Type}";
    }
}