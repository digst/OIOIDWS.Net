using System;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;

namespace Digst.OioIdws.Rest.Server.Wsp
{
    internal interface ITokenProvider
    {
        Task<OioIdwsToken> RetrieveTokenAsync(string accessToken, Uri accessTokenRetrievalEndpoint);
    }
}
