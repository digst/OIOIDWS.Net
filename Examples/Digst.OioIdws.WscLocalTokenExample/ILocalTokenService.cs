using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace Digst.OioIdws.WscLocalTokenExample
{
    /// <summary>
    /// Represents some security token service (STS) which can issue local tokens based
    /// on some locally available context (e.g. user logged into a corporate network)
    /// </summary>
    public interface ILocalTokenService
    {
        SecurityToken Issue(Saml2Subject subject, IEnumerable<Saml2Attribute> attributes, Uri audience);
    }
}