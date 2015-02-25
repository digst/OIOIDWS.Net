using System.IdentityModel.Tokens;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Digst.OioIdws.TestWebServiceConsumer.WebServiceProducerProxy;
using log4net.Config;

namespace Digst.OioIdws.TestWebServiceConsumer
{
    class Program
    {
        private static void Main(string[] args)
        {
            // Setup Log4Net configuration by loading it from configuration file.
            XmlConfigurator.Configure();

            // Get Token
            ITokenService tokenService = new TokenService();
            var issuedToken = tokenService.GetToken();

            // Create binding
            var binding = new WS2007FederationHttpBinding(WSFederationHttpSecurityMode.TransportWithMessageCredential);
            binding.Security.Message.EstablishSecurityContext = false;
            // This setting is needed in order to use the BearerKey communication paradigme instead of HolderOfKey which is default.
            // HolderOfKey ensures that only the right client can use the token, whereas BearerKey can be used by any client that gets a hold of the token.
            // BearerKey is therefore in theory vulnerable to the Man In The Middle attack. 
            // HTTPS can be used to minimize the risk that others get a hold of the token greatly. Hence, HTTPS is used in the communication with STS and WSP.
            binding.Security.Message.IssuedKeyType = SecurityKeyType.BearerKey;

            // Change binding to use SOAP 1.1
            var customBinding = new CustomBinding(binding.CreateBindingElements());
            var textMessageEncodingBindingElement =
                customBinding.Elements.OfType<TextMessageEncodingBindingElement>().Single();
            textMessageEncodingBindingElement.MessageVersion = MessageVersion.Soap11WSAddressing10;

            // Set up channel factory
            var channelFactory = new ChannelFactory<ISecurityTokenServiceMessageEcho>(customBinding,
                new EndpointAddress("https://securetokenwsp.test-nemlog-in.dk/SecurityTokenServiceMessageEcho.svc"));
            
            // Call WSP
            var securityTokenServiceMessageEcho = channelFactory.CreateChannelWithIssuedToken(issuedToken);
            securityTokenServiceMessageEcho.IssueToken(Message.CreateMessage(MessageVersion.Soap11WSAddressing10,
                "http://nemlog-in.dk/securitytokenservices/echo/Issue", "Hello World"));

        }
    }
}
