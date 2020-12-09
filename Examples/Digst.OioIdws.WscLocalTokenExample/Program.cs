using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security.Tokens;
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

            var stsTokenServiceConfiguration = TokenServiceConfigurationFactory.CreateConfiguration();

            ILocalTokenService localTokenService = new LocalTokenService();
            var localToken = localTokenService.GetHolderOfKeyToken(stsTokenServiceConfiguration.ClientCertificate);
            //var localToken = localTokenService.GetBearerToken();

            // Retrieve token

            IStsTokenService stsTokenService = new StsTokenServiceCache(stsTokenServiceConfiguration);
            var securityToken = stsTokenService.GetTokenWithLocalToken(localToken);

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


        public SecurityToken GetHolderOfKeyToken(X509Certificate2 heldKey)
        {
            var subjectConfirmation = new Saml2SubjectConfirmation(new Uri("urn:oasis:names:tc:SAML:2.0:cm:holder-of-key"))
            {
                SubjectConfirmationData = new Saml2SubjectConfirmationData()
                {
                    KeyIdentifiers = {new SecurityKeyIdentifier(new X509RawDataKeyIdentifierClause(heldKey))}
                }
            };

            return GetTokenInternal(subjectConfirmation);
        }

        public SecurityToken GetTokenInternal(Saml2SubjectConfirmation subjectConfirmation)
        {
            var issuer = new Saml2NameIdentifier("https://sts.oioidws-net.dk");
            var assertion = new Saml2Assertion(issuer)
            {
                Id = new Saml2Id("_" + Guid.NewGuid().ToString("D")),
                SigningCredentials = new X509SigningCredentials(_certificate),
                //Subject = new Saml2Subject(
                //    new Saml2NameIdentifier(
                //        "flollop",
                //        new Uri("urn:oasis:names:tc:SAML:2.0:nameid-format:persistent")))
                //{
                //    SubjectConfirmations =
                //    {
                //        subjectConfirmation
                //    }
                //},
                Subject = new Saml2Subject(
                    new Saml2NameIdentifier(
                        "C=DK,O=Økonomistyrelsen // CVR:34051178,CN=Morten Mortensen,Serial=CVR:34051178-RID:93947552",
                        new Uri("urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName")))
                {
                    SubjectConfirmations =
                    {
                        subjectConfirmation
                    }
                },
                IssueInstant = DateTime.UtcNow,
                Conditions = new Saml2Conditions()
                {
                    AudienceRestrictions = { new Saml2AudienceRestriction(new Uri("https://local.sts.nemlog-in.dk/")) },
                    NotBefore = DateTime.UtcNow.AddMinutes(-1),
                    NotOnOrAfter = DateTime.UtcNow.AddMinutes(11),
                },
                Statements =
                {
                    new Saml2AttributeStatement()
                    {
                        Attributes =
                        {
                            new Saml2Attribute("dk:gov:saml:attribute:AssuranceLevel") { NameFormat = new Uri("urn:oasis:names:tc:SAML:2.0:attrname-format:basic"), Values = { "3" }},
                            //new Saml2Attribute("dk:gov:saml:attribute:SpecVer") { NameFormat = new Uri("urn:oasis:names:tc:SAML:2.0:attrname-format:basic"), Values = { "DK-SAML-2.0" }},
                        }
                    }
                }
            };

            return new Saml2SecurityToken(assertion);
        }

        public SecurityToken GetBearerToken()
        {
            var subjectConfirmation = new Saml2SubjectConfirmation(new Uri("urn:oasis:names:tc:SAML:2.0:cm:bearer"));
            return GetTokenInternal(subjectConfirmation);
        }
    }

    public interface ILocalTokenService
    {
        SecurityToken GetHolderOfKeyToken(X509Certificate2 heldKey);

        SecurityToken GetBearerToken();
    }
}