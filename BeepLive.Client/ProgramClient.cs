using System.IO;
using BeepLive.Config;

namespace BeepLive.Client
{
    public class ProgramClient
    {
        private static void Main(string[] args)
        {
            const string beepConfigXml = "BeepConfig.xml";

            BeepConfig beepConfig;
            if (!File.Exists(beepConfigXml)) File.WriteAllText(beepConfigXml, XmlHelper.ToXml(beepConfig = new BeepConfig()));
            else beepConfig = XmlHelper.LoadFromXmlString<BeepConfig>(File.ReadAllText(beepConfigXml));

            if (beepConfig != null) _ = new BeepClient(beepConfig).Client.Connect();
        }
    }
}