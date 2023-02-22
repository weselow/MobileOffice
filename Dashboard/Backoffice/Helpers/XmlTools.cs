using System.Xml.Serialization;

namespace Dashboard.Backoffice.Helpers
{
    public static class XmlTools
    {
        /// <summary>
        /// Сериализуем класс в текст-xml.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SerializeXml<T>(T data)
        {
            if (data == null) return string.Empty;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, data);
                return textWriter.ToString();
            }
        }

        /// <summary>
        /// Десериализуем xml: текст в класс.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T DeserializeXml<T>(this string data)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringReader textReader = new StringReader(data))
            {
                return (T)xmlSerializer.Deserialize(textReader)!;
            }
        }
    }
}
