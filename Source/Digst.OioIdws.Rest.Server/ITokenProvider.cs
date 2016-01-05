using System;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;

namespace Digst.OioIdws.Rest.Server
{
    internal interface ITokenProvider
    {
        Task<OioIdwsToken> RetrieveTokenAsync(string accessToken, Uri accessTokenRetrievalEndpoint);
    }
}
