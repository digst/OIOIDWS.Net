using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.OioWsTrust.InMemory;
using Digst.OioIdws.WscLocalTokenExample;

namespace DK.Gov.Oio.Idws.IntegrationTests.TokenAcquisition
{
    /// <summary>
    /// Local token scenario. Requests a token from the STS with a token issued by a local STS (in-memory)
    /// </summary>
    public class LocalTokenScenario : ITokenAcquisitionScenario
    {
        private readonly IStsTokenService _tokenService;
        private readonly X509Certificate2 _holderOfKeyCertificate;
        private readonly ILocalTokenService _localTokenService;

        /// <summary>
        /// Instantiate the Local Token Scenario
        /// </summary>
        /// <param name="tokenService">The STS to acquire the WSP-required token from.</param>
        /// <param name="holderOfKeyCertificate">Certificate used to generate the SubjectConfirmation</param>
        public LocalTokenScenario(IStsTokenService tokenService, X509Certificate2 holderOfKeyCertificate)
        {
            _tokenService = tokenService;
            _holderOfKeyCertificate = holderOfKeyCertificate;

            var inMemoryLocalTokenServiceConfiguration = InMemoryLocalTokenServiceConfigurationFactory.CreateConfiguration();
            _localTokenService = new InMemoryLocalTokenService(inMemoryLocalTokenServiceConfiguration);
        }
        
        public SecurityToken AcquireTokenFromSts()
        {
            var localToken = AcquireTokenFromLocalSts();
            return _tokenService.GetTokenWithLocalToken(localToken);
        }

        private SecurityToken AcquireTokenFromLocalSts()
        {
            var subjectConfirmation = SubjectFactory.HolderOfKeySubjectConfirmation(_holderOfKeyCertificate);
            var subject = SubjectFactory.X509Subject(
                employeeRid: "12345678", 
                employeeName: "Benny Bomstærk", 
                organizationCvr: "34051178", 
                organizationName: "Digitaliseringsstyrelsen", 
                subjectConfirmation: subjectConfirmation);
            
            // The entity ID of the NemLog-in STS.
            // There are separate entity IDs for each token case, so this is the "local token" entity ID.
            var nemLoginLocalTokenStsEntityId = ConfigurationManager.AppSettings["NemLoginLocalTokenStsEntityId"];
            
            return _localTokenService.Issue(
                subject: subject,
                attributes: new[]
                {
                    new Saml2Attribute("dk:gov:saml:attribute:AssuranceLevel") { NameFormat = new Uri("urn:oasis:names:tc:SAML:2.0:attrname-format:basic"), Values = { "3" }},
                }, 
                audience: new Uri(nemLoginLocalTokenStsEntityId));
        }
    }
}