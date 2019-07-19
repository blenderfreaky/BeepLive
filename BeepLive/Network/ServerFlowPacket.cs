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

        public override string ToString()
        {
            return $"{nameof(Type)}: {Type}, {nameof(UnixTime)}: {UnixTime}";
        }
    }

    public enum ServerFlowType
    {
        StartTeamSelection,
        StartSpawning,
        StartSimulation,
        StartPlanning

    }
}