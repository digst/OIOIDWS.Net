using System.Xml.Linq;

namespace Digst.OioIdws.Common.XmlSerialization
{
    public interface ISerializableXmlElement
    {
        XName Name { get; }
    }
}