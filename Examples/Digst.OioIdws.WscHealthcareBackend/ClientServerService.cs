using System;
using System.Configuration;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Digst.OioIdws.Examples.Healthcare.ClientServer.Backend.Connected_Services.HelloWorldProxy;
using Digst.OioIdws.Examples.Healthcare.ClientServer.Contracts;
using Digst.OioIdws.Healthcare.SamlAttributes;
using Digst.OioIdws.Healthcare.Wsc;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.OioWsTrust.TokenCache;
using static System.Console;

namespace Digst.OioIdws.Examples.Healthcare.ClientServer.Backend
{
    /// <summary>
    /// Example application server of a client/server scenario. The server is invoked by an application client. The server is itself a service consumer of an identity based (IDWS) web service.
    /// </summary>
    public class ClientServerService : IClientServerService
    {
        private ISecurityTokenServiceClient mySts;
        private ITokenCache myTokenCache;

        public HelloResponseMessage HelloClientServer(HelloRequestMessage requestMessage)
        {
            WriteLine($"Service invoked with the following greeting: \"{requestMessage.Greeting}\"");
            WriteLine($"AUT token is \"{requestMessage.AutToken}\"");

            // Read the configuration for the Web Service Provider (WSP) which hosts the service that we intend to invoke
            var wspUrl = ConfigurationManager.AppSettings["wspUrl"];
            var wspEntityId = ConfigurationManager.AppSettings["wspEntityId"];

            // Recreate the token from the XML received as part of the request message
            // How to do this will depend on how the token was transported from the client to the server. This is just one example on how to do it.
            var xmlToken = new XmlDocument();
            xmlToken.LoadXml(requestMessage.AutToken.SerializedToken);
            var aut = new GenericXmlSecurityToken(
                xmlToken.DocumentElement,
                null,
                requestMessage.AutToken.NotBefore,
                requestMessage.AutToken.NotOnOrAfter,
                new LocalIdKeyIdentifierClause(requestMessage.AutToken.TokenId),
                new KeyNameIdentifierClause(requestMessage.AutToken.TokenId),
                null);

            IOioSecurityTokenServiceClient sts = new StandardSecurityTokenServiceClient(SecurityTokenServiceClientConfigurationSection.FromConfiguration());

            // Exchange the AUT for a BST
            var bst = sts.GetBootstrapTokenFromAuthenticationToken(aut);

            // Initialize a claims request collection
            var claims = new RequestClaimCollection()
            {
                Dialect = "http://docs.oasis-open.org/wsfed/authorization/200706/authclaims",
            };
            claims.Add(new RequestClaim(CommonHealthcareAttributes.SystemVersion.Name, true, "0.0"));
            claims.Add(new RequestClaim(CommonHealthcareAttributes.SystemName.Name, true, "Bennys Astralhealing"));
            claims.Add(new RequestClaim(CommonHealthcareAttributes.SystemUsingOrganisationName.Name, true, "Bennys Astralhealing"));
            claims.Add(new RequestClaim(CommonHealthcareAttributes.PatientResourceId.Name, true, "2512484916^^^&1.2.208.176.1.2&ISO"));
            claims.Add(new RequestClaim(CommonHealthcareAttributes.SubjectProviderIdentifier.Name, true, @"<id xmlns=""urn:hl7-org:v3"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:type=""II"" root=""1.2.208.176.1.1"" extension=""8041000016000^Sydvestjysk Sygehus"" assigningAuthorityName=""Sundhedsvæsenets Organisationsregister (SOR)""/>"));

            // Get a service and subject specific identity token (IDT) from the STS
            var idt = sts.GetIdentityTokenFromBootstrapToken(bst, wspEntityId, claims, KeyType.HolderOfKey);

            // Create a channel for the service we want to invoke
            var wspClient = new HelloWorldClient();
            var channel = wspClient.ChannelFactory.CreateChannelWithIssuedToken(idt);

            // Invoke WSP service
            var answers = new StringBuilder();
            answers.AppendLine();
            answers.AppendLine(channel.HelloSign("Schultz"));
            answers.AppendLine(channel.HelloNone("Schultz"));

            return new HelloResponseMessage()
            {
                Answer = $"I am happy to meet you!." + answers.ToString(),
            };
        }


    }
}