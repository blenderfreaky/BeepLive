using System.Data;
using System.IO;
using System.Xml.Serialization;

namespace BeepLive.Config
{
    // https://stackoverflow.com/questions/11447529/convert-an-object-to-an-xml-string/21685169
    public static class XmlHelper
    {
        public static string ToXml<T>(T elem)
        {
            using var stringWriter = new StringWriter();
            var serializer = new XmlSerializer(elem.GetType());
            serializer.Serialize(stringWriter, elem);

            return stringWriter.ToString();
        }

        public static T LoadFromXmlString<T>(string xmlText)
        {
            using var stringReader = new StringReader(xmlText);
            var serializer = new XmlSerializer(typeof(T));

            return serializer.Deserialize(stringReader) is T t
                ? t
                : throw new StrongTypingException("Type doesn't match");
        }
    }
}