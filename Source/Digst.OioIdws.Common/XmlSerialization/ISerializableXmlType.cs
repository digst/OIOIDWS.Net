namespace Digst.OioIdws.Common.XmlSerialization
{
    public interface ISerializableXmlType<T>
    {
        void Serialize(ITypeSerializer<T> serializer);
    }
}