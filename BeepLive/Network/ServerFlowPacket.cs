using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace BeepLive.Network
{
    [ProtoContract]
    public class ServerFlowPacket
    {
        [ProtoMember(1)]
        public long UnixTime;

        [ProtoMember(2)]
        public ServerFlowType Type;
    }

    public enum ServerFlowType
    {
        Spawn,
        StartSimulation,
        StartPlanning,
    }
}
