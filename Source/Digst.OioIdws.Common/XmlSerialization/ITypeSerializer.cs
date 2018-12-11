using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Digst.OioIdws.Common.XmlSerialization
{
    public interface ITypeSerializer<T>
    {
        void Attribute<TProp>(Expression<Func<T, TProp>> attribute);

        void Sequence(Action<ISequenceSerializer<T>> sequence);

        void Base<TBase>(Action<ITypeSerializer<TBase>> serialize);

        void Choice<TProp>(Expression<Func<T,TProp>> property, Action<IChoiceSerializer<T,TProp>> choice);

        void RepeatedChoice<TProp>(Expression<Func<T,ICollection<TProp>>> property, Action<IChoiceSerializer<T,TProp>> choice, MinOccurs minOccurs=MinOccurs.Optional, MaxOccurs maxOccurs=MaxOccurs.UnBounded);

        void Text(Expression<Func<T, string>> text);
    }
}