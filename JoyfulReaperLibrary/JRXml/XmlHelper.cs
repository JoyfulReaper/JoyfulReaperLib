using System.IO;
using System.Xml.Serialization;

namespace JoyfulReaperLib.JRXml
{
    public static class XmlHelper
    {
        public static T DeserializeXml<T>(string xmlFile)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            FileStream fs = new FileStream(xmlFile, FileMode.Open);

            var output = (T)xs.Deserialize(fs);
            fs.Close();

            return output;
        }

        public static void SerializeXml<T>(T item)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));

            using (Stream s = File.Create(@"rss2.xml"))
            {
                xs.Serialize(s, item);
            }
        }
    }
}