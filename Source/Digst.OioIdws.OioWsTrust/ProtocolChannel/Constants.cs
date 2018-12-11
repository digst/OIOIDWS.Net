using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digst.OioIdws.OioWsTrust.ProtocolChannel
{
    public class Constants
    {
        // Namespaces
        public const string WsaNamespace = "http://www.w3.org/2005/08/addressing";
        public const string S11Namespace = "http://schemas.xmlsoap.org/soap/envelope/";
        public const string Wsp12Namespace = "http://schemas.xmlsoap.org/ws/2004/09/policy"; // Corresponds to WS-Policy 1.2.
        
        public const string Wst13Namespace = "http://docs.oasis-open.org/ws-sx/ws-trust/200512";
        public const string Wst14Namespace = "http://docs.oasis-open.org/ws-sx/ws-trust/200802";
        public const string WsuNamespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";
        public const string Wsse10Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
        public const string Wsse11Namespace = "http://docs.oasis-open.org/wss/oasis-wss-wssecurity-secext-1.1.xsd";
        public const string XmlDigSigNamespace = "http://www.w3.org/2000/09/xmldsig#";
        public const string AuthNamespace = "http://docs.oasis-open.org/wsfed/authorization/200706";

        public const string VsDebuggerNamespace = "http://schemas.microsoft.com/vstudio/diagnostics/servicemodelsink";
        public const string WcfDiagnosticsNamespace = "http://schemas.microsoft.com/2004/09/ServiceModel/Diagnostics";

        // STS Entity ID's
        public const string BootstrapTokenCaseEntityId = "https://bootstrap.sts.nemlog-in.dk/";
        public const string LocalTokenCaseEntityId = " https://local.sts.nemlog-in.dk/"; // Currently not used
        public const string SignatureCaseEntityId = "https://signature.sts.nemlog-in.dk/";

        // Token type
        public const string Saml20TokenType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0";

        // SAML value type
        public const string SamlValueType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLID";

        // ID values.
        public const string ActionIdValue = "action";
        public const string MessageIdIdValue = "msgid";
        public const string ToIdValue = "to";
        public const string TimeStampIdValue = "sec-ts";
        public const string BinarySecurityTokenIdValue = "sec-binsectoken";
        public const string BodyIdValue = "body";

        // Namespace prefixes
        public const string S11Prefix = "S11";
        public const string WsaPrefix = "wsa";
        public const string WsuPrefix = "wsu";
        public const string WssePrefix = "wsse";
        public const string WspPrefix = "wsp";
        public const string Wst13Prefix = "wst";

        // Datetime formats
        public const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ"; // Results in format 2015-01-14T14:50:24Z mandated by spec.

    }
}
