using System;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Digst.OioIdws.Rest.Client
{
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
            
            //todo: Detect token type, only add client certificate if holder-of-key
            ClientCertificates.Add(client.Settings.ClientCertificate);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await EnsureValidAccessTokenAsync(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue(_accessToken.TypeString, _accessToken.Value);

            var response = await base.SendAsync(request, cancellationToken);
            //todo: proper handling of auth errors

            return response;
        }

        private async Task EnsureValidAccessTokenAsync(CancellationToken cancellationToken)
        {
            if (_accessToken == null || !_accessToken.IsValid())
            {
                EnsureValidSecurityToken();
                _accessToken = await _client.GetAccessTokenAsync(_securityToken, cancellationToken);
            }
        }

        private void EnsureValidSecurityToken()
        {
            //todo: time skew?
            if (_securityToken == null || _securityToken.ValidTo < DateTime.UtcNow)
            {
                _securityToken = _client.GetSecurityToken();
            }
        }
    }
}
