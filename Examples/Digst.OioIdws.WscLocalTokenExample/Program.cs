using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.Wsc.OioWsTrust;
using Digst.OioIdws.WscLocalTokenExample.HelloWorldProxy;
using log4net.Config;

namespace Digst.OioIdws.WscLocalTokenExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Setup Log4Net configuration by loading it from configuration file. 
            // log4net is not necessary and is only being used for demonstration.
            XmlConfigurator.Configure();

            // To ensure that the WSP is up and running.
            Thread.Sleep(1000);

            ILocalTokenService localTokenService = new LocalTokenService();
            var localToken = localTokenService.GetToken();

            // Retrieve token
            IStsTokenService stsTokenService = new StsTokenServiceCache(TokenServiceConfigurationFactory.CreateConfiguration());
            var securityToken = stsTokenService.GetToken();

            // Call WSP with token
            var client = new HelloWorldClient();

            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(securityToken);

            Console.WriteLine(channelWithIssuedToken.HelloNone("Schultz")); // Even if the protection level is set to 'None' Digst.OioIdws.Wsc ensures that the body is always at least signed.
            Console.WriteLine(channelWithIssuedToken.HelloSign("Schultz"));
            Console.WriteLine(channelWithIssuedToken.HelloEncryptAndSign("Schultz"));

            ////Checking that SOAP faults can be read. SOAP faults are encrypted in Sign and EncryptAndSign mode if no special care is taken.
            try
            {
                channelWithIssuedToken.HelloSignError("Schultz");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //Checking that SOAP faults can be read when only being signed. SOAP faults are only signed if special care is taken.
            try
            {
                channelWithIssuedToken.HelloSignErrorNotEncrypted("Schultz");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }
    }

    public class LocalTokenService : ILocalTokenService
    {
        private X509Certificate2 _certificate;

        public LocalTokenService()
        {
            var thumbprint = ConfigurationManager.AppSettings["LocalTokenCertificateThumbprint"];
            using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);
                _certificate = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, true).Cast<X509Certificate2>().Single();
                store.Close();
            }
        }

        public SecurityToken GetToken()
        {
            var issuer = new Saml2NameIdentifier("Some local issuer");
            var assertion = new Saml2Assertion(issuer)
            {
                Id = new Saml2Id("_" + Guid.NewGuid().ToString("D")),
                SigningCredentials = new X509SigningCredentials(_certificate)
            };
            return new Saml2SecurityToken(assertion);
        }
    }

    public interface ILocalTokenService
    {
        SecurityToken GetToken();
    }
}