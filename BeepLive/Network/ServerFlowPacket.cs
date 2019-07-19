using ProtoBuf;

namespace BeepLive.Network
{
    [ProtoContract]
    public class ServerFlowPacket : Packet
    {
        [ProtoMember(1)] public ServerFlowType Type;
    }

    public enum ServerFlowType
    {
        StartTeamSelection,
        StartSpawning,
        StartSimulation,
        StartPlanning
    }
}