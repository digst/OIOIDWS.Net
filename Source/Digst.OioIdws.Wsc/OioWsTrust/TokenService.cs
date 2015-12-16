using System;
using System.IdentityModel.Tokens;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.Wsc.Utils;

namespace Digst.OioIdws.Wsc.OioWsTrust
{
    /// <summary>
    /// <see cref="ITokenService"/>
    /// </summary>
    public class TokenService : ITokenService
    {
        /// <summary>
        /// <see cref="ITokenService.GetToken()"/>
        /// </summary>
        public SecurityToken GetToken()
        {
            // Retrieve Configuration
            var config =
                (Configuration)System.Configuration.ConfigurationManager.GetSection("oioIdwsWcfConfiguration");

            return GetToken(config);
        }

        /// <summary>
        /// <see cref="ITokenService.GetToken(Configuration)"/>
        /// </summary>
        public SecurityToken GetToken(Configuration config)
        {
            // Check input arguments
            if (config == null) throw new ArgumentNullException("config");
            if (string.IsNullOrEmpty(config.WspEndpointID)) throw new ArgumentException("WspEndpointID");
            if (string.IsNullOrEmpty(config.StsEndpointAddress)) throw new ArgumentException("StsEndpointAddress");
            // X509FindType cannot be tested below because default value is FindByThumbprint
            if (config.ClientCertificate == null || config.ClientCertificate.StoreLocation == 0 || config.ClientCertificate.StoreName == 0 || string.IsNullOrEmpty(config.ClientCertificate.FindValue)) throw new ArgumentException("ClientCertificate");
            if (config.StsCertificate == null || config.StsCertificate.StoreLocation == 0 || config.StsCertificate.StoreName == 0 || string.IsNullOrEmpty(config.StsCertificate.FindValue)) throw new ArgumentException("StsCertificate");

            return new TokenIssuingService().RequestToken(new TokenIssuingRequestConfiguration
            {
                ClientCertificate = CertificateUtil.GetCertificate(config.ClientCertificate),
                StsCertificate = CertificateUtil.GetCertificate(config.StsCertificate),
                SendTimeout = config.DebugMode ? TimeSpan.FromDays(1) : (TimeSpan?) null,
                StsEndpointAddress = config.StsEndpointAddress,
                TokenLifeTimeInMinutes = config.TokenLifeTimeInMinutes,
                WspEndpointId = config.WspEndpointID,
            });
        }
    }
}