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
        StartTeamSelection,
        StopTeamSelection,
        StartSpawning,
        StopSpawning,
        StartSimulation,
        StopSimulation,
        StartPlanning,
        StopPlanning
    }
}