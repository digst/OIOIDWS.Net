using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Security.Cryptography.Xml;
using Digst.OioIdws.Common.Attributes;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;


namespace Digst.OioIdws.TestDoubles
{
    public class AuthenticationDescriptor
    {
        public string SessionIndex { get; set; }

        public string IpAddress { get; set; }

        public string DnsName { get; set; }

        public AssuranceLevel AssuranceLevel { get; set; }

        public DateTime? SessionNotOnOrAfter { get; set; }

        public DateTime AuthenticationInstant { get; set; }

        public SubjectDescriptor Subject { get; set; }
    }
}