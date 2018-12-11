using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Digst.OioIdws.Common.Attributes;
using Digst.OioIdws.Examples.Healthcare.ClientServer.Contracts;
using Digst.OioIdws.Healthcare.Wsc;
using Digst.OioIdws.SamlAttributes;
using Digst.OioIdws.SecurityTokens;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;
using static System.Console;


namespace Digst.OioIdws.Examples.Healthcare.ClientServer.Frontend
{
    class Program
    {
        public static readonly string MocesCertThumbprint = ConfigurationManager.AppSettings["MocesCertThumbprint"];
        public static readonly string StsEntityId = ConfigurationManager.AppSettings["HealthcareStsEntityId"];
        public static readonly string ClientEntityId = ConfigurationManager.AppSettings["ClientEntityId"];

        static void Main(string[] args)
        {



            // Start the client
            var channelFactory = new ChannelFactory<IClientServerService>(new WSHttpBinding(), new EndpointAddress("http://localhost:9901"));
            var proxy = channelFactory.CreateChannel();

            // Locate the MOCES certificate in the user's certificate store
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            var moces = store.Certificates.Find(X509FindType.FindByThumbprint, MocesCertThumbprint, true).OfType<X509Certificate2>().Single();
            store.Close();

            // Create an authentication token (AUT) using the employee MOCES certificate
            // Note: Depending on how the certificate was installed, this may cause a prompt from the security provider asking the user to consent
            var autToken = CreateMocesAuthenticationToken(moces);

            // Serialize the token. This enables us to send the token across the wire to the backend
            var tokenHandler = new ExtendedSaml2SecurityTokenHandler();
            var serializedToken = tokenHandler.WriteToken(autToken);

            while(true)
            {
                WriteLine("--------------------------------------------------------------------------------");
                WriteLine("Client/server scenario: Example frontend");
                WriteLine("--------------------------------------------------------------------------------");

                Console.WriteLine();
                Console.WriteLine("Press '1' to invoke claims reflection operation.");
                Console.WriteLine("Press 'q' to exit");
                var c = Console.ReadKey();
                Console.WriteLine();
                Console.WriteLine();

                switch (c.KeyChar)
                {
                    case '1':
                        var aut = CreateMocesAuthenticationToken(moces);
                        var response = proxy.HelloClientServer(new HelloRequestMessage()
                        {
                            AutToken = new AuthenticationToken()
                            {
                                SerializedToken = serializedToken,
                                NotBefore = autToken.Assertion.Conditions.NotBefore.Value,
                                NotOnOrAfter = autToken.Assertion.Conditions.NotOnOrAfter.Value,
                                TokenId = autToken.Assertion.Id.Value,
                            },
                            Greeting = "Hello, this is frontend!",
                        });
                        WriteLine($"Response from backend: \"{response.Answer}\"");
                        break;
                    case 'Q':
                    case 'q':
                        return;
                    default:
                        WriteLine("Unrecognized option. Please try again.");
                        break;
                }
            }
        }


        // Creates an authentication token (AUT) based on a employee (MOCES) certificare
        private static Saml2SecurityToken CreateMocesAuthenticationToken(X509Certificate2 certificate)
        {

            var tokenFactory = new HealthcareAuthenticationTokenFactory();
            var token = tokenFactory.CreateAuthenticationToken(certificate, AssuranceLevel.Level3);
            return token;
        }
    }
}
