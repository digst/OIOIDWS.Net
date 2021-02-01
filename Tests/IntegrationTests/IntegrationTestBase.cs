using System.ServiceModel;
using Digst.OioIdws.OioWsTrust;
using DK.Gov.Oio.Idws.IntegrationTests.TokenAcquisition;

namespace DK.Gov.Oio.Idws.IntegrationTests
{
    public abstract class IntegrationTestBase
    {
        private IStsTokenService _tokenService;
        private ITokenAcquisitionScenario _acquisitionScenario;
        private Configuration _configuration;

        protected IntegrationTestBase()
        {
            // Optional log4net: XmlConfigurator.Configure();
        }

        protected void ConfigureWscAndSts(Configuration configuration)
        {
            _configuration = configuration;
            _tokenService = new StsTokenService(configuration.StsConfiguration);
        }

        protected void ConfigureSystemUserScenario()
        {
            _acquisitionScenario = new SystemUserScenario(_tokenService);
        }

        protected void ConfigureLocalTokenScenario()
        {
            _acquisitionScenario = new LocalTokenScenario(_configuration.LocalStsConfiguration, _tokenService);
        }
        
        protected void ConfigureBootstrapScenario()
        {
            _acquisitionScenario = new BootstrapScenario(_tokenService);
        }
        
        protected T CreateChannelWithIssuedToken<T>(ChannelFactory<T> factory)
        {
            var stsToken = _acquisitionScenario.AcquireTokenFromSts();
            return factory.CreateChannelWithIssuedToken(stsToken);
        }
    }
}