using System.Xml.Linq;

namespace Digst.OioIdws.Common.XmlSerialization
{
    public interface IChoiceSerializer<T,TProp>
    {
        IChoiceSerializer<T, TProp> Element<TDerived>() where TDerived : ISerializableXmlType<TProp>, ISerializableXmlElement;

        IChoiceSerializer<T, TProp> Element<TDerived>(XName name) where TDerived : ISerializableXmlType<TProp>;
    }
}