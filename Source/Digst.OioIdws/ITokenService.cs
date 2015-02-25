using System.IdentityModel.Tokens;
using Digst.OioIdws.Configurations;

namespace Digst.OioIdws
{
    /// <summary>
    /// Used for retrieving a token from NemLog-in STS. The token can then be used to call WSP's (Web Service Providers).
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Recieves a token from a STS. The STS endpoint, client certificate and WSP endpointID are configured in the configuration file.
        /// This method is thread safe.
        /// </summary>
        /// <returns>Returns a token.</returns>
        SecurityToken GetToken();

        /// <summary>
        /// Recieves a token from a STS. The STS endpoint, client certificate and WSP endpointID are configured in the configuration file.
        /// Takes configuration as input argument instead of retrieving it from configuration file.
        /// This method is thread safe.
        /// </summary>
        /// <param name="config">Needed configuration in order to retrieve token from STS.</param>
        /// <returns>Returns a token.</returns>
        SecurityToken GetToken(OioIdwsConfiguration config);
    }
}
