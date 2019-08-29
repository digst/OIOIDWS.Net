using System;
using System.Configuration;
using System.Web;
using System.Web.UI;
using dk.nita.saml20.config;
using dk.nita.saml20.identity;
using dk.nita.saml20.Logging;
using dk.nita.saml20.protocol;
using System.IdentityModel.Tokens;
using System.Xml;
using System.IO;
using System.Net.Http;
using System.ServiceModel.Security;
using System.Text;
//using Digst.OioIdws.OioWsTrust;
//using Digst.OioIdws.Rest.Client;
//using Digst.OioIdws.Wsc.OioWsTrust;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.Wsc.OioWsTrust;
using Digst.OioIdws.Rest.Client;
using Digst.OioIdws.SamlAttributes;
using Digst.OioIdws.SamlAttributes.AttributeAdapters;
using WebsiteDemo.Service_References.HelloWorldProxy;

namespace WebsiteDemo
{
    public partial class WebForm1 : Page
    {
        protected static string ServiceResponse { get; set; }

        protected  void Page_PreInit(object sender, EventArgs e)
        {
            
        }
        protected void Page_Load(object sender, EventArgs e)
        {            
            Title = "My page on SP " + SAML20FederationConfig.GetConfig().ServiceProvider.ID;

            if (Request.QueryString["action"] == "sso")
            {
                // Example of logging required by the requirements BSA6/SSO6 ("Id of internal account that is matched to SAML Assertion")
                // Since FormsAuthentication is used in this sample, the user name to log can be found in context.User.Identity.Name.
                // This user will not be set until after a new redirect, so unfortunately we cannot just log it in our LogAction.LoginAction
                AuditLogging.logEntry(Direction.IN, Operation.LOGIN, "ServiceProvider login",
                                      "SP internal user id: " +
                                      (Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name : "(not logged in)"));
            }

        }

        protected void Btn_Relogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("login.ashx?" + Saml20SignonHandler.IDPForceAuthn + "=true&ReturnUrl=" + HttpContext.Current.Request.Url.AbsolutePath);
        }

        protected void Btn_Passive_Click(object sender, EventArgs e)
        {
            Response.Redirect("login.ashx?" + Saml20SignonHandler.IDPIsPassive + "=true&ReturnUrl=" + HttpContext.Current.Request.Url.AbsolutePath);
        }

        protected void Btn_ReloginNoForceAuthn_Click(object sender, EventArgs e)
        {
            Response.Redirect("login.ashx?ReturnUrl=" + HttpContext.Current.Request.Url.AbsolutePath);
        }

        protected void Btn_Logoff_Click(object sender, EventArgs e)
        {
            // Example of logging required by the requirements SLO1 ("Id of internal account that is matched to SAML Assertion")
            // Since FormsAuthentication is used in this sample, the user name to log can be found in context.User.Identity.Name
            AuditLogging.logEntry(Direction.OUT, Operation.LOGOUTREQUEST, "ServiceProvider logoff requested, local user id: " + HttpContext.Current.User.Identity.Name);
            Response.Redirect("logout.ashx");
        }

        protected void Btn_CallService_Click(object sender, EventArgs e)
        {
            ServiceResponse = "";

            ServiceResponse = RunSoap(null);

            Response.Redirect("MyPage.aspx");
        }

        protected void Btn_CallServiceWithToken_Click(object sender, EventArgs e)
        {
            ServiceResponse = "";

            var config = TokenServiceConfigurationFactory.CreateConfiguration();
            var bootstrapToken = GetBootstrapToken(new X509SecurityToken(config.WscCertificate));

            ServiceResponse = RunSoap(bootstrapToken);

            Response.Redirect("MyPage.aspx");
        }

        private string RunSoap(SecurityToken bootstrapToken)
        {
            // Retrieve token
            ISecurityTokenServiceClient stsTokenService = new LocalSecurityTokenServiceClient(TokenServiceConfigurationFactory.CreateConfiguration(), null);
            SecurityToken securityToken = null;

            // Call WSP with token
            var client = new HelloWorldClient();


            if (bootstrapToken != null)
            {
                securityToken = stsTokenService.GetIdentityTokenFromBootstrapToken(bootstrapToken, client.Endpoint.Address.Uri.AbsoluteUri, KeyType.HolderOfKey); 
            }
            else
            {
                securityToken = stsTokenService.GetServiceToken(client.Endpoint.Address.Uri.AbsoluteUri, KeyType.HolderOfKey); 
            }

            // enable revocation check if not white listed at Nets, don't do this in production!
            //client.ClientCredentials.ServiceCertificate.Authentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck;

            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(securityToken);

            return channelWithIssuedToken.HelloSign("Oiosaml-net.dk TEST");
        }

        protected void Btn_CallRestService_Click(object sender, EventArgs e)
        {
            ServiceResponse = "";

            ServiceResponse = RunRest(null);

            Response.Redirect("MyPage.aspx");
        }

        protected void Btn_CallRestServiceWithToken_Click(object sender, EventArgs e)
        {
            ServiceResponse = "";

            var config = TokenServiceConfigurationFactory.CreateConfiguration();

            var bootstrapToken = GetBootstrapToken(new X509SecurityToken(config.WscCertificate));

            ServiceResponse = RunRest(bootstrapToken);

            Response.Redirect("MyPage.aspx");
        }

        private static string RunRest(SecurityToken bootstrapToken)
        {
            //configures the internal logger for OIO WS-TRUST communication
            //LoggerFactory.SetLogger(new ConsoleLogger());

            var settings = new OioIdwsClientSettings
            {
                ClientCertificate =
                    CertificateUtil.GetCertificate(ConfigurationManager.AppSettings["ClientCertificate"]),
                AudienceUri = new Uri(ConfigurationManager.AppSettings["AudienceUri"]),
                AccessTokenIssuerEndpoint = new Uri(ConfigurationManager.AppSettings["AsEndpoint"]),
                SecurityTokenService = new OioIdwsStsSettings
                {
                    Certificate = CertificateUtil.GetCertificate(ConfigurationManager.AppSettings["StsCertificate"]),
                    EndpointAddress = new Uri(ConfigurationManager.AppSettings["StsEndpointAddress"]),
                    TokenLifeTime =
                        TimeSpan.FromSeconds(int.Parse(ConfigurationManager.AppSettings["TokenLifeTimeInSeconds"])),
                    UseTokenCache = false
                }
            };

            var idwsClient = bootstrapToken != null ? new OioIdwsClient(settings, bootstrapToken) : new OioIdwsClient(settings);

            var httpClient = new HttpClient(idwsClient.CreateMessageHandler());

            //first invocation - security token is retrieved and stored in the AS, access token cached by client
            var response = httpClient.GetAsync(ConfigurationManager.AppSettings["WspTestEndpointAddress"]).Result;
            return response.EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result;
        }

        private static SecurityToken GetBootstrapToken(SecurityToken proofToken)
        {

            var attributeManager = new ClaimsAttributeAdapter(Saml20Identity.Current.Claims);
            var bstXmlDocument = attributeManager.GetValue(CommonOioAttributes.DiscoveryEpr);

            var nsmgr = new XmlNamespaceManager(new NameTable());
            nsmgr.AddNamespace("", "urn:oasis:names:tc:SAML:2.0:assertion");

            var id = ((XmlAttribute)bstXmlDocument.SelectSingleNode("/Assertion/@ID", nsmgr)).Value;
            var notBefore = ((XmlAttribute)bstXmlDocument.SelectSingleNode("/Assertion/Conditions/@NotBefore", nsmgr)).Value;
            var notOnOrAfter = ((XmlAttribute)bstXmlDocument.SelectSingleNode("/Assertion/Conditions/@NotOnOrAfter", nsmgr)).Value;

            var token = new GenericXmlSecurityToken(bstXmlDocument.DocumentElement, proofToken, DateTime.Parse(notBefore).ToUniversalTime() , DateTime.Parse(notOnOrAfter).ToUniversalTime(), new LocalIdKeyIdentifierClause(id), null, null);

            return token;
        }
    }
}
