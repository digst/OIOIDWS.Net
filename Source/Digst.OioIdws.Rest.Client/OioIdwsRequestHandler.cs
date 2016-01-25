using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;

namespace Digst.OioIdws.Rest.Client
{
    /// <summary>
    /// Can be used by HttpClient or similar to handle plumbing of issuing tokens and token expirations.
    /// The handler is not thread-safe.
    /// </summary>
    public class OioIdwsRequestHandler : WebRequestHandler
    {
        private readonly OioIdwsClient _client;
        private GenericXmlSecurityToken _securityToken;
        private AccessToken _accessToken;
        public OioIdwsRequestHandler(OioIdwsClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            _client = client;

            //We can't know in advance whether it's a bearer/holder-of-key token we're going to work with. Either way we just add the certificate to the request, if given
            if (client.Settings.ClientCertificate != null)
            {
                ClientCertificates.Add(client.Settings.ClientCertificate);
            }
        }

        /// <summary>
        /// Normally the handler will perform expiration test to avoid using tokens about to expire and instead re-negotiate security token and access token immediately.
        /// Setting this property to true will disable this, forcing the handler to experience invalid_token challenge.
        /// </summary>
        public bool DisableClientSideExpirationValidation { get; set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await SendAuthenticatedRequest(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized && IsInvalidToken(response))
            {
                //if the request is denied due to the token no longer being valid, we flush the tokens, ensure they are reloaded, and refire the request
                _securityToken = null;
                _accessToken = null;
                response = await SendAuthenticatedRequest(request, cancellationToken);
            }

            return response;
        }

        private async Task<HttpResponseMessage> SendAuthenticatedRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await EnsureValidAccessTokenAsync(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue(_accessToken.TypeString, _accessToken.Value);
            return await base.SendAsync(request, cancellationToken);
        }

        private static bool IsInvalidToken(HttpResponseMessage response)
        {
            return response.Headers.WwwAuthenticate.Any(x =>
                AccessTokenTypeParser.FromString(x.Scheme).HasValue &&
                HttpHeaderUtils.ParseOAuthSchemeParameter(x.Parameter)["error"].Equals(AuthenticationErrorCodes.InvalidToken, StringComparison.OrdinalIgnoreCase));
        }

        private async Task EnsureValidAccessTokenAsync(CancellationToken cancellationToken)
        {
            if (_accessToken == null || (!DisableClientSideExpirationValidation && !_accessToken.IsValid()))
            {
                _securityToken = _client.GetSecurityToken();
                _accessToken = await _client.GetAccessTokenAsync(_securityToken, cancellationToken);
            }
        }
    }
}
