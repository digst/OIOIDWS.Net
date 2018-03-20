namespace Digst.OioIdws.Wsp.Wsdl.Utils
{
    using System.Collections.Generic;
    using System.Xml;

    using Namespace = System.String;
    using Prefix = System.String;

    public static class Namespaces
    {
        private static string tempUriNamespace = "http://tempuri.org/";
        private static Dictionary<Prefix, Namespace> required =
            new Dictionary<Prefix, Namespace>
            {
                { "xs" ,   "http://www.w3.org/2001/XMLSchema" },
                { "xsi",   "http://www.w3.org/2001/XMLSchema-instance" },
                // http://schemas.xmlsoap.org/
                { "soap",  "http://schemas.xmlsoap.org/wsdl/soap12/" },
                { "http",  "http://schemas.xmlsoap.org/wsdl/http" },
                { "mime",  "http://schemas.xmlsoap.org/wsdl/mime" },
                // https://www.w3.org/Submission/WS-PAEPR/#notational 
                { "wsa",   "http://www.w3.org/2005/08/addressing" },
                { "wsp",   "http://www.w3.org/ns/ws-policy" },
                // https://www.w3.org/Submission/WS-EndpointReference/#XML_Namespaces 
                { "wsdl",  "http://schemas.xmlsoap.org/wsdl/" },
                // https://www.w3.org/TR/2006/CR-ws-addr-wsdl-20060529/#namespaces 
                { "wsaw",  "http://schemas.xmlsoap.org/ws/2004/09/policy" },
                // https://www.w3.org/TR/ws-addr-metadata/#namespaces 
                { "wsam",  "http://www.w3.org/2007/05/addressing/metadata" },
                // https://msdn.microsoft.com/en-us/library/ms996497.aspx (Appendix C)
                { "wsap",  "http://www.w3.org/2006/05/addressing/wsdl" },
                // http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702/ws-securitypolicy-1.2-spec-os.html#_Toc161826497
                { "sp",    "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702" },
                { "wsse",  "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd" },
                { "wsu",   "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd" },
                { "wst",   "http://docs.oasis-open.org/ws-sx/ws-trust/200512" },
                // http://docs.oasis-open.org/imi/identity/v1.0/os/identity-1.0-spec-os.html#_Toc229451821
                { "ds",    "http://www.w3.org/2000/09/xmldsig#" },
                { "wsai",  "http://schemas.xmlsoap.org/ws/2006/02/addressingidentity" },
                { "sp11",  "http://schemas.xmlsoap.org/ws/2005/07/securitypolicy" },
                { "wst12", "http://schemas.xmlsoap.org/ws/2005/02/trust" },
                // Serialization
                { "xsser", "http://schemas.microsoft.com/2003/10/Serialization/" },
                // TempUriNamespace
                { "tns",   tempUriNamespace }
            };
        private static string operationNamespace = required["wsa"];

        public static string TempUriNamespace { get => tempUriNamespace; }
        public static Dictionary<Prefix, Namespace> Required { get => required; }
        public static string OperationNamespace { get => operationNamespace; }

        public static Dictionary<Namespace, Prefix> Merge(
            XmlQualifiedName[] namespaces,
            Dictionary<Prefix, Namespace> custom)
        {
            foreach (var ns in namespaces)
            {
                if (!custom.ContainsKey(ns.Name) &&
                    !custom.ContainsValue(ns.Namespace))
                {
                    custom.Add(ns.Name, ns.Namespace);
                }
            }

            return custom;
        }
    }
}