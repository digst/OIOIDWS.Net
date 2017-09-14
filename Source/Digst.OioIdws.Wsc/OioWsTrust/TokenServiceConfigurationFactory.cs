using System;
using Digst.OioIdws.OioWsTrust;

namespace Digst.OioIdws.Wsc.OioWsTrust
{
    /// <summary>
    /// This factory class can be used to generate a <see cref="StsTokenServiceConfiguration"/> configuration based on a <see cref="Configuration"/> configuration.
    /// </summary>
    public class TokenServiceConfigurationFactory
    {
        public static StsTokenServiceConfiguration CreateConfiguration(Configuration wscConfiguration)
        {
            var tokenServiceConfiguration = new StsTokenServiceConfiguration
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

        public static StsTokenServiceConfiguration CreateConfiguration()
        {
            var wscConfiguration =
                (Configuration) System.Configuration.ConfigurationManager.GetSection("oioIdwsWcfConfiguration");

            return CreateConfiguration(wscConfiguration);
        }
    }
}