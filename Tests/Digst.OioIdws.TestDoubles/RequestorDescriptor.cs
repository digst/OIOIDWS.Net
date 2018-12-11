using System.Security.Cryptography.X509Certificates;

namespace Digst.OioIdws.TestDoubles
{
    public class RequestorDescriptor
    {
        public X509Certificate2 Certificate { get; set; }
    }
}