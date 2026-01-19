using System;
using Digst.OioIdws.Rest.Server.Wsp;

namespace Digst.OioIdws.Rest.Server.AuthorizationServer.CertificateRetrieval
{
    /// <summary>
    ///     Factory for creating certificate retrieval strategies based on configuration.
    /// </summary>
    public static class CertificateStrategyFactory
    {
        /// <summary>
        ///     Creates the appropriate <see cref="ICertificateRetrievalStrategy" />
        ///     based on the <see cref="CertificateStrategySection" /> settings. If nothing is configured, then
        ///     <see cref="EndpointCertificateStrategy" /> will be returned.
        /// </summary>
        public static ICertificateRetrievalStrategy Create()
        {
            var config = CertificateStrategySection.GetConfig();

            if (config == null)
                return new EndpointCertificateStrategy();

            switch (config.StrategyType)
            {
                case CertificateStrategyType.HttpHeader:
                    return new Rfc9440HttpHeaderCertificateStrategy(config.Value);

                case CertificateStrategyType.Connection:
                default:
                    return new EndpointCertificateStrategy();
            }
        }

        /// <summary>
        /// Creates new strategy with the specified options.
        /// </summary>
        /// <param name="options"><see cref="OioIdwsAuthorizationServiceOptions"/>Options the strategy will be created with.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ICertificateRetrievalStrategy Create(OioIdwsAuthorizationServiceOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.CertificateRetrievalStrategy != null)
            {
                return options.CertificateRetrievalStrategy;
            }

            if (options.CertificateStrategyType.HasValue)
            {
                switch (options.CertificateStrategyType.Value)
                {
                    case CertificateStrategyType.HttpHeader:
                        return new Rfc9440HttpHeaderCertificateStrategy(options.CertificateStrategyValue);
                    case CertificateStrategyType.Connection:
                    default: return new EndpointCertificateStrategy();
                }
            }

            return Create();
        }

        public static ICertificateRetrievalStrategy Create(OioIdwsAuthenticationOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (options.CertificateRetrievalStrategy != null) return options.CertificateRetrievalStrategy;
            if (options.CertificateStrategyType.HasValue)
                switch (options.CertificateStrategyType.Value)
                {
                    case CertificateStrategyType.HttpHeader:
                        return new Rfc9440HttpHeaderCertificateStrategy(options.CertificateStrategyValue);
                    case CertificateStrategyType.Connection:
                    default: return new EndpointCertificateStrategy();
                }

            return Create();
        }
    }
}