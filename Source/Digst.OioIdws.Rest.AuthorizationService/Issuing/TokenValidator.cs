using System;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Digst.OioIdws.Rest.Common;

namespace Digst.OioIdws.Rest.AuthorizationService.Issuing
{
    internal class TokenValidator : ITokenValidator
    {
        public TokenValidationResult ValidateToken(string token, X509Certificate2 clientCertificate, OioIdwsAuthorizationServiceMiddleware.Settings settings)
        {
            //todo: decrypt token, test if it's an Assertion or EncryptedAssertion
            //todo validate assertion was issued by STS
            //todo validate signature value and digest
            //todo validate assertion xml, is not expired
            //todo validate AudienceRestriction identifies WSP
            //todo: return proper error codes
            using (var reader = new StringReader(token))
            {
                using (var xmlReader = XmlReader.Create(reader))
                {
                    var samlHandler = new Saml2SecurityTokenHandler
                    {
                        Configuration = new SecurityTokenHandlerConfiguration
                        {
                            ServiceTokenResolver = settings.ServiceTokenResolver,
                            AudienceRestriction = new AudienceRestriction
                            {
                                AllowedAudienceUris = { new Uri("https://wsp.itcrew.dk") } //todo: configurable
                            },
                            IssuerNameRegistry = new ConfigurationBasedIssuerNameRegistry
                            {
                                ConfiguredTrustedIssuers = { { "2e7a061560fa2c5e141a634dc1767dacaeec8d12", "Digitaliseringsstyrelsen - NemLog-in Test" } } //todo: configurable
                            }
                        }
                    };

                    var securityToken = samlHandler.ReadToken(xmlReader);
                    //todo: support audiences per issuer?
                    var identities = samlHandler.ValidateToken(securityToken);
                    
                    //todo check for holder-of-key/verify client cert
                    return new TokenValidationResult
                    {
                        Success = true,
                        AccessTokenType = AccessTokenType.Bearer, //todo: set properly
                        ClaimsIdentity = identities.First(),
                    };
                }
            }
        }
    }
}