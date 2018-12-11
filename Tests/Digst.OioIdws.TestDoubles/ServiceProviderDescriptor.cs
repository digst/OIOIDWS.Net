using System;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Digst.OioIdws.TestDoubles
{

    public class ServiceProviderDescriptor
    {
        public Uri EntityId { get; set; }

        public Uri AssertionConsumerService { get; set; }

        public X509Certificate2 Certificate { get; set; }
    }
}
