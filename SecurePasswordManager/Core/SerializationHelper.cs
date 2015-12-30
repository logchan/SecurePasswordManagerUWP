using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SecurePasswordManager.Core
{
    public static class SerializationHelper
    {
        public static T DeserializeXml<T>(string xmlData) where T : class
        {
            T result = null;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            XmlReader reader = XmlReader.Create(new StringReader(xmlData));
            result = serializer.Deserialize(reader) as T;

            return result;
        }

        public static string SerializeXml<T>(T obj) where T : class
        {
            StringBuilder sb = new StringBuilder();
            TextWriter writer = new StringWriter(sb);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(writer, obj);

            return sb.ToString();
        }
    }
}
