using System;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;

namespace Digst.OioIdws.Rest.Authentication
{
    internal interface ITokenProvider
    {
        Task<OioIdwsToken> RetrieveTokenAsync(string accessToken, Uri accessTokenRetrievalEndpoint);
    }
}
