using System;
using System.Xml;
using System.Xml.Linq;

namespace Digst.OioIdws.Common.XmlSerialization
{
    public static class XmlWriterExtensions
    {

        public static void WriteStartElement(this XmlWriter writer, XName name)
        {
            writer.WriteStartElement(name.LocalName, name.NamespaceName);
        }


        public static void WriteStartAttribute(this XmlWriter writer, XName name)
        {
            writer.WriteStartAttribute(name.LocalName, name.NamespaceName);
        }


        public static void WriteAttribute(this XmlWriter writer, string localName, string value)
        {
            if (value == null) return;
            writer.WriteAttributeString(localName, value);
        }


        public static void WriteAttribute(this XmlWriter writer, XName name, string value)
        {
            if (value == null) return;
            writer.WriteAttributeString(name.LocalName, name.NamespaceName, value);
        }


        public static void WriteAttribute(this XmlWriter writer, string localName, DateTime? value)
        {
            if (!value.HasValue) return;
            writer.WriteAttributeString(localName, value.Value.ToUniversalTime().ToString("O"));
        }


        public static void WriteAttribute(this XmlWriter writer, string localName, bool? value)
        {
            if (!value.HasValue) return;
            writer.WriteAttributeString(localName, value.Value ? "true" : "false");
        }


        public static void WriteAttribute(this XmlWriter writer, string localName, int? value)
        {
            if (!value.HasValue) return;
            writer.WriteAttributeString(localName, value.Value.ToString());
        }


        public static void WriteAttributeUri(this XmlWriter writer, string localName, Uri value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            writer.WriteAttributeString(localName, value.ToString());
        }


    }
}