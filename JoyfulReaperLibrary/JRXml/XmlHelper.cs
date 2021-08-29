/*
MIT License

Copyright(c) 2021 Kyle Givler
https://github.com/JoyfulReaper

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

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