using System;
using System.IO;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.AuthorizationService.Storage;
using Microsoft.Owin;
using Newtonsoft.Json;

namespace Digst.OioIdws.Rest.AuthorizationService.Retrieval
{
    internal class AccessTokenRetriever
    {
        private readonly ISecurityTokenStore _securityTokenStore;

        public AccessTokenRetriever(ISecurityTokenStore securityTokenStore)
        {
            if (securityTokenStore == null)
            {
                throw new ArgumentNullException(nameof(securityTokenStore));
            }

            _securityTokenStore = securityTokenStore;
        }

        public async Task RetrieveAsync(IOwinContext context, OioIdwsAuthorizationServiceMiddleware.Settings settings)
        {
            if (!context.Request.QueryString.HasValue)
            {
                //todo return token given
            }

            //todo: check that token is valid
            var accessToken = context.Request.QueryString.Value;

            var token = await _securityTokenStore.RetrieveTokenAsync(accessToken);

            if (token.ValidUntilUtc < DateTime.UtcNow)
            {
                //todo return token expired
            }

            context.Response.ContentType = "application/json; charset=UTF-8";

            var serializer = new JsonSerializer();
            using (var writer = new StreamWriter(context.Response.Body))
            {
                serializer.Serialize(writer, token);
            }
        }
    }
}
