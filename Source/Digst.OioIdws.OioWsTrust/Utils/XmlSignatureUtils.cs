using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Xml.Linq;
using Digst.OioIdws.OioWsTrust.SignatureCase;

namespace Digst.OioIdws.OioWsTrust.Utils
{
    public class XmlSignatureUtils
    {
        static XmlSignatureUtils()
        {
            // Adds SHA256 support to the .NET SignedXml class
            RsaPkcs1Sha256SignatureDescription.Register();
        }

        /// <summary>
        /// Signs an XmlDocument with an xml signature using the signing certificate given as argument to the method.
        /// In case it is ever needed ... this link explains how to add namespace prefixes to the generated signature. http://stackoverflow.com/questions/12219232/xml-signature-ds-prefix
        /// </summary>
        /// <param name="xDoc">The XDocument to be signed</param>
        /// <param name="ids">The ids of the elements in the xmldocument that must be signed.</param>
        /// <param name="cert">The certificate used to sign the document</param>
        public static XDocument SignDocument(XDocument xDoc, IEnumerable<string> ids, X509Certificate2 cert)
        {
            // Convert to XmlDocument as SignedXml only understands this type.
            var doc = ToXmlDocument(xDoc);

            // Apply private key, canonicalization method and signature method
            var signedXml = new SignedXmlWithIdResolvement(doc);
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            signedXml.SignedInfo.SignatureMethod = RsaPkcs1Sha256SignatureDescription.XmlDsigMoreRsaSha256Url;
            signedXml.SigningKey = cert.PrivateKey;

            // Make a reference for each element that must be signed.
            foreach (var id in ids)
            {
                var reference = new Reference("#" + id);
                reference.AddTransform(new XmlDsigExcC14NTransform());
                reference.DigestMethod = RsaPkcs1Sha256SignatureDescription.XmlEncSha256Url;
                signedXml.AddReference(reference);
            }

            // Include a reference to the certificate
            var referenceElement = doc.CreateElement(SignatureCaseMessageTransformer.WssePrefix,
                "Reference",
                SignatureCaseMessageTransformer.Wsse10Namespace);
            referenceElement.SetAttribute("URI", "#sec-binsectoken"); // Attribute must be in the empty namespace.
            var securityTokenReferenceElement = doc.CreateElement(SignatureCaseMessageTransformer.WssePrefix, "SecurityTokenReference",
                SignatureCaseMessageTransformer.Wsse10Namespace);
            securityTokenReferenceElement.AppendChild(referenceElement);
            signedXml.KeyInfo.AddClause(new KeyInfoNode(securityTokenReferenceElement));

            signedXml.ComputeSignature();

            // Append the computed signature. The signature must be placed as the sibling of the BinarySecurityToken element.
            var nsManager = new XmlNamespaceManager(doc.NameTable);
            nsManager.AddNamespace(SignatureCaseMessageTransformer.S11Prefix, SignatureCaseMessageTransformer.S11Namespace);
            nsManager.AddNamespace(SignatureCaseMessageTransformer.WssePrefix, SignatureCaseMessageTransformer.Wsse10Namespace);
            var securityNode = doc.SelectSingleNode("/" + SignatureCaseMessageTransformer.S11Prefix + ":Envelope/" + SignatureCaseMessageTransformer.S11Prefix + ":Header/" + SignatureCaseMessageTransformer.WssePrefix + ":Security", nsManager);
            var binarySecurityTokenNode = doc.SelectSingleNode("/" + SignatureCaseMessageTransformer.S11Prefix + ":Envelope/" + SignatureCaseMessageTransformer.S11Prefix + ":Header/" + SignatureCaseMessageTransformer.WssePrefix + ":Security/" + SignatureCaseMessageTransformer.WssePrefix + ":BinarySecurityToken", nsManager);
            securityNode.InsertAfter(doc.ImportNode(signedXml.GetXml(), true), binarySecurityTokenNode);

            var signedDocument = ToXDocument(doc);

            return signedDocument;
        }

        public static bool VerifySignature(XDocument xDocument, X509Certificate2 cert)
        {
            return VerifySignature(ToXmlDocument(xDocument), cert);
        }

        private static bool VerifySignature(XmlDocument xmlDocument, X509Certificate2 cert)
        {
            var signedXml = new SignedXmlWithIdResolvement(xmlDocument);
            var nodeList = xmlDocument.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#");
            if (nodeList.Count == 0)
                throw new InvalidOperationException("Document does not contain a signature to verify.");

            signedXml.LoadXml((XmlElement)nodeList[0]);

            return signedXml.CheckSignature(cert, true);
        }

        public static XmlDocument ToXmlDocument(XDocument xDocument)
        {
            // This conversion is not as effective as using XmlDocument.CreateReader().
            // However, using xDocument.CreateReader() makes the response signature from the STS invalid. The signature created by this class is not affected by using xDocument.CreateReader().
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xDocument.ToString());
            return xmlDocument;
        }

        public static XDocument ToXDocument(XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                // The node reader is disposed but not before the XDocument has been loaded.
                return XDocument.Load(nodeReader);
            }
        }
    }
}
