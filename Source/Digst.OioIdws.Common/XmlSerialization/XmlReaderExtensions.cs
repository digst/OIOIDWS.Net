using System;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace Digst.OioIdws.Common.XmlSerialization
{
    public static class XmlReaderExtensions
    {
        public static bool IsStartElement(this XmlReader reader, XName name)
        {
            return reader.IsStartElement(name.LocalName, name.NamespaceName);
        }

        public static void ReadStartElement(this XmlReader reader, XName name)
        {
            reader.ReadStartElement(name.LocalName, name.NamespaceName);
        }


        public static string GetAttribute(this XmlReader reader, XName name)
        {
            return reader.GetAttribute(name.LocalName, name.NamespaceName);
        }




        public static DateTime GetAttributeAsUtcDateTime(this XmlReader reader, string localName)
        {
            var s = reader.GetAttribute(localName);
            return DateTime.ParseExact(s, "O", null, DateTimeStyles.AssumeUniversal);
        }


        public static Uri GetOptionalAttributeAsUri(this XmlReader reader, string localName)
        {
            var s = reader.GetAttribute(localName);
            if (s == null) return null;
            return new Uri(s);
        }

        public static Uri GetRequiredAttributeAsUri(this XmlReader reader, string localName)
        {
            var s = reader.GetAttribute(localName);
            if (s == null) throw new XmlException($"Missing attribute {localName}");
            return new Uri(s);
        }

    }
}