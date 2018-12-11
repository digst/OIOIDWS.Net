using System;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Xml.Linq;
using Digst.OioIdws.Common.Attributes;
using Digst.OioIdws.Healthcare.Common;
using Digst.OioIdws.Healthcare.SamlAttributes;
using Digst.OioIdws.Healthcare.Sts;
using Digst.OioIdws.SamlAttributes;
using Digst.OioIdws.SecurityTokens;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;
using Digst.OioIdws.Wsc.OioWsTrust;
using Digst.OioIdws.WscUnencryptedBearerExample.Connected_Services.HelloWorldProxy;

namespace Digst.OioIdws.WscUnencryptedBearerExample
{

    class Program
    {

        private static X509Certificate2 _wscCert = CertificateUtil.GetCertificate(StoreName.My, StoreLocation.LocalMachine,
            X509FindType.FindByThumbprint, "0e6dbcc6efaaff72e3f3d824e536381b26deecf5");

        private static X509Certificate2 _stsCert = CertificateUtil.GetCertificate(StoreName.My, StoreLocation.LocalMachine,
            X509FindType.FindByThumbprint, "0e6dbcc6efaaff72e3f3d824e536381b26deecf5");

        private static X509Certificate2 _wspCert = CertificateUtil.GetCertificate(StoreName.My, StoreLocation.LocalMachine,
            X509FindType.FindByThumbprint, "1f0830937c74b0567d6b05c07b6155059d9b10c7");

        private static readonly HealthcareSecurityTokenFactory HealthcareSecurityTokenFactory = new HealthcareSecurityTokenFactory(_stsCert, "http://sts.example.org", "https://wsp.oioidws-net.dk", TimeSpan.FromHours(8));


        private static SecurityToken GetToken()
        {
            var hl = XNamespace.Get("urn:dk:healthcare:saml:user_authorization_profile:1.0");


            var userAuthorizationList = new UserAuthorizationList()
            {
                UserAuthorizations = new Collection<UserAuthorization>()
                {
                    new UserAuthorization()
                    {
                        AuthorizationCode = "341KY",
                        EducationCode = "7170",
                        EducationType = "Læge"
                    }
                }
            };


            var purposeOfUse = PurposeOfUse.Treatment;
            
            var holderOfKeyToken = HealthcareSecurityTokenFactory.CreateServiceToken(_wscCert, new Uri("https://digst.oioidws.wsp:9090/helloworld"), TimeSpan.FromHours(8), null);

            var mgr = new AttributeStatementAttributeAdapter(holderOfKeyToken.Assertion.Statements.OfType<Saml2ComplexAttributeStatement>().Single());

            mgr.SetValue(CommonHealthcareAttributes.PurposeOfUse, purposeOfUse);
            mgr.SetValue(CommonHealthcareAttributes.UserAuthorizations, userAuthorizationList);
            mgr.SetValue(CommonOioAttributes.AssuranceLevel, AssuranceLevel.Level3);

            return holderOfKeyToken;
        }

        static void Main(string[] args)
        {
            //XmlConfigurator.Configure();


            var token = GetToken();
            var handler = new ExtendedSaml2SecurityTokenHandler();

            Console.WriteLine(handler.WriteToken(token));

            Thread.Sleep(5000);

            // Call WSP with token
            var client = new HelloWorldClient();

            var channel = client.ChannelFactory.CreateChannelWithIssuedToken(token);
            

            Console.WriteLine(channel.HelloNone("Schultz")); // Even if the protection level is set to 'None' Digst.OioIdws.Wsc ensures that the body is always at least signed.
            Console.WriteLine(channel.HelloSign("Schultz"));
            Console.WriteLine(channel.HelloEncryptAndSign("Schultz"));

        }
    }


}