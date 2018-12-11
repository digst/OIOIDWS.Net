using System;
using Digst.OioIdws.OioWsTrust;

namespace Digst.OioIdws.Wsc.OioWsTrust
{
    /// <summary>
    /// This factory class can be used to generate a <see cref="SecurityTokenServiceClientConfiguration"/> configuration based on a <see cref="Configuration"/> configuration.
    /// </summary>
    public class TokenServiceConfigurationFactory
    {
        public static SecurityTokenServiceClientConfiguration CreateConfiguration(Configuration wscConfiguration)
        {
            var tokenServiceConfiguration = new SecurityTokenServiceClientConfiguration
            {
                WscCertificate = CertificateUtil.GetCertificate(wscConfiguration.ClientCertificate),
                StsCertificate = CertificateUtil.GetCertificate(wscConfiguration.StsCertificate),
                SendTimeout = wscConfiguration.DebugMode ? TimeSpan.FromDays(1) : (TimeSpan?)null,
                ServiceTokenUrl = new Uri(wscConfiguration.StsEndpointAddress),
                TokenLifeTime = wscConfiguration.TokenLifeTimeInMinutes.HasValue ? TimeSpan.FromMinutes(wscConfiguration.TokenLifeTimeInMinutes.Value) : TimeSpan.FromMinutes(5),
                WscIdentifier = wscConfiguration.WscIdentifier,
                StsIdentifier = wscConfiguration.StsIdentifier,
            };

            if (wscConfiguration.CacheClockSkewInSeconds.HasValue)
                tokenServiceConfiguration.CacheClockSkew =
                    TimeSpan.FromSeconds((double)wscConfiguration.CacheClockSkewInSeconds);

            return tokenServiceConfiguration;
        }

        public static SecurityTokenServiceClientConfiguration CreateConfiguration()
        {
            var wscConfiguration =
                (Configuration)System.Configuration.ConfigurationManager.GetSection("oioIdwsWcfConfiguration");

            return CreateConfiguration(wscConfiguration);
        }
    }
}