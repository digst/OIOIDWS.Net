using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using Digst.OioIdws.OioWsTrust;

namespace DK.Gov.Oio.Idws.IntegrationTests.TokenAcquisition
{
    /// <summary>
    /// Local token scenario. Requests a token from the STS with a token issued by a local STS (in-memory)
    /// </summary>
    public class LocalTokenScenario : ITokenAcquisitionScenario
    {
        private readonly LocalStsConfiguration _configuration;
        private readonly IStsTokenService _tokenService;

        /// <summary>
        /// Instantiate the Local Token Scenario
        /// </summary>
        /// <param name="configuration">Configuration for the local token service.</param>
        /// <param name="tokenService">The STS to acquire the WSP-required token from.</param>
        public LocalTokenScenario(LocalStsConfiguration configuration, IStsTokenService tokenService)
        {
            _configuration = configuration;
            _tokenService = tokenService;
        }
        
        public SecurityToken AcquireTokenFromSts()
        {
            var localToken = AcquireTokenFromLocalSts();
            return _tokenService.GetTokenWithLocalToken(localToken);
        }

        /// <summary>
        /// Acquire a token from a simulated in-memory local security token service.
        /// </summary>
        private SecurityToken AcquireTokenFromLocalSts()
        {
            var subjectConfirmation = CreateHolderOfKeySubjectConfirmation(_configuration.HolderOfKeyCertificate);
            var subject = CreateX509Subject(
                employeeRid: "12345678", 
                employeeName: "Benny Bomstærk", 
                organizationCvr: "34051178", 
                organizationName: "Digitaliseringsstyrelsen", 
                subjectConfirmation: subjectConfirmation);
            
            var issuer = new Saml2NameIdentifier(_configuration.EntityId);
            var assertion = new Saml2Assertion(issuer)
            {
                Id = new Saml2Id("_" + Guid.NewGuid().ToString("D")),
                SigningCredentials = new X509SigningCredentials(_configuration.SigningCertificate),
                Subject = subject,
                IssueInstant = DateTime.UtcNow,
                Conditions = new Saml2Conditions
                {
                    NotBefore = DateTime.UtcNow,
                    NotOnOrAfter = DateTime.UtcNow.AddMinutes(5),
                },
            };

            assertion.Conditions.AudienceRestrictions.Add(
                new Saml2AudienceRestriction(new Uri(_configuration.NemLoginLocalTokenStsEntityId)));
            
            assertion.Statements.Add(new Saml2AttributeStatement(new[]
            {
                new Saml2Attribute("dk:gov:saml:attribute:AssuranceLevel") { NameFormat = new Uri("urn:oasis:names:tc:SAML:2.0:attrname-format:basic"), Values = { "3" }},
            }));

            return new Saml2SecurityToken(assertion);
        }
        
        /// <summary>
        /// Generates an X509Subject in accordance with the danish certificate policy standard.
        /// This is a crude method which does not validate the input parameters. Using characters
        /// such as , (comma) or = (equals) will almost certainly generate invalid subject names.
        /// It is generally NOT advisable to synthesize X509 subject names like this in a production
        /// environment. Instead the X509 subject name should be retrieved through some other means
        /// e.g. by reading it from an employee (MOCES) certificate or retrieving it from a service.
        /// </summary>
        /// <param name="employeeRid">The employee RID (role-identifier). Uniquely identifies the employee within the organization.</param>
        /// <param name="employeeName">Name of the employee.</param>
        /// <param name="organizationCvr">The organization CVR number.</param>
        /// <param name="organizationName">Name of the organization.</param>
        /// <param name="subjectConfirmation">The desired subject confirmation method.</param>
        /// <param name="country">The country.</param>
        private Saml2Subject CreateX509Subject(string employeeRid, string employeeName, string organizationCvr, string organizationName, Saml2SubjectConfirmation subjectConfirmation, string country = "DK")
        {
            var identifier = $"C={country},O={organizationName} // CVR:{organizationCvr},CN={employeeName},Serial=CVR:{organizationCvr}-RID:{employeeRid}";
            var nameId = new Saml2NameIdentifier(identifier, new Uri("urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName"));
            return new Saml2Subject(nameId) { SubjectConfirmations = { subjectConfirmation } };
        }

        /// <summary>
        /// Get a holder-of-key confirmation method based on a certificate.
        /// </summary>
        /// <param name="heldKey">The held key. The presenter/caller of a service must prove
        /// access to the private key of this certificate, e.g. by signing the request using the key.</param>
        private Saml2SubjectConfirmation CreateHolderOfKeySubjectConfirmation(X509Certificate2 heldKey)
        {
            return new Saml2SubjectConfirmation(new Uri("urn:oasis:names:tc:SAML:2.0:cm:holder-of-key"))
            {
                SubjectConfirmationData = new Saml2SubjectConfirmationData()
                {
                    KeyIdentifiers = { new SecurityKeyIdentifier(new X509RawDataKeyIdentifierClause(heldKey)) }
                }
            };
        }
    }
}