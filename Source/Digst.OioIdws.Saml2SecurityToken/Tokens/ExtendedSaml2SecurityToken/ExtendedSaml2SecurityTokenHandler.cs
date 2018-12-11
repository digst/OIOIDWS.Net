using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.ServiceModel.Security;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Digst.OioIdws.Common.XmlSerialization;
using static Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken.Saml2Constants.Namespaces;
using static Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken.Saml2Constants.ElementNames;
using static Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken.Saml2Constants.AttributeNames;

namespace Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken
{
    public class ExtendedSaml2SecurityTokenHandler : Saml2SecurityTokenHandler
    {

        private static readonly XNamespace wsse = XNamespace.Get("http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
        private static readonly XName SecurityTokenReference = wsse + "SecurityTokenReference";
        private static readonly XName Reference = wsse + "Reference";
        private static readonly XName KeyIdentifier = wsse + "KeyIdentifier";

        public ExtendedSaml2SecurityTokenHandler()
        {
        }

        public ExtendedSaml2SecurityTokenHandler(SamlSecurityTokenRequirement samlSecurityTokenRequirement) : base(samlSecurityTokenRequirement)
        {
        }


        protected override void WriteStatement(XmlWriter writer, Saml2Statement data)
        {
            if (data is Saml2ComplexAttributeStatement statement)
            {
                WriteAttributeStatement(writer, statement);
            }
            else base.WriteStatement(writer, data);
        }


        protected virtual void WriteAttributeStatement(XmlWriter writer, Saml2ComplexAttributeStatement statement)
        {
            // <AttributeStatement>
            writer.WriteStartElement(AttributeStatementXName);

            var xsi = writer.LookupPrefix("http://www.w3.org/2001/XMLSchema-instance");
            if (xsi == null)
            {
                writer.WriteAttributeString("xsi", XNamespace.Xmlns.NamespaceName, "http://www.w3.org/2001/XMLSchema-instance");
                xsi = "xsi";
            }

            var xs = writer.LookupPrefix("http://www.w3.org/2001/XMLSchema");
            if (xs == null)
            {
                writer.WriteAttributeString("xs", XNamespace.Xmlns.NamespaceName, "http://www.w3.org/2001/XMLSchema");
                xs = "xs";
            }

            // <Attribute> 1-OO
            foreach (var attribute in statement.Attributes)
            {
                WriteAttribute(writer, attribute);
            }

            // </AttributeStatement>
            writer.WriteEndElement();
        }


        protected virtual void WriteAttribute(XmlWriter writer, ComplexSamlAttribute complexAttribute)
        {

            // <Attribute>
            writer.WriteStartElement(AttributeXName);

            // @name attribute
            writer.WriteAttributeString(NameXName.LocalName, complexAttribute.Name);

            // @nameFormat attribute
            writer.WriteAttribute(NameFormatXName.LocalName, complexAttribute.NameFormat?.AbsoluteUri);

            // @friendlyName attribute
            writer.WriteAttribute(FriendlyNameXName.LocalName, complexAttribute.FriendlyName);

            // @originalIssuer attribute
            writer.WriteAttribute(OriginalIssuerXName, complexAttribute.OriginalIssuer);


            // <Value> elements
            foreach (var value in complexAttribute.Values)
            {
                WriteAttributeValue(writer, value, complexAttribute.XsiType);
            }

            // </Attribute>
            writer.WriteEndElement();
        }


        private void WriteAttributeValue(XmlWriter writer, ComplexSamlAttributeValue value, XName xsiType)
        {
            writer.WriteStartElement(AttributeValueXName);

            if (xsiType != null)
            {
                // Need to write the xsi:Type attribute

                var prefix = writer.LookupPrefix(xsiType.NamespaceName);
                if (prefix == null)
                {
                    writer.WriteAttributeString("q", XNamespace.Xmlns.NamespaceName, xsiType.NamespaceName);
                    prefix = "q";
                }

                writer.WriteStartAttribute("xsi", "type", "http://www.w3.org/2001/XMLSchema-instance");
                writer.WriteQualifiedName(xsiType.LocalName, xsiType.NamespaceName);
                writer.WriteEndAttribute();
            }

            foreach (var node in value.AttributeValueElement.Nodes())
            {
                node.WriteTo(writer);
            }

            writer.WriteEndElement();
        }



        public override string WriteToken(SecurityToken token)
        {
            var sb = new StringBuilder();

            var xw = XmlWriter.Create(sb, new XmlWriterSettings() { Encoding = Encoding.UTF8, Indent = true });

            WriteToken(xw, token);

            return sb.ToString();
        }


        public override SecurityToken ReadToken(string tokenString)
        {
            var xr = XmlReader.Create(new StringReader(tokenString));
            return new Saml2SecurityToken(base.ReadAssertion(xr));
        }


        protected virtual IEnumerable<ComplexSamlAttributeValue> ReadComplexAttributeValues(XmlReader reader)
        {
            var list = new List<ComplexSamlAttributeValue>();
            while (reader.IsStartElement(AttributeValueXName))
            {
                list.Add(ReadComplexAttributeValue(reader));
            }
            return list;
        }


        protected virtual ComplexSamlAttributeValue ReadComplexAttributeValue(XmlReader reader)
        {
            if (!reader.IsStartElement(AttributeValueXName)) throw new InvalidOperationException("The current element is not an AttributeValue element.");

            var xsiTypeAttribute = reader.GetAttribute(Saml2Constants.Namespaces.xsi + "type");
            XElement attributeValueElement = (XElement) XNode.ReadFrom(reader);

            XName xsiType;
            if (!string.IsNullOrWhiteSpace(xsiTypeAttribute))
            {
                var split = xsiTypeAttribute.Split(new char[] {':'}, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length > 1)
                {
                    xsiType = XName.Get(split[1],split[0]);
                }
                else
                {
                    xsiType = XName.Get(split[0], reader.LookupNamespace(string.Empty)??"");
                }
            }
            else
            {
                xsiType = null;
            }

            if (attributeValueElement.HasElements)
            {
                return new ComplexSamlAttributeValue(attributeValueElement.Elements().Single());
            }
            else
            {
                return new ComplexSamlAttributeValue(attributeValueElement.Value);
            }
        }


        protected override Saml2Statement ReadStatement(XmlReader reader)
        {

            if (reader.IsStartElement(AttributeStatementXName))
            {
                return ReadComplexAttributeStatement(reader);
            }
            return base.ReadStatement(reader);
        }


        protected override Saml2AttributeStatement ReadAttributeStatement(XmlReader reader)
        {
            return this.ReadComplexAttributeStatement(reader);
        }

        protected virtual Saml2ComplexAttributeStatement ReadComplexAttributeStatement(XmlReader reader)
        {
            reader.ReadStartElement(AttributeStatementXName);

            var attributes = ReadComplexAttributes(reader);

            var statement = new Saml2ComplexAttributeStatement(attributes);

            reader.ReadEndElement();

            return statement;
        }

        protected virtual IEnumerable<ComplexSamlAttribute> ReadComplexAttributes(XmlReader reader)
        {
            var list = new List<ComplexSamlAttribute>();
            while (reader.IsStartElement(AttributeXName))
            {
                list.Add(ReadComplexAttribute(reader));
                reader.MoveToContent();
            }
            return list;
        }

        protected virtual ComplexSamlAttribute ReadComplexAttribute(XmlReader reader)
        {

            var name = reader.GetAttribute("Name");
            var nameFormat = reader.GetAttribute(NameFormatXName.LocalName)?.ToUri();
            var friendlyName = reader.GetAttribute(FriendlyNameXName.LocalName);
            var originalIssuer = reader.GetAttribute(OriginalIssuerXName);

            reader.ReadStartElement(AttributeXName);

            var values = ReadComplexAttributeValues(reader).ToList();

            reader.ReadEndElement();

            return new ComplexSamlAttribute(name, values)
            {
                FriendlyName = friendlyName,
                NameFormat = nameFormat,
                OriginalIssuer = originalIssuer,
            };
        }

        public override SecurityKeyIdentifierClause ReadKeyIdentifierClause(XmlReader reader)
        {
            string referencedId;

            reader.ReadStartElement(SecurityTokenReference);

            reader.ReadStartElement(KeyIdentifier);
            referencedId = reader.ReadString();
            reader.ReadEndElement();

            reader.ReadEndElement();

            return new LocalIdKeyIdentifierClause(referencedId);
        }
    }
}