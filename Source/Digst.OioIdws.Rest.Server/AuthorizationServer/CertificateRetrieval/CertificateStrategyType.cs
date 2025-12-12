namespace Digst.OioIdws.Rest.Server.AuthorizationServer.CertificateRetrieval
{
    /// <summary>
    /// Enumeration of supported certificate retrieval strategies.
    /// </summary>
    public enum CertificateStrategyType
    {
        /// <summary>
        /// Retrieve the client certificate directly from the TLS connection.
        /// </summary>
        Connection,

        /// <summary>
        /// Retrieve the client certificate from an HTTP header.
        /// </summary>
        HttpHeader
    }
}