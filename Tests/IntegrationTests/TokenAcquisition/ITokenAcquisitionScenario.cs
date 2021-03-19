using System.IdentityModel.Tokens;

namespace DK.Gov.Oio.Idws.IntegrationTests.TokenAcquisition
{
    public interface ITokenAcquisitionScenario
    {
        SecurityToken AcquireTokenFromSts();
    }
}