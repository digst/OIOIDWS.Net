using System.Security.Cryptography.Xml;
using System.Xml;
using Digst.OioIdws.OioWsTrust.ProtocolChannel;

namespace Digst.OioIdws.OioWsTrust.Utils
{
    ///<summary>
    /// Needed in order to reference elements where the id attribute is prefixed with a namespace. E.g. wsu:Id
    ///</summary>
    public class SignedXmlWithIdResolvement : SignedXml
    {
        public SignedXmlWithIdResolvement(XmlDocument document) : base(document)
        {
        }

        /// <summary>
        /// Id attributes are prefixed with <see cref="Constants.WsuPrefix"/> and can not be resolved by the standard implementation. This implementation handles this.
        /// </summary>
        public override XmlElement GetIdElement(XmlDocument document, string idValue)
        {
            // Check to see if it's a standard ID reference
            var idElem = base.GetIdElement(document, idValue);

            if (idElem == null)
            {
                var nsManager = new XmlNamespaceManager(document.NameTable);
                nsManager.AddNamespace(Constants.WsuPrefix, Constants.WsuNamespace);

                idElem = document.SelectSingleNode("//*[@" + Constants.WsuPrefix + ":Id=\"" + idValue + "\"]", nsManager) as XmlElement;
            }

            return idElem;
        }
    }
}