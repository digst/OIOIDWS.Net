using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Threading;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.OioWsTrust.InMemory;
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

            // Read configuration settings for this example
            var inMemoryLocalTokenServiceConfiguration = InMemoryLocalTokenServiceConfigurationFactory.CreateConfiguration();
            var stsTokenServiceConfiguration = TokenServiceConfigurationFactory.CreateConfiguration();

            // The Local Token Service is a "fake" in-memory token service representing some token issuer available
            // on the organizations infrastructure (e.g. an identity provider or local STS).
            ILocalTokenService localTokenService = new InMemoryLocalTokenService(inMemoryLocalTokenServiceConfiguration);

            // The NemLog-in STS
            IStsTokenService nemLoginTokenService = new StsTokenServiceCache(stsTokenServiceConfiguration);

            // The entity ID of the NemLog-in STS. 
            // There are separate entity IDs for each token case, so this is the "local token" entity ID.
            var nemLoginLocalTokenStsEntityId = ConfigurationManager.AppSettings["NemLoginLocalTokenStsEntityId"];

            // To ensure that the WSP is up and running.
            Thread.Sleep(1000);

            // Create the subject (the actual "user") and specify how (bearer or holder-of-key)
            // the recipient of the token can confirm the authenticity of the presenter.
            var subjectConfirmation = SubjectFactory.HolderOfKeySubjectConfirmation(stsTokenServiceConfiguration.ClientCertificate);
            var subject = SubjectFactory.X509Subject(
                employeeRid: "12345678", 
                employeeName: "Benny Bomstærk", 
                organizationCvr: "34051178", 
                organizationName: "Digitaliseringsstyrelsen", 
                subjectConfirmation: subjectConfirmation);

            // Create a local token for the user.
            // In a real system (outside of this example) you will retrieve this token from
            // a local identity provider or local security token service, such as an
            // Active Directory Federation Server (ADFS) on the organization infrastructure.
            var localToken = localTokenService.Issue(
                subject: subject,
                attributes: new Saml2Attribute[]
                {
                    new Saml2Attribute("dk:gov:saml:attribute:AssuranceLevel") { NameFormat = new Uri("urn:oasis:names:tc:SAML:2.0:attrname-format:basic"), Values = { "3" }},
                }, 
                audience: new Uri(nemLoginLocalTokenStsEntityId));

            // We now have a local token identifying our subject, attributes and confirmation method. 
            // The token is issued to the NemLog-in local-token STS as its intended audience.

            // Use NemLog-in STS to retrieve a token for the WSP by passing the local token.
            // The local token service must have been registered with NemLog-in STS
            // so that its entity ID and signing certificate is known and
            // trusted by NemLog-in STS
            var wspToken = nemLoginTokenService.GetTokenWithLocalToken(localToken);

            // We now have a token from NemLog-in STS issued specifically for the web service provider (WSP)
            // Although encrypted so that only the WSP will be able to decrypt, it contains our subject
            // and attributes, and possibly more attributes added by NemLog-in STS

            // Set up a communications channel with the WSP based on the token
            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(wspToken);

            // Invoke a WSP operation which requires neither signing nor encryption
            // (Even if the protection level is set to 'None' Digst.OioIdws.Wsc ensures that the body
            // is always at least signed. The signature will be ignored by the WSP in this case)
            Console.WriteLine(channelWithIssuedToken.HelloNone("Schultz"));

            // Invoke a WSP operation which requires signature but not encryption.
            Console.WriteLine(channelWithIssuedToken.HelloSign("Schultz"));

            // Invoke a WSP operation which requires signature and expects an encrypted message.
            Console.WriteLine(channelWithIssuedToken.HelloEncryptAndSign("Schultz"));

            // Invoke a WSP operation which expects signed requests and which will fail with a signed and encrypted SOAP fault
            // SOAP faults are encrypted in Sign and EncryptAndSign mode if no special care is taken.
            try
            {
                channelWithIssuedToken.HelloSignError("Schultz");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Invoke a WSP operation which expects signed requests and which will fail with a signed but *not* encrypted SOAP fault
            // SOAP faults are unencrypted by WSP only if special care is taken.
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
}