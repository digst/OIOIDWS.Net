using System.Security.Cryptography.X509Certificates;

namespace DK.Gov.Oio.Idws.IntegrationTests
{
    public class LocalStsConfiguration
    {
        public string EntityId { get; set; }
        public X509Certificate2 SigningCertificate { get; set; }
        public X509Certificate2 HolderOfKeyCertificate { get; set; }
    }
}