using System;
using System.IO;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.AuthorizationService.Storage;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Newtonsoft.Json;

namespace Digst.OioIdws.Rest.AuthorizationService.Retrieval
{
    internal class AccessTokenRetriever
    {
        private readonly ISecurityTokenStore _securityTokenStore;
        private readonly ILogger _logger;

        public AccessTokenRetriever(ISecurityTokenStore securityTokenStore, ILogger logger)
        {
            if (securityTokenStore == null)
            {
                throw new ArgumentNullException(nameof(securityTokenStore));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _securityTokenStore = securityTokenStore;
            _logger = logger;
        }

        public async Task RetrieveAsync(IOwinContext context, OioIdwsAuthorizationServiceMiddleware.Settings settings)
        {
            if (!context.Request.QueryString.HasValue)
            {
                //todo return token given
            }

            //todo: check that token is valid
            var accessToken = context.Request.QueryString.Value;

            _logger.WriteInformation($"Attempting to retrieve information for access token {accessToken}");
            var token = await _securityTokenStore.RetrieveTokenAsync(accessToken);

            if (token == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

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
