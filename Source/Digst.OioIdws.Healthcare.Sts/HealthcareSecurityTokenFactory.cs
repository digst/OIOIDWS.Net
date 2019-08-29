using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;

namespace Digst.OioIdws.Healthcare.Sts
{
    /// <summary>A factory which can create the different types of tokens</summary>
    public class HealthcareSecurityTokenFactory
    {
        private const string UrnX509SubjectName = "urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName";
        private const string UrnBearerToken = "urn:oasis:names:tc:SAML:2.0:cm:bearer";
        private const string UrnHolderOfKey = "urn:oasis:names:tc:SAML:2.0:cm:holder-of-key";
        private readonly X509Certificate2 _tokenSigningCertificate;
        private readonly string _issuerName;
        private readonly string _nameQualifier;
        private readonly TimeSpan _duration;
        private readonly Func<DateTime> _issueInstant;


        /// <summary>
        /// Initializes a new instance of the <see cref="HealthcareSecurityTokenFactory"/> class.
        /// </summary>
        /// <param name="tokenSigningCertificate">The token signing certificate.</param>
        /// <param name="issuerName">Identity of the security token service (the issuer of the tokens).</param>
        /// <param name="nameQualifier">The name qualifier.</param>
        /// <param name="duration">Default duration of the issued tokens. If not specified the default will be set to 5 minutes.</param>
        /// <param name="issueInstant">Function which returns the issue instant for token (the date/time the token is issued). If not specified it will default to the current date/time at the time the token is created. Use this if you need to create tokens with specific issue instants, e.g. for reproducible testing or for creating stale tokens.</param>
        public HealthcareSecurityTokenFactory(X509Certificate2 tokenSigningCertificate, string issuerName, string nameQualifier, TimeSpan? duration=null, Func<DateTime> issueInstant=null)
        {
            _tokenSigningCertificate = tokenSigningCertificate;
            _issuerName = issuerName;
            _nameQualifier = nameQualifier;
            _duration = duration ?? TimeSpan.FromMinutes(5);
            _issueInstant = issueInstant ?? (()=>DateTime.UtcNow);
        }


        /// <summary>
        /// Creates a holder of key token for invoking a WSP service *not* in the context of a specific user, i.e. this token is used for the "system user" scenario.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="consumerCertificate">The certificate of the service consumer.</param>
        /// <param name="serviceUri">The service URI.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="encryptionCertificate"></param>
        /// <returns></returns>
        public Saml2SecurityToken CreateServiceToken(X509Certificate2 consumerCertificate, Uri serviceUri,
            TimeSpan duration, X509Certificate2 encryptionCertificate = null)
        {
            return CreateServiceToken(new ComplexSamlAttribute[0], consumerCertificate, serviceUri, duration, encryptionCertificate);
        }

        /// <summary>
        /// Creates a holder of key token for invoking a WSP service *not* in the context of a specific user, i.e. this token is used for the "system user" scenario. Can optionally encrypt the token. Can optionally restrict the token to a given audience.
        /// </summary>
        /// <param name="attributes">A list of potentially complex (xml valued) attributes. The token will contain a SAML attribute statement with these attributes.</param>
        /// <param name="consumerCertificate">The certificate (proof token) of the service consumer. Upon service invocation, the caller must prove posession of the private key of this certificate.</param>
        /// <param name="serviceUri">The URI of the service to which the token will be presented. If specified, the token will be created with an audience restriction to the given URI. If not specified, the token/assertion will not have any audience restriction.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="encryptionCertificate">If specified indicates the certificate to be used for encrypting the token(assertion). If not specified, the token will not be encrypted. The public key of the certificate will be used so that only the holder of the private key can decrypt the token.</param>
        /// <returns></returns>
        public Saml2SecurityToken CreateServiceToken(IEnumerable<ComplexSamlAttribute> attributes, X509Certificate2 consumerCertificate, Uri serviceUri, TimeSpan duration, X509Certificate2 encryptionCertificate = null)
        {
            var instant = _issueInstant();
            var assertion = new Saml2Assertion(new Saml2NameIdentifier(_issuerName))
            {
                // Make sure the assertion has a unique ID
                Id = new Saml2Id($"uuid-{Guid.NewGuid():D}"),

                // Set up the Subject and -confirmations
                Subject = new Saml2Subject()
                {
                    NameId = new Saml2NameIdentifier(consumerCertificate.SubjectName.Name)
                    {
                        Format = new Uri(UrnX509SubjectName),
                        NameQualifier = _nameQualifier,
                    },
                    SubjectConfirmations =
                    {
                        new Saml2SubjectConfirmation(new Uri(UrnHolderOfKey))
                        {
                            SubjectConfirmationData = new Saml2SubjectConfirmationData()
                            {
                                KeyIdentifiers =
                                {
                                    new SecurityKeyIdentifier() { new X509RawDataKeyIdentifierClause(consumerCertificate) }
                                }
                            }
                        }
                    },
                },

                Conditions = new Saml2Conditions()
                {
                    NotBefore = instant,
                    NotOnOrAfter = instant.Add(_duration),
                },

                Statements = { new Saml2ComplexAttributeStatement(attributes) },

                SigningCredentials = GetSigningCredentials(_tokenSigningCertificate),

                EncryptingCredentials = (encryptionCertificate != null) ? GetEncryptionCredentials(encryptionCertificate) : null,
            };

            // If a service uri is specified then make sure that the audience is restricted to this uri
            if (serviceUri != null)
            {
                assertion.Conditions.AudienceRestrictions.Add(new Saml2AudienceRestriction(serviceUri));
            }

            return new Saml2SecurityToken(assertion);

        }




        /// <summary>Creates an identity token (IDT) to be used as security token when invoking a WSP service. Can create a bearer or a holder-of-key token. Can optionally encrypt the token.</summary>
        /// <param name="subjectCertificate">The subject's certificate</param>
        /// <param name="attributes">Attributes to be built into the token</param>
        /// <param name="proofCertificate">If this parameter is null then the token issued will be a *bearer* token. If specified, then the issued token will be a *holder-of-key* token with the private key of this certificate as the proof key.</param>
        /// <param name="serviceUri">The URL of the service to be invoked</param>
        /// <param name="duration">The duration from issue instant that the token should be valid</param>
        /// <param name="encryptionCertificate">
        ///     If specified, the token will be encrypted using this token. This parameter should be null for non-encrypted tokens, it should be the service certificate if specified.
        /// </param>
        public Saml2SecurityToken CreateIdentityToken(X509Certificate2 subjectCertificate, IEnumerable<ComplexSamlAttribute> attributes, X509Certificate2 proofCertificate, Uri serviceUri,
            TimeSpan duration, X509Certificate2 encryptionCertificate = null)
        {
            if (subjectCertificate == null) throw new ArgumentNullException(nameof(subjectCertificate));

            var instant = _issueInstant();

            // Determine the key type based on whether a proof certificate was passed or not.
            var keyType = proofCertificate != null ? UrnHolderOfKey : UrnBearerToken;

            var assertion = new Saml2Assertion(new Saml2NameIdentifier("http://sts.sundhedsdatastyrelsen.dk/"))
            {
                Id = new Saml2Id($"uuid-{Guid.NewGuid():D}"),

                Subject = new Saml2Subject()
                {
                    NameId = new Saml2NameIdentifier(subjectCertificate.SubjectName.Name)
                    {
                        Format = new Uri(UrnX509SubjectName),
                        NameQualifier = _nameQualifier,
                    },
                    SubjectConfirmations =
                    {
                        new Saml2SubjectConfirmation(new Uri(keyType))
                        {
                            NameIdentifier = new Saml2NameIdentifier(subjectCertificate.SubjectName.Name)
                            {
                                Format = new Uri(UrnX509SubjectName),
                                NameQualifier = _nameQualifier,
                            },
                        },
                    },
                },

                Conditions = new Saml2Conditions()
                {
                    NotBefore = instant,
                    NotOnOrAfter = instant + duration,
                },

                Statements = { new Saml2ComplexAttributeStatement(attributes ?? Enumerable.Empty<ComplexSamlAttribute>()) },

                SigningCredentials = GetSigningCredentials(_tokenSigningCertificate),

                EncryptingCredentials = (encryptionCertificate != null) ? GetEncryptionCredentials(encryptionCertificate) : null,
            };

            // If a proof certificate was passed, make sure that the subject confirmation
            // data indicates posession of the certificate private key as a way to confirm the subject
            if (proofCertificate != null)
            {
                assertion.Subject.SubjectConfirmations[0].SubjectConfirmationData = new Saml2SubjectConfirmationData()
                {
                    KeyIdentifiers = { new SecurityKeyIdentifier(new X509RawDataKeyIdentifierClause(proofCertificate)) }
                };
            }

            // If a service uri is specified then make sure that the audience is restricted to this uri
            if (serviceUri != null)
            {
                assertion.Conditions.AudienceRestrictions.Add(new Saml2AudienceRestriction(serviceUri));
            }

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