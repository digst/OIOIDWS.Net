using System;
using System.Threading;

using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.Wsc.OioWsTrust;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using static System.Console;
using Digst.OioIdws.SecurityTokens;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;
using System.Xml;
using System.ServiceModel.Security;
using Digst.OioIdws.Common.Attributes;
using Digst.OioIdws.Healthcare.Wsc;
using Digst.OioIdws.WscLocalSignatureBearerToken.Connected_Services.HelloWorldProxy;

namespace Digst.OioIdws.WscLocalSignatureBearerToken
{
    class Program
    {
        public static readonly string MocesCertThumbprint = "c8c97200d5114f436691369e0f4ee4e9c0a0cf9c";
        public static readonly string ServiceURI = "https://digst.oioidws.wsp:9090/helloworld";

        static void Main(string[] args)
        {
            WriteLine("--------------------------------------------------------------------------------");
            WriteLine("This console application represents the client ");
            WriteLine("of an imaginary Consumer-SecurityTokenService-Provider system. ");
            WriteLine("--------------------------------------------------------------------------------");

            //Read configuration for SecurityTokenService client found in App.config <oioIdwsWcfConfiguration> section
            var tsConf = TokenServiceConfigurationFactory.CreateConfiguration();

            //Retrieve the MOCES certificate for signing the locally issued AUT token
            var mocesCert = CertificateUtil.GetCertificate(StoreName.My, StoreLocation.CurrentUser, X509FindType.FindByThumbprint, MocesCertThumbprint);

            //To ensure that the WSP is up and running.
            Thread.Sleep(3000);

            //Build AUT token to send to STS (ActAs)
            var tokenFactory = new HealthcareAuthenticationTokenFactory();
            var autToken = tokenFactory.CreateAuthenticationToken(mocesCert, AssuranceLevel.Level3);

            //As the <ComplexSamlAttribute> is depending on the ExtendenSaml2SecurityTokenHandler we need to transform it 
            //to XmlElement of a GenericXmlSecurityToken that can be used in a call to the STS for an IdentityToken.
            //In the config this line: <add type = "Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken.ExtendedSaml2SecurityTokenHandler, Digst.OioIdws.SecurityTokens" /> 
            //should fix this but WCF seems to ignore it.
            var extendedSaml2SecurityTokenHandler = new ExtendedSaml2SecurityTokenHandler();
            var serializedToken = extendedSaml2SecurityTokenHandler.WriteToken(autToken);

            var xmlToken = new XmlDocument();
            xmlToken.LoadXml(serializedToken);

            var genericXmlSecurityToken = new GenericXmlSecurityToken(
                xmlToken.DocumentElement,
                null,
                autToken.ValidFrom,
                autToken.ValidTo,
                new LocalIdKeyIdentifierClause(autToken.Id),
                new KeyNameIdentifierClause(autToken.Id),
                null
                );

            // WSP proxy
            var client = new HelloWorldClient();

            // Retrieve token
            ISecurityTokenServiceClient stsTokenService = new LocalSecurityTokenServiceClient(tsConf, null);
            var idToken = stsTokenService.GetIdentityTokenFromBootstrapToken(genericXmlSecurityToken, client.Endpoint.Address.Uri.AbsoluteUri.ToLower(), KeyType.Bearer);

            //Create a channel with the issues ´token
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(idToken);

            //Call the service and log the response to console
            WriteLine(channelWithIssuedToken.HelloNone("Schultz"));

            //Checking that SOAP faults can be read. SOAP faults are encrypted in Sign and EncryptAndSign mode if no special care is taken.
            //try
            //{
            //    channelWithIssuedToken.HelloSignError("Schultz");
            //}
            //catch (Exception e)
            //{
            //    WriteLine(e.Message);
            //}

            ReadKey();
        }
    }
}