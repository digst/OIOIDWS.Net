using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken
{
    public class ComplexSamlAttribute
    {

        /// <summary>
        /// Gets the values of the attribute.
        /// </summary>
        public List<ComplexSamlAttributeValue> Values { get; }


        public ComplexSamlAttribute(string name, string stringValue)
            : this(name, null, stringValue)
        {
        }

        public ComplexSamlAttribute(string name, Uri nameFormat, string stringValue)
            : this(name, nameFormat, new[] { stringValue })
        {
        }


        public ComplexSamlAttribute(string name, Uri nameFormat, IEnumerable<string> stringValues)
            : this(name, nameFormat, stringValues.Select(x => new ComplexSamlAttributeValue(x)))
        {
        }


        public ComplexSamlAttribute(string name, Uri nameFormat, XElement value)
            : this(name, nameFormat, new[] { value })
        {
        }


        public ComplexSamlAttribute(string name, XElement value, XName xsiType = null)
            : this(name, null, new[] { value }, xsiType)
        {
        }



        public ComplexSamlAttribute(string name, IEnumerable<XElement> values, XName xsiType = null)
            : this(name, null, values, xsiType)
        {
        }


        public ComplexSamlAttribute(string name, Uri nameFormat, IEnumerable<XElement> values, XName xsiType = null)
            : this(name, nameFormat, values.Select(x => new ComplexSamlAttributeValue(x)))
        {
        }


        public ComplexSamlAttribute(string name, IEnumerable<ComplexSamlAttributeValue> values) : this(name, null, values)
        {
        }

        public ComplexSamlAttribute(string name, Uri nameFormat, IEnumerable<ComplexSamlAttributeValue> values, XName xsiType=null)
        {
            Name = name;
            NameFormat = nameFormat;
            Values = values.ToList();
            XsiType = xsiType;
        }

        /// <summary>
        /// Gets or sets a string that provides a more human-readable form of the attribute's 
        /// name. [Saml2Core, 2.7.3.1]
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the name of the attribute. [Saml2Core, 2.7.3.1]
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a URI reference representing the classification of the attribute 
        /// name for the purposes of interpreting the name. [Saml2Core, 2.7.3.1]
        /// </summary>
        public Uri NameFormat { get; set; }


        public XName XsiType { get; }

        /// <summary>
        /// Gets or sets the string that represents the OriginalIssuer of the this SAML Attribute.
        /// </summary>
        public string OriginalIssuer { get; set; }


    }

}