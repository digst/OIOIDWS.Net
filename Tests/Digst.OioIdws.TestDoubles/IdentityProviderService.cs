using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Digst.OioIdws.Common.Utils;
using Digst.OioIdws.SamlAttributes;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;

namespace Digst.OioIdws.TestDoubles
{
    public class IdentityProviderService
    {
        private readonly IdentityProviderDescriptor _identityProviderDescriptor;
        private readonly IAuthenticationAttributeProviderFactory _authenticationAttributeProviderFactory;
        private readonly IAttributeProvider _serviceAttributeProvider;
        private readonly TimeSpan _bootstrapTokenLifeTime;

        public IdentityProviderService(IdentityProviderDescriptor identityProviderDescriptor, IAuthenticationAttributeProviderFactory authenticationAttributeProviderFactory, IAttributeProvider serviceAttributeProvider, TimeSpan _bootstrapTokenLifeTime)
        {
            _identityProviderDescriptor = identityProviderDescriptor;
            _authenticationAttributeProviderFactory = authenticationAttributeProviderFactory;
            _serviceAttributeProvider = serviceAttributeProvider;
            this._bootstrapTokenLifeTime = _bootstrapTokenLifeTime;
        }



        public SecurityToken CreateHealthcareBootstrapToken(IEnumerable<ServiceProviderDescriptor> securityTokenServices, AuthenticationDescriptor authenticationDescriptor, RequestDescriptor requestDescriptor)
        {
            var subject = new Saml2Subject(
                new Saml2NameIdentifier(authenticationDescriptor.Subject.Certificate.Subject, new Uri("urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName")));

            subject.SubjectConfirmations.Add(new Saml2SubjectConfirmation(new Uri("urn:oasis:names:tc:SAML:2.0:cm:holder-of-key"), new Saml2SubjectConfirmationData()
            {
                KeyIdentifiers = { new SecurityKeyIdentifier(new X509RawDataKeyIdentifierClause(requestDescriptor.Requestor.Certificate)) }
            }));

            var assertion = new Saml2Assertion(GetIssuer())
            {
                Subject = subject,
                SigningCredentials = GetSigningCredentials(_identityProviderDescriptor.Certificate),
                Conditions = new Saml2Conditions()
                {
                    AudienceRestrictions = { new Saml2AudienceRestriction(securityTokenServices.Select(x => x.EntityId)) },
                    NotBefore = DateTime.UtcNow,
                    NotOnOrAfter = DateTime.UtcNow.Add(_bootstrapTokenLifeTime),
                }
            };
            return new Saml2SecurityToken(assertion);
        }



        /// <summary>
        /// Creates an authentication token (AUT).
        /// </summary>
        /// <param name="serviceProvider">The service provider which will consume this token.</param>
        /// <param name="authenticationDescriptor">Describes the authentication event which established the identity of the subject of this token.</param>
        /// <param name="requestDescriptor">Describes the authentication request of the token requestor (usually the service provider).</param>
        /// <returns></returns>
        public SecurityToken CreateAuthenticationToken(ServiceProviderDescriptor serviceProvider, AuthenticationDescriptor authenticationDescriptor, RequestDescriptor requestDescriptor)
        {

            var subject = new Saml2Subject(new Saml2NameIdentifier(authenticationDescriptor.Subject.Certificate.Subject, new Uri("urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName")));
            subject.SubjectConfirmations.Add(new Saml2SubjectConfirmation(new Uri("urn:oasis:names:tc:SAML:2.0:cm:bearer"), new Saml2SubjectConfirmationData()
            {
                Recipient = serviceProvider.AssertionConsumerService,
                NotOnOrAfter = DateTime.UtcNow.AddMinutes(5),
                InResponseTo = new Saml2Id(requestDescriptor.RequestMessageId),
            }));

            var conditions = new Saml2Conditions()
            {
                NotBefore = DateTime.UtcNow,
                NotOnOrAfter = DateTime.UtcNow.AddMinutes(5),
                AudienceRestrictions = { new Saml2AudienceRestriction(serviceProvider.EntityId) },
            };

            var authnStatement = new Saml2AuthenticationStatement(new Saml2AuthenticationContext())
            {
                SubjectLocality = authenticationDescriptor.IpAddress != null || authenticationDescriptor.DnsName != null ? new Saml2SubjectLocality(authenticationDescriptor.IpAddress, authenticationDescriptor.DnsName) : null,
                SessionIndex = authenticationDescriptor.SessionIndex,
                SessionNotOnOrAfter = authenticationDescriptor.SessionNotOnOrAfter,
                AuthenticationInstant = authenticationDescriptor.AuthenticationInstant,
            };

            var attributeStatement = new Saml2ComplexAttributeStatement();

            var attributeManager = new AttributeStatementAttributeAdapter(attributeStatement);

            var attributeProvider = new CompoundAttributeProvider(_serviceAttributeProvider, _authenticationAttributeProviderFactory.Create(authenticationDescriptor));

            foreach (var requestedAttribute in requestDescriptor.RequestedAttributes)
            {
                attributeProvider.ProvideAttribute(requestedAttribute, attributeManager);
            }

            var assertion = new Saml2Assertion(GetIssuer())
            {
                IssueInstant = DateTime.UtcNow,
                Subject = subject,
                Conditions = conditions,
                Statements = { authnStatement, attributeStatement },
                EncryptingCredentials = GetEncryptionCredentials(serviceProvider.Certificate),
                SigningCredentials = GetSigningCredentials(_identityProviderDescriptor.Certificate),
                Id = new Saml2Id($"_{Guid.NewGuid():D}")
            };

            return new Saml2SecurityToken(assertion);
        }

        private Saml2NameIdentifier GetIssuer()
        {
            return new Saml2NameIdentifier(_identityProviderDescriptor.IssuerDomain);
        }


        private static X509SigningCredentials GetSigningCredentials(X509Certificate2 signingCertificate)
        {
            return new X509SigningCredentials(signingCertificate, SecurityAlgorithms.RsaSha256Signature, SecurityAlgorithms.Sha256Digest);
        }


        private EncryptingCredentials GetEncryptionCredentials(X509Certificate2 encryptCertificate)
        {
            var keyBytes = new byte[32];
            System.Security.Cryptography.RandomNumberGenerator.Create().GetBytes(keyBytes);
            var securityKey = new InMemorySymmetricSecurityKey(keyBytes);
            // Random 256 bits have been chosen and a key created from them

            // To transmit the symmetric key, the key itself is encrypted (wrapped) using the public key of the intended recipient's certificate.
            // This way only the holder of the certificate's private key can decrypt the symmetrical key used to encrypt the assertion
            var securityKeyIdentifier = new SecurityKeyIdentifier(new EncryptedKeyIdentifierClause(keyBytes, SecurityAlgorithms.RsaOaepKeyWrap, new SecurityKeyIdentifier(new X509IssuerSerialKeyIdentifierClause(encryptCertificate))));

            // The assertion itself is symmetrically entrypted using 256 bit AES encryption.
            var algorithm = SecurityAlgorithms.Aes256Encryption;

            return new EncryptingCredentials(securityKey, securityKeyIdentifier, algorithm);
        }

    }
}