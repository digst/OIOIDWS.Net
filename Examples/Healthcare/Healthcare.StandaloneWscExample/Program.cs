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
using System.ServiceModel.Description;
using System.Text;
using static System.Console;
using Digst.OioIdws.SecurityTokens;
using Digst.OioIdws.SecurityTokens.Tokens;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;
using System.Xml;
using Digst.OioIdws.Common.Logging;
using Digst.OioIdws.Healthcare.Common;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.SamlAttributes;
using Digst.OioIdws.Healthcare.SamlAttributes;
using Digst.OioIdws.Healthcare.Wsc;
using Digst.OioIdws.SamlAttributes.AttributeAdapters;

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
                            WriteLine($"Error invoking service: {GetExceptionMessage(x)}");
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

        private static string GetExceptionMessage(Exception x)
        {
            var s = new StringBuilder();
            while (x != null)
            {
                s.AppendLine(x.Message);
                x = x.InnerException;
            }
            return s.ToString();
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

            var claimsAdapter = new RequestClaimCollectionAttributeAdapter(claims, false);

            claims.Add(new RequestClaim(CommonHealthcareAttributes.SystemVersion.Name, true, "0.0"));
            claims.Add(new RequestClaim(CommonHealthcareAttributes.SystemName.Name, true, "Bennys Astralhealing"));
            claims.Add(new RequestClaim(CommonHealthcareAttributes.SystemUsingOrganisationName.Name, true, "Bennys Astralhealing"));
            claims.Add(new RequestClaim(CommonHealthcareAttributes.PatientResourceId.Name, true, "2512484916^^^&1.2.208.176.1.2&ISO"));
            claims.Add(new RequestClaim(CommonHealthcareAttributes.SubjectProviderIdentifier.Name, true, @"<id xmlns=""urn:hl7-org:v3"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:type=""II"" root=""1.2.208.176.1.1"" extension=""8041000016000^Sydvestjysk Sygehus"" assigningAuthorityName=""Sundhedsvæsenets Organisationsregister (SOR)""/>"));

            // Get identity token
            var idt = sts.GetIdentityTokenFromBootstrapToken(bst, wspEntityId, keyType, claims);

            // WSP proxy
            // var client = new HelloWorldClient();
            var fmk = new MedicineCard.MedicineCardPortTypeClient();
            
            fmk.ClientCredentials.ServiceCertificate.DefaultCertificate = CertificateUtil.GetCertificate(StoreName.My, StoreLocation.CurrentUser, X509FindType.FindByThumbprint, "0e6dbcc6efaaff72e3f3d824e536381b26deecf5");
            fmk.ClientCredentials.UseIdentityConfiguration = true;

            //Create a channel with the issued token
            // var channel = client.ChannelFactory.CreateChannelWithIssuedToken(idt, new EndpointAddress(new Uri(wspUrl), EndpointIdentity.CreateDnsIdentity("wsp.oioidws-net.dk TEST (funktionscertifikat)")));
            var fmkChannel = fmk.ChannelFactory.CreateChannelWithIssuedToken(idt,
                new EndpointAddress(
                    new Uri("https://test1.fmk.netic.dk/proxy/services/fmk_xua_144"),
                    EndpointIdentity.CreateDnsIdentity("Oiosaml-net.dk TEST (funktionscertifikat)"))
                );

            //Call the service and log the response to console
            // WriteLine(channel.HelloSign("Schultz"));
            WriteLine(fmkChannel.GetMedicineCard_2015_01_01(new MedicineCard.GetMedicineCardRequest_2015_01_01()
            {
                GetMedicineCardRequest = new MedicineCard.GetMedicineCardRequestType()
                {
                    DateTime = new[] { DateTime.Now },
                    PersonIdentifier = "1111111118"
                },
            }));

        }


    }
}