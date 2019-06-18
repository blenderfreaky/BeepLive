using ProtoBuf;

namespace BeepLive.Network
{
    [ProtoContract]
    public class ServerFlowPacket
    {
        [ProtoMember(2)] public ServerFlowType Type;

        [ProtoMember(1)] public long UnixTime;
    }

    public enum ServerFlowType
    {
        Spawn,
        StartSimulation,
        StartPlanning
    }
}