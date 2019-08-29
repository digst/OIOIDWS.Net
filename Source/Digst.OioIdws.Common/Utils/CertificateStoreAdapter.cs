using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Digst.OioIdws.Common.Utils
{
    public interface ICertificateStore
    {
        X509Certificate2 GetByThumbprint(string thumbprint);
    }

    public interface ICertificateStoreFactory
    {
        ICertificateStore GetCertificateStore();
    }

    public class LocalMachineCertificateStoreFactory : ICertificateStoreFactory
    {
        public ICertificateStore GetCertificateStore()
        {
            return new X509CertificateStore(new X509Store(StoreName.My, StoreLocation.LocalMachine));
        }
    }

    public class CurrentUserCertificateStoreFactory : ICertificateStoreFactory
    {
        public ICertificateStore GetCertificateStore()
        {
            return new X509CertificateStore(new X509Store(StoreName.My, StoreLocation.CurrentUser));
        }
    }

    public class X509CertificateStore : ICertificateStore
    {
        private readonly X509Store _x509Store;

        public X509CertificateStore(X509Store x509Store)
        {
            _x509Store = x509Store;
        }

        public X509Certificate2 GetByThumbprint(string thumbprint)
        {
            _x509Store.Open(OpenFlags.ReadOnly);
            var cert = _x509Store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, true);
            _x509Store.Close();
            return cert.Count > 0 ? cert[0] : null;
        }
    }
}
