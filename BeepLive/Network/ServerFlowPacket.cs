using System;
using ProtoBuf;

namespace BeepLive.Network
{
    [ProtoContract]
    public class ServerFlowPacket
    {
        [ProtoMember(2)] public ServerFlowType Type;

        [ProtoMember(1)] public long UnixTime;

        public ServerFlowPacket()
        {
            UnixTime = DateTime.UtcNow.Ticks;
        }
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