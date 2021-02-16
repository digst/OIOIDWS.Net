using System;
using Digst.OioIdws.Rest.Client;

namespace DK.Gov.Oio.Idws.IntegrationTests
{
    public class RestWspConfiguration : OioIdwsClientSettings
    {
        public Uri Endpoint { get; set; }
    }
}