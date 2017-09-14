using System;
using System.Security.Cryptography.X509Certificates;
using Digst.OioIdws.Rest.Client.AccessToken;

namespace Digst.OioIdws.Rest.Client
{
    /// <summary>
    /// Settings related to communication with AS
    /// </summary>
    public class OioIdwsClientSettings
    {
        /// <summary>
        /// Constructor that defaults makes use of token cache and uses a default value of 5 minutes in clock skew in the cache implementation.
        /// </summary>
        public OioIdwsClientSettings()
        {
            CacheClockSkew = TimeSpan.FromSeconds(300);
            UseTokenCache = true;
        }

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
        /// <summary>
        /// This is used to determine how long before the token actually expires ... the token should be removed from the cache.
        /// E.g. if token will expire in 100 seconds and <see cref="CacheClockSkew"/> is set to 10 seconds ... then the token will be removed from the cache after 90 seconds.
        /// If not set ... the default value is 300 seconds.
        /// This configuration setting is only used in conjunction with <see cref="AccessTokenServiceCache"/>
        /// </summary>
        public TimeSpan CacheClockSkew { get; set; }
        /// <summary>
        /// Specifies wheter or not to use the token cache variant <see cref="AccessTokenServiceCache"/> or <see cref="AccessTokenService"/>.
        /// If true <see cref="AccessTokenServiceCache"/> is used which is the default.
        /// Be aware that if <see cref="OioIdwsStsSettings.TokenLifeTime"/> is shorter than <see cref="DesiredAccessTokenExpiry"/> then it is the longest life time of these two that determines the actual caching time.
        /// Thus, if <see cref="OioIdwsStsSettings.TokenLifeTime"/> is 5 minutes and <see cref="DesiredAccessTokenExpiry"/> is 1 hour then a new access token is still only fetched each 1 hour.
        /// </summary>
        public bool UseTokenCache { get; set; }
    }
}