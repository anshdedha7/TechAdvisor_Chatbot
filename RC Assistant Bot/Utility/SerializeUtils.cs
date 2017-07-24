using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace RC_Assistant_Bot.Utility
{
    public class SerializeUtils
    {
        public static string serialize(Type t, object o)
        {
            String XmlizedString = null;
            MemoryStream memoryStream = new MemoryStream();
            XmlSerializer xs = new XmlSerializer(t);
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            xs.Serialize(xmlTextWriter, o);
            memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
            UTF8Encoding encoding = new UTF8Encoding();
            XmlizedString = encoding.GetString(memoryStream.ToArray());
            memoryStream.Close();
            return XmlizedString.Trim();
        }

        public static object deserialize(Type t, string xmlData)
        {
            XmlSerializer xs = new XmlSerializer(t);
            UTF8Encoding encoding = new UTF8Encoding();
            MemoryStream memoryStream = new MemoryStream(encoding.GetBytes(xmlData.Trim()));
            object o = xs.Deserialize(memoryStream);
            memoryStream.Close();
            return o;
        }
    }
}