using System;
using System.Security.Cryptography.X509Certificates;

namespace Digst.OioIdws.Rest.Client
{
    public class OioIdwsClientSettings
    {
        /// <summary>
        /// Represents the client certificate including the private key. This should be either a MOCES, FOCES or VOCES certificate.
        /// </summary>
        public X509Certificate2 ClientCertificate { get; set; }
        /// <summary>
        /// The audiance of the WSP
        /// </summary>
        public Uri AudienceUri { get; set; }
        /// <summary>
        /// Settings for the Security Token Service
        /// </summary>
        public OioIdwsStsSettings SecurityTokenService { get; set; }
        /// <summary>
        /// Endpoint on the Authorization Server that issues access tokens
        /// </summary>
        public Uri AccessTokenIssuerEndpoint { get; set; }
        /// <summary>
        /// Access tokens will have an expire time controlled by the Authorization Server. The client can make a request to control the expiration, based on this value.
        /// It's not a part of the spec and thereby only supported in the .NET implementation.
        /// The server will only allow issuing tokens with a lower expires_in based on this request.
        /// </summary>
        public TimeSpan? DesiredAccessTokenExpiry { get; set; }
    }
}