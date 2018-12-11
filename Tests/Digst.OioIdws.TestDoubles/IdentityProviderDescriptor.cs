using System.Security.Cryptography.X509Certificates;

namespace Digst.OioIdws.TestDoubles
{
    public class IdentityProviderDescriptor
    {
        public string IssuerName { get; set; }

        public X509Certificate2 Certificate { get; set; }

        public string IssuerDomain { get; set; }
    }
}