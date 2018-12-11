using System;
using System.Threading;
using Digst.OioIdws.Wsc.OioWsTrust;

using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Claims;
using System.IdentityModel.Protocols.WSTrust;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.ServiceModel;
using static System.Console;
using Digst.OioIdws.SecurityTokens;
using Digst.OioIdws.SecurityTokens.Tokens;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;
using System.Xml;
using Digst.OioIdws.Common.Attributes;
using Digst.OioIdws.Common.Logging;
using Digst.OioIdws.Healthcare.Common;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.SamlAttributes;
using Digst.OioIdws.WscLocalSignature.Connected_Services.HelloWorldProxy;
using Digst.OioIdws.Healthcare.SamlAttributes;
using Digst.OioIdws.Healthcare.Wsc;

namespace Digst.OioIdws.WscLocalSignature
{
    class Program
    {
        //public static readonly string MocesCertThumbprint = "c8c97200d5114f436691369e0f4ee4e9c0a0cf9c";
        //public static readonly string ServiceURI = "https://digst.oioidws.wsp:9090/helloworld";



        public static void Main(string[] args)
        {
            WriteLine("--------------------------------------------------------------------------------");
            WriteLine("Scenario: Local signature. Role: client/consumer.");
            WriteLine("--------------------------------------------------------------------------------");


            Logger.Instance.Trace("Starting");

            // Read the configuration for the Web Service Provider (WSP) which hosts the service that we intend to invoke
            var wspUrl = ConfigurationManager.AppSettings["wspUrl"];
            var wspEntityId = ConfigurationManager.AppSettings["wspEntityId"];

            //Retrieve the MOCES certificate for signing the locally issued AUT token
            var mocesCertThumbprint = ConfigurationManager.AppSettings["SubjectCertificateThumbprint"];
            var mocesCert = CertificateUtil.GetCertificate(StoreName.My, StoreLocation.CurrentUser, X509FindType.FindByThumbprint, mocesCertThumbprint);

            var config = SecurityTokenServiceClientConfigurationSection.FromConfiguration();

            
            // The STS
            IOioSecurityTokenServiceClient sts = new StandardSecurityTokenServiceClient(config);


            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Press '1' to invoke claims reflection operation with holder-of-key token.");
                Console.WriteLine("Press '2' to invoke faulting operation.");
                Console.WriteLine("Press '3' to invoke claims reflection operation with bearer token.");
                Console.WriteLine("Press 'q' to exit");
                var c = Console.ReadKey();
                Console.WriteLine();
                Console.WriteLine();

                switch (c.KeyChar)
                {
                    case 'Q':
                    case 'q':
                        return;
                    case '1':
                        try
                        {
                            InvokeClaimsReflectingOperation(sts, mocesCert, wspEntityId, wspUrl, KeyType.HolderOfKey);
                        }
                        catch (Exception x)
                        {
                            WriteLine($"Error invoking service: {x.Message}");
                            Logger.Instance.Error(x.Message, x);
                        }
                        break;
                    case '2':
                        try
                        {
                            InvokeFaultingOperation(sts, mocesCert, wspEntityId, wspUrl);
                        }
                        catch (Exception x)
                        {
                            WriteLine($"Error invoking service: {x.Message}");
                            Logger.Instance.Error(x.Message, x);
                        }
                        break;
                    case '3':
                        try
                        {
                            InvokeClaimsReflectingOperation(sts, mocesCert, wspEntityId, "https://digst.oioidws.wsp:9091/helloworld", KeyType.Bearer);
                        }
                        catch (Exception x)
                        {
                            WriteLine($"Error invoking service: {x.Message}");
                            Logger.Instance.Error(x.Message, x);
                        }
                        break;
                    default:
                        Console.WriteLine($"Unknown option: {c.KeyChar}");
                        break;
                }
            }
        }


        private static void InvokeClaimsReflectingOperation(IOioSecurityTokenServiceClient sts, X509Certificate2 mocesCert, string wspEntityId, string wspUrl, KeyType keyType)
        {
            //Build AUT token to send to STS
            var factory = new HealthcareAuthenticationTokenFactory(TimeSpan.FromMinutes(5));
            var aut = factory.CreateAuthenticationToken(mocesCert, AssuranceLevel.Level3).ToGenericXmlSecurityToken();

            // Exchange the AUT for a BST
            var bst = sts.GetBootstrapTokenFromAuthenticationToken(aut);

            // Exchange the BST for an IDT specific to the WSP
            var claims = new RequestClaimCollection()
            {
                Dialect = "http://docs.oasis-open.org/wsfed/authorization/200706/authclaims",
            };
            
            claims.Add(new RequestClaim(CommonHealthcareAttributes.SystemVersion.Name, true, "0.0"));
            claims.Add(new RequestClaim(CommonHealthcareAttributes.SystemName.Name, true, "Bennys Astralhealing"));
            claims.Add(new RequestClaim(CommonHealthcareAttributes.SystemUsingOrganisationName.Name, true, "Bennys Astralhealing"));
            claims.Add(new RequestClaim(CommonHealthcareAttributes.PatientResourceId.Name, true, "2512484916^^^&1.2.208.176.1.2&ISO"));
            claims.Add(new RequestClaim(CommonHealthcareAttributes.SubjectProviderIdentifier.Name, true, @"<id xmlns=""urn:hl7-org:v3"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:type=""II"" root=""1.2.208.176.1.1"" extension=""8041000016000^Sydvestjysk Sygehus"" assigningAuthorityName=""Sundhedsvæsenets Organisationsregister (SOR)""/>"));

            // Get identity token
            var idt = sts.GetIdentityTokenFromBootstrapToken(bst, wspEntityId, claims, keyType);

            // WSP proxy
            var client = new HelloWorldClient();

            //Create a channel with the issued token
            var channel = client.ChannelFactory.CreateChannelWithIssuedToken(idt, new EndpointAddress(new Uri(wspUrl), EndpointIdentity.CreateDnsIdentity("wsp.oioidws-net.dk TEST (funktionscertifikat)")));

            //Call the service and log the response to console
            WriteLine(channel.HelloSign("Schultz"));

        }


        private static void InvokeFaultingOperation(IOioSecurityTokenServiceClient sts, X509Certificate2 mocesCert, string wspEntityId, string wspUrl)
        {
            //Build AUT token to send to STS
            var factory = new HealthcareAuthenticationTokenFactory(TimeSpan.FromMinutes(5));
            var aut = factory.CreateAuthenticationToken(mocesCert, AssuranceLevel.Level3).ToGenericXmlSecurityToken();

            // Exchange the AUT for a BST
            var bst = sts.GetBootstrapTokenFromAuthenticationToken(aut);

            // Exchange the BST for an IDT specific to the WSP
            var claims = new RequestClaimCollection()
            {
                Dialect = "http://docs.oasis-open.org/wsfed/authorization/200706/authclaims",
            };
            claims.Add(new RequestClaim(CommonHealthcareAttributes.SystemVersion.Name, true, "0.0"));
            claims.Add(new RequestClaim(CommonHealthcareAttributes.SystemName.Name, true, "Bennys Astralhealing"));
            claims.Add(new RequestClaim(CommonHealthcareAttributes.SystemUsingOrganisationName.Name, true, "Bennys Astralhealing"));
            claims.Add(new RequestClaim(CommonHealthcareAttributes.PatientResourceId.Name, true, "2512484916^^^&1.2.208.176.1.2&ISO"));
            claims.Add(new RequestClaim(CommonHealthcareAttributes.SubjectProviderIdentifier.Name, true, @"<id xmlns=""urn:hl7-org:v3"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:type=""II"" root=""1.2.208.176.1.1"" extension=""8041000016000^Sydvestjysk Sygehus"" assigningAuthorityName=""Sundhedsvæsenets Organisationsregister (SOR)""/>"));

            // Get identity token
            var idt = sts.GetIdentityTokenFromBootstrapToken(bst, wspEntityId, claims, KeyType.HolderOfKey);

            // WSP proxy
            var client = new HelloWorldClient();

            //Create a channel with the issued token
            var channel = client.ChannelFactory.CreateChannelWithIssuedToken(idt, new EndpointAddress(new Uri(wspUrl), EndpointIdentity.CreateDnsIdentity("wsp.oioidws-net.dk TEST (funktionscertifikat)")));

            //Checking that SOAP faults can be read. SOAP faults are encrypted in Sign and EncryptAndSign mode if no special care is taken.
            channel.HelloSignError("Schultz");
        }



    }
}