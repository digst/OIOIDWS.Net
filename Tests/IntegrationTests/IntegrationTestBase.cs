using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.Rest.Client;
using Digst.OioIdws.Rest.Client.AccessToken;
using Digst.OioIdws.Rest.Common;
using DK.Gov.Oio.Idws.IntegrationTests.TokenAcquisition;

namespace DK.Gov.Oio.Idws.IntegrationTests
{
    public abstract class IntegrationTestBase
    {
        private readonly IStsTokenService _tokenService;
        private readonly IAccessTokenService _accessTokenService;

        private ITokenAcquisitionScenario _acquisitionScenario;

        protected Configuration Configuration { get; }

        protected IntegrationTestBase(Configuration configuration)
        {
            // Optional log4net: XmlConfigurator.Configure();

            Configuration = configuration;
            _tokenService = new StsTokenService(configuration.StsConfiguration);
            _accessTokenService = new AccessTokenService(Configuration.RestWspConfiguration);
        }

        protected void ConfigureSystemUserScenario()
        {
            _acquisitionScenario = new SystemUserScenario(_tokenService);
        }

        protected void ConfigureLocalTokenScenario()
        {
            _acquisitionScenario = new LocalTokenScenario(Configuration.LocalStsConfiguration, _tokenService);
        }

        protected void ConfigureBootstrapScenario()
        {
            //_acquisitionScenario = new BootstrapScenario(_tokenService);
        }

        protected T CreateChannelWithIssuedToken<T>(ChannelFactory<T> factory)
        {
            var stsToken = _acquisitionScenario.AcquireTokenFromSts();
            return factory.CreateChannelWithIssuedToken(stsToken);
        }

        protected async Task<HttpClient> CreateHttpClientWithIssuedToken()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) =>
            {
                // Because the Java WSP is hosted on localhost using a self-signed certificate, we ignore certification validation.
                if (sender is HttpWebRequest req && req.Address.IsLoopback) return true;
                return errors == SslPolicyErrors.None;
            };
            
            var stsToken = (GenericXmlSecurityToken)_acquisitionScenario.AcquireTokenFromSts();
            var accessToken = await _accessTokenService.GetTokenAsync(stsToken, CancellationToken.None);

            var requestHandler = new WebRequestHandler();
            requestHandler.ClientCertificates.Add(Configuration.RestWspConfiguration.ClientCertificate);
            
            var client = new HttpClient(requestHandler);
            
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(AccessTokenTypeParser.ToString(accessToken.Type), accessToken.Value);

            return client;
        }
    }
}
