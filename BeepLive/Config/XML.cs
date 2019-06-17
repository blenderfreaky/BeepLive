using System.Data;
using System.IO;
using System.Xml.Serialization;

namespace BeepLive.Config
{
    // https://stackoverflow.com/questions/11447529/convert-an-object-to-an-xml-string/21685169
    internal class XML
    {
        public string ToXML()
        {
            using StringWriter stringWriter = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(GetType());
            serializer.Serialize(stringWriter, this);

            return stringWriter.ToString();
        }

        public static T LoadFromXMLString<T>(string xmlText)
        {
            using StringReader stringReader = new StringReader(xmlText);
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            return serializer.Deserialize(stringReader) is T t
                ? t
                : throw new StrongTypingException("Type doesn't match");
        }
    }
}