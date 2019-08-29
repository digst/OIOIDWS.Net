using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Runtime.Caching;

namespace Digst.OioIdws.OioWsTrust
{
    /// <summary>
    /// A security token service client which caches the tokens issued by an inner client
    /// </summary>
    /// <seealso cref="Digst.OioIdws.OioWsTrust.IOioSecurityTokenServiceClient" />
    public class CachedOioSecurityTokenServiceClient : IOioSecurityTokenServiceClient
    {
        private readonly IOioSecurityTokenServiceClient _innerClient;
        private readonly MemoryCache _identityTokens = new MemoryCache("Default");

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedOioSecurityTokenServiceClient"/> class.
        /// </summary>
        /// <param name="innerClient">The inner client.</param>
        public CachedOioSecurityTokenServiceClient(IOioSecurityTokenServiceClient innerClient)
        {
            _innerClient = innerClient;
        }

        /// <summary>
        /// Gets the bootstrap token from authentication token. This will NOT be cached
        /// </summary>
        /// <param name="authenticationToken">The authentication token.</param>
        /// <returns></returns>
        public SecurityToken GetBootstrapTokenFromAuthenticationToken(SecurityToken authenticationToken)
        {
            return _innerClient.GetBootstrapTokenFromAuthenticationToken(authenticationToken);
        }

        /// <summary>
        /// Gets the identity token from bootstrap token.
        /// </summary>
        /// <param name="bootstrapToken">The bootstrap token.</param>
        /// <param name="wspIdentifier">The WSP identifier.</param>
        /// <param name="keyType">Type of the key.</param>
        /// <param name="claims">The claims.</param>
        /// <returns></returns>
        public SecurityToken GetIdentityTokenFromBootstrapToken(SecurityToken bootstrapToken,
            string wspIdentifier,
            KeyType keyType,
            RequestClaimCollection claims)
        {
            var key = $"{bootstrapToken}|{wspIdentifier}|{keyType}";
            foreach (var claim in claims)
            {
                key += $"[{claim.ClaimType}:{claim.Value}]";
            }

            var idt = (SecurityToken)_identityTokens.Get(key,"IdentityTokens");
            if (idt != null && idt.ValidTo.AddMinutes(-1) >= DateTime.UtcNow)
            {
                // Still at least one minute to go
                return idt;
            }

            idt = _innerClient.GetIdentityTokenFromBootstrapToken(bootstrapToken, wspIdentifier, keyType, claims);

            _identityTokens.Add(key, idt, idt.ValidTo, "IdentityTokens");

            return idt;
        }

        /// <summary>
        /// Gets a service token.
        /// </summary>
        /// <param name="wspIdentifier">The WSP identifier.</param>
        /// <param name="claims">The claims.</param>
        /// <returns></returns>
        public SecurityToken GetServiceToken(string wspIdentifier, RequestClaimCollection claims)
        {
            return _innerClient.GetServiceToken(wspIdentifier, claims);
        }
    }
}