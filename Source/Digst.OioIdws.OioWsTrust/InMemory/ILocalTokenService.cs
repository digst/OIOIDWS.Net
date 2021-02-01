using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;

namespace Digst.OioIdws.OioWsTrust.InMemory
{
    /// <summary>
    /// Represents some security token service (STS) which can issue local tokens based
    /// on some locally available context (e.g. user logged into a corporate network)
    /// </summary>
    public interface ILocalTokenService
    {
        /// <summary>
        /// Issues a token
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="attributes">The attributes.</param>
        /// <param name="audience">The audience.</param>
        /// <returns></returns>
        SecurityToken Issue(Saml2Subject subject, IEnumerable<Saml2Attribute> attributes, Uri audience);
    }
}