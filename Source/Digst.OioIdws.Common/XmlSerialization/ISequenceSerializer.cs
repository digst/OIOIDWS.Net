using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Digst.OioIdws.Common.XmlSerialization
{
    public interface ISequenceSerializer<T>
    {
        ISequenceSerializer<T> Element<TProp>(Expression<Func<T, TProp>> property, MinOccurs minOccurs=MinOccurs.Required) where TProp : ISerializableXmlElement;

        ISequenceSerializer<T> RepeatedElement<TProp>(Expression<Func<T, ICollection<TProp>>> element, MinOccurs min=MinOccurs.Optional, MaxOccurs max=MaxOccurs.UnBounded) where TProp : ISerializableXmlElement;




        ISequenceSerializer<T> Element<TProp,TType>(Expression<Func<T, TProp>> element, XName name) where TProp : ISerializableXmlType<TType>;

        ISequenceSerializer<T> RepeatedElement<TProp,TType>(Expression<Func<T, ICollection<TProp>>> element, XName name, MinOccurs min = MinOccurs.Optional, MaxOccurs max = MaxOccurs.UnBounded) where TProp : ISerializableXmlType<TType>;

    }
}