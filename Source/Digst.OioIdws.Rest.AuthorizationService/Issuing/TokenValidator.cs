using System;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;
using Digst.OioIdws.Rest.Common;

namespace Digst.OioIdws.Rest.AuthorizationService.Issuing
{
    internal class TokenValidator : ITokenValidator
    {
        public async Task<TokenValidationResult> ValidateTokenAsync(string token, X509Certificate2 clientCertificate, OioIdwsAuthorizationServiceOptions options)
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
                            ServiceTokenResolver = options.ServiceTokenResolver,
                        }
                    };

                    //Parses the token, as well as decrypting it
                    var securityToken = samlHandler.ReadToken(xmlReader) as Saml2SecurityToken;

                    if (securityToken == null)
                    {
                        throw new InvalidOperationException("Token could not be decrypted to a valid Saml2SecurityToken");
                    }

                    var issuerToken = securityToken.IssuerToken as X509SecurityToken;

                    if (issuerToken == null)
                    {
                        return TokenValidationResult.Error(AuthenticationErrorCodes.InvalidToken, "IssuerToken is expected to be of type X509SecurityToken");
                    }

                    var issuerAudience = (await options.IssuerAudiences()).SingleOrDefault(x => x.IssuerThumbprint == issuerToken.Certificate.Thumbprint?.ToLowerInvariant());

                    if (issuerAudience == null)
                    {
                        return TokenValidationResult.Error(AuthenticationErrorCodes.InvalidToken, $"Issuer certificate '{issuerToken.Certificate.Thumbprint}' was unknown");
                    }

                    samlHandler.Configuration.IssuerNameRegistry = new SpecificIssuerNameRegistry(issuerAudience.IssuerThumbprint, issuerAudience.IssuerFriendlyName);
                    samlHandler.Configuration.AudienceRestriction = new AudienceRestriction();
                    issuerAudience.ToList().ForEach(a => samlHandler.Configuration.AudienceRestriction.AllowedAudienceUris.Add(a));

                    ClaimsIdentity identity = null;

                    //todo handle exception
                    try
                    {
                        identity = samlHandler.ValidateToken(securityToken).First();
                    }
                    catch (AudienceUriValidationFailedException)
                    {
                        return TokenValidationResult.Error(AuthenticationErrorCodes.InvalidToken, "Audience was not known");
                    }
                    
                    //todo check for holder-of-key/verify client cert
                    return new TokenValidationResult
                    {
                        Success = true,
                        AccessTokenType = AccessTokenType.Bearer, //todo: set properly
                        ClaimsIdentity = identity,
                    };
                }
            }
        }

        class SpecificIssuerNameRegistry : IssuerNameRegistry
        {
            private readonly string _thumbprint;
            private readonly string _name;

            public SpecificIssuerNameRegistry(string thumbprint, string name)
            {
                _thumbprint = thumbprint;
                _name = name;
            }

            public override string GetIssuerName(SecurityToken securityToken)
            {
                var x509Token = securityToken as X509SecurityToken;

                if (x509Token?.Certificate.Thumbprint?.ToLowerInvariant() == _thumbprint)
                {
                    return _name;
                }

                throw new SecurityTokenException("Untrusted issuer");
            }
        }
    }
}