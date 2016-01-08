using System;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;
using Digst.OioIdws.Rest.Common;

namespace Digst.OioIdws.Rest.Server.AuthorizationServer.Issuing
{
    internal class TokenValidator : ITokenValidator
    {
        private static readonly Uri Saml2BearerMethod = new Uri("urn:oasis:names:tc:SAML:2.0:cm:bearer");
        private static readonly Uri Saml2HolderOfKeyMethod = new Uri("urn:oasis:names:tc:SAML:2.0:cm:holder-of-key");

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

                    //var sb = new StringBuilder();
                    //var writer = new XmlTextWriter(new StringWriter(sb));
                    //samlHandler.WriteToken(writer, securityToken);

                    if (securityToken == null)
                    {
                        return TokenValidationResult.Error("Token could not be decrypted to a valid Saml2SecurityToken");
                    }

                    var accessTokenType = DetermineAccessTokenType(securityToken);
                    
                    if (accessTokenType == AccessTokenType.HolderOfKey)
                    {
                        X509RawDataKeyIdentifierClause key;

                        try
                        {
                            key = GetSecurityKeyIdentifierClause(securityToken);
                        }
                        catch (Exception)
                        {
                            return TokenValidationResult.Error("X509Certificate used as SubjectConfirmationData could not read from the token");
                        }

                        if (!key.Matches(clientCertificate))
                        {
                            return TokenValidationResult.Error("X509Certificate used as SubjectConfirmationData did not match the provided client certificate");
                        }
                    }
                    else if (accessTokenType == AccessTokenType.Bearer)
                    {
                        //no validation needed
                    }
                    else
                    {
                        return TokenValidationResult.Error("SubjectConfirmation method was not valid");
                    }

                    var issuerToken = securityToken.IssuerToken as X509SecurityToken;

                    if (issuerToken == null)
                    {
                        return TokenValidationResult.Error("IssuerToken is expected to be of type X509SecurityToken");
                    }

                    var issuerAudience = (await options.IssuerAudiences()).SingleOrDefault(x => x.IssuerThumbprint == issuerToken.Certificate.Thumbprint?.ToLowerInvariant());

                    if (issuerAudience == null)
                    {
                        return TokenValidationResult.Error($"Issuer certificate '{issuerToken.Certificate.Thumbprint}' was unknown");
                    }

                    samlHandler.Configuration.IssuerNameRegistry = new SpecificIssuerNameRegistry(issuerAudience.IssuerThumbprint, issuerAudience.IssuerFriendlyName);
                    samlHandler.Configuration.AudienceRestriction = new AudienceRestriction();
                    issuerAudience.ToList().ForEach(a => samlHandler.Configuration.AudienceRestriction.AllowedAudienceUris.Add(a));

                    ClaimsIdentity identity;

                    //todo handle exception
                    try
                    {
                        identity = samlHandler.ValidateToken(securityToken).First();
                    }
                    catch (AudienceUriValidationFailedException)
                    {
                        return TokenValidationResult.Error("Audience was not known");
                    }
                    
                    return new TokenValidationResult
                    {
                        Success = true,
                        AccessTokenType = accessTokenType.Value, 
                        ClaimsIdentity = identity,
                    };
                }
            }
        }

        private AccessTokenType? DetermineAccessTokenType(Saml2SecurityToken securityToken)
        {
            var method = securityToken.Assertion.Subject.SubjectConfirmations.FirstOrDefault()?.Method;
            if (method == Saml2BearerMethod)
            {
                return AccessTokenType.Bearer;
            }

            if (method == Saml2HolderOfKeyMethod)
            {
                return AccessTokenType.HolderOfKey;
            }

            return null;
        }

        X509RawDataKeyIdentifierClause GetSecurityKeyIdentifierClause(Saml2SecurityToken securityToken)
        {
            return (X509RawDataKeyIdentifierClause)securityToken.Assertion.Subject.SubjectConfirmations.First().SubjectConfirmationData.KeyIdentifiers.First().First();
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