namespace Digst.OioIdws.Rest.Server.AuthorizationServer.CertificateRetrieval
{
    /// <summary>
    /// Factory for creating certificate retrieval strategies based on configuration.
    /// </summary>
    public static class CertificateStrategyFactory
    {
        /// <summary>
        /// Creates the appropriate <see cref="ICertificateRetrievalStrategy"/> 
        /// based on the <see cref="CertificateStrategySection"/> settings. If nothing is configured, then <see cref="EndpointCertificateStrategy"/> will be returned.
        /// </summary>
        public static ICertificateRetrievalStrategy Create()
        {
            var config = CertificateStrategySection.GetConfig();

            if(config==null)
                return new EndpointCertificateStrategy();
            
            switch (config.StrategyType)
            {
                case CertificateStrategyType.HttpHeader:
                    return new HttpHeaderCertificateStrategy(config.Value);

                case CertificateStrategyType.Connection:
                default:
                    return new EndpointCertificateStrategy();
            }
        }
    }
}