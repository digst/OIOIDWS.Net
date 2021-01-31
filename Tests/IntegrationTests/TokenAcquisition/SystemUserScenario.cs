using System.IdentityModel.Tokens;
using Digst.OioIdws.OioWsTrust;

namespace DK.Gov.Oio.Idws.IntegrationTests.TokenAcquisition
{
    public class SystemUserScenario : ITokenAcquisitionScenario
    {
        private readonly IStsTokenService _tokenService;

        public SystemUserScenario(IStsTokenService tokenService)
        {
            _tokenService = tokenService;
        }
        
        public SecurityToken AcquireTokenFromSts()
        {
            return _tokenService.GetToken();
        }
    }
}