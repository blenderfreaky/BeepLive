namespace BeepLive.Network
{
    using BeepLive.Config;
    using ProtoBuf;

    [ProtoContract]
    public class SyncPacket : Packet
    {
        [ProtoMember(1)] public string BeepConfigXml;

        public SyncPacket() : this(new BeepConfig())
        {
        }

        public SyncPacket(BeepConfig beepConfig)
        {
            BeepConfig = beepConfig;
        }

        public BeepConfig BeepConfig
        {
            get => XmlHelper.LoadFromXmlString<BeepConfig>(BeepConfigXml);
            set => BeepConfigXml = XmlHelper.ToXml(value);
        }

        public override string ToString()
        {
            return $"{nameof(BeepConfigXml)}: {BeepConfigXml}, {nameof(BeepConfig)}: {BeepConfig}";
        }
    }
}