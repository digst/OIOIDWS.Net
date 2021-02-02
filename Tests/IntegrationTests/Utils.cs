using System.Security.Cryptography.X509Certificates;

namespace DK.Gov.Oio.Idws.IntegrationTests
{
    public static class Utils
    {
        public static X509Certificate2 ReadCertificateFile(string path)
        {
            return new X509Certificate2(path);
        }

        public static X509Certificate2 ReadCertificateFile(string path, string password)
        {
            return new X509Certificate2(path, password, X509KeyStorageFlags.Exportable);
        }
    }
}