using BeepLive.Config;
using ProtoBuf;

namespace BeepLive.Network
{
    [ProtoContract]
    public class SyncPacket
    {
        [ProtoMember(1)] public MapConfig MapConfig;
    }
}