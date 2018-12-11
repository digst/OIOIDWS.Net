using System;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using Digst.OioIdws.Common.Attributes;

namespace Digst.OioIdws.Healthcare.Wsc
{
    /// <summary>A factory which can create the different types of tokens</summary>
    public class HealthcareAuthenticationTokenFactory
    {
        private readonly TimeSpan _duration;
        private readonly Func<DateTime> _issueInstant;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthcareAuthenticationTokenFactory"/> class.
        /// </summary>
        /// <param name="duration">The duration of tokens created by the factory. Defaults to 5 minutes if not specified (null).</param>
        /// <param name="issueInstant">Function which returns the "current" date/time used as issue instant (and "NotBefore") on tokens. If not specified it defaults to current date/time. Use this if you need to create historic or future tokens for testing purposes.</param>
        public HealthcareAuthenticationTokenFactory(TimeSpan? duration=null, Func<DateTime> issueInstant=null)
        {
            _duration = duration ?? TimeSpan.FromMinutes(5);
            _issueInstant = issueInstant ?? (()=>DateTime.UtcNow);
        }

        /// <summary>
        /// Creates an authentication token (AUT).
        /// The token will be issued by the passed subject certificate
        /// </summary>
        /// <param name="subjectCertiticate">The subject certiticate. This should be a MOCES certificate identifying the employee.</param>
        /// <param name="assuranceLevel">The assurance level which describes the strength of the authentication.</param>
        /// <param name="duration">The duration. This should be kept short - usually in minutes as the AUT token is intended to be presented to the consumer immediately</param>
        /// <param name="encryptCertificate">The encrypt certificate.</param>
        /// <param name="issueInstant">The issue instant.</param>
        /// <returns></returns>
        public Saml2SecurityToken CreateAuthenticationToken(X509Certificate2 subjectCertiticate, AssuranceLevel assuranceLevel, X509Certificate2 encryptCertificate = null)
        {
            var assertion = new Saml2Assertion(new Saml2NameIdentifier("http://sts.sundhedsdatastyrelsen.dk/"))
            {
                Id = new Saml2Id($"uuid-{Guid.NewGuid():D}"),

                Subject = new Saml2Subject()
                {
                    NameId = new Saml2NameIdentifier(subjectCertiticate.SubjectName.Name)
                    {
                        Format = new Uri("urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName"),
                        NameQualifier = "NameQualifier",
                    },
                    SubjectConfirmations =
                    {
                        new Saml2SubjectConfirmation(new Uri("urn:oasis:names:tc:SAML:2.0:cm:bearer"))
                        {
                            NameIdentifier = new Saml2NameIdentifier(subjectCertiticate.SubjectName.Name)
                            {
                                Format = new Uri("urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName"),
                                NameQualifier = "NameQualifier",
                            },
                        }
                    },
                },

                Conditions = new Saml2Conditions()
                {
                    NotBefore = _issueInstant?.Invoke() ?? DateTime.UtcNow,
                    NotOnOrAfter = (_issueInstant?.Invoke() ?? DateTime.UtcNow).Add(_duration),
                },

                Statements = { new Saml2AttributeStatement()
                {
                    Attributes =
                    {
                        new Saml2Attribute("dk:gov:saml:attribute:AssuranceLevel", SamlAttributeValueConverter<AssuranceLevel>.ToSamlValueString(assuranceLevel)),
                        new Saml2Attribute("dk:gov:saml:attribute:SpecVer", SpecVer.DkSaml20.VersionIdentifier),
                    }
                } },

                SigningCredentials = GetSigningCredentials(subjectCertiticate),

                EncryptingCredentials = (encryptCertificate != null) ? GetEncryptionCredentials(encryptCertificate) : null,
            };

            return new Saml2SecurityToken(assertion);
        }



        private static X509SigningCredentials GetSigningCredentials(X509Certificate2 signingCertificate)
        {
            return new X509SigningCredentials(signingCertificate, SecurityAlgorithms.RsaSha256Signature, SecurityAlgorithms.Sha256Digest);
        }

        private static EncryptingCredentials GetEncryptionCredentials(X509Certificate2 encryptCertificate)
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