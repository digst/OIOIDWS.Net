using System.IdentityModel.Tokens;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Digst.OioIdws.OioWsTrust;
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
            _accessTokenService = new AccessTokenService(configuration.OioIdwsClientSettings);
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
            // Request token from STS.
            var stsToken = (GenericXmlSecurityToken)_acquisitionScenario.AcquireTokenFromSts();
            
            // Request access token from the access token service using the acquired STS token.
            var accessToken = await _accessTokenService.GetTokenAsync(stsToken, CancellationToken.None);

            var requestHandler = new WebRequestHandler();
            requestHandler.ClientCertificates.Add(Configuration.OioIdwsClientSettings.ClientCertificate);
            
            // Build HttpClient with authorization header set using the access token.
            var client = new HttpClient(requestHandler)
            {
                BaseAddress = Configuration.RestWspConfiguration.Endpoint,
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue(
                        AccessTokenTypeParser.ToString(accessToken.Type), accessToken.Value)
                }
            };

            return client;
        }
    }
}