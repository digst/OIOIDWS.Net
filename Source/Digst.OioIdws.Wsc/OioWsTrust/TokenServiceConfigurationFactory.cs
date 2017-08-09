using System;
using Digst.OioIdws.OioWsTrust;

namespace Digst.OioIdws.Wsc.OioWsTrust
{
    /// <summary>
    /// This factory class ca be used to generate a <see cref="TokenServiceConfiguration"/> configuration based on a <see cref="Configuration"/> configuration.
    /// </summary>
    public class TokenServiceConfigurationFactory
    {
        public static TokenServiceConfiguration CreateConfiguration(Configuration wscConfiguration)
        {
            var tokenServiceConfiguration = new TokenServiceConfiguration
            {
                ClientCertificate = CertificateUtil.GetCertificate(wscConfiguration.ClientCertificate),
                StsCertificate = CertificateUtil.GetCertificate(wscConfiguration.StsCertificate),
                SendTimeout = wscConfiguration.DebugMode ? TimeSpan.FromDays(1) : (TimeSpan?)null,
                StsEndpointAddress = wscConfiguration.StsEndpointAddress,
                TokenLifeTimeInMinutes = wscConfiguration.TokenLifeTimeInMinutes,
                WspEndpointId = wscConfiguration.WspEndpointID
            };

            if (wscConfiguration.CacheClockSkewInSeconds.HasValue)
                tokenServiceConfiguration.CacheClockSkew =
                    TimeSpan.FromSeconds((double)wscConfiguration.CacheClockSkewInSeconds);

            return tokenServiceConfiguration;
        }

        public static TokenServiceConfiguration CreateConfiguration()
        {
            var wscConfiguration =
                (Configuration) System.Configuration.ConfigurationManager.GetSection("oioIdwsWcfConfiguration");

            return CreateConfiguration(wscConfiguration);
        }
    }
}