using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Digst.OioIdws.SamlAttributes;
using Digst.OioIdws.SamlAttributes.AttributeAdapters;
using Digst.OioIdws.SamlAttributes.AttributeMarshals;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;

namespace Digst.OioIdws.Common.Utils
{
    /// <summary>
    /// Builds SAML2 assertions for test purposes.
    /// </summary>
    public class AssertionBuilder
    {
        private readonly string _issuerName;
        private Saml2SubjectConfirmation _subjectConfirmation;

        private Saml2NameIdentifier _subjectNameId;
        private Saml2AudienceRestriction _audienceRestriction;
        private SigningCredentials _signingCredentials;
        private TimeSpan? _duration = null;
        private bool _oneTimeUse = false;
        private X509EncryptingCredentials _encryptionCredentials;
        private Saml2AuthenticationStatement _authnStatement;
        private readonly Saml2ComplexAttributeStatement _attributeStatement;
        private readonly AttributeStatementAttributeAdapter _attributeAdapter;

        /// <summary>Initializes a new instance of the <see cref="AssertionBuilder"/> class.</summary>
        /// <param name="issuerName">Name of the issuer.</param>
        public AssertionBuilder(string issuerName)
        {
            _issuerName = issuerName;
            _attributeStatement = new Saml2ComplexAttributeStatement();
            _attributeAdapter = new AttributeStatementAttributeAdapter(_attributeStatement);
        }

        /// <summary>Sets the builder to build holder-of-key tokens.</summary>
        /// <param name="proofCertificate">The proof certificate. When the token is used, the caller who claims access by including the token must prove ownership of the private key of this certificate.</param>
        /// <returns>The AssertionBuilder (fluent API)</returns>
        public AssertionBuilder HolderOfKeySubjectConfirmation(X509Certificate2 proofCertificate)
        {
            _subjectConfirmation =
                new Saml2SubjectConfirmation(new Uri("urn:oasis:names:tc:SAML:2.0:cm:holder-of-key"))
                {
                    SubjectConfirmationData = new Saml2SubjectConfirmationData()
                    {
                        KeyIdentifiers = { new SecurityKeyIdentifier(new X509RawDataKeyIdentifierClause(proofCertificate)) }
                    }
                };
            return this;
        }

        /// <summary>Configures the duration of issued tokens. Tokens issued by the AssertionBuilder will have this duration.</summary>
        /// <param name="duration">The duration.</param>
        /// <returns>The AssertionBuilder (fluent API)</returns>
        public AssertionBuilder Duration(TimeSpan duration)
        {
            _duration = duration;
            return this;
        }

        /// <summary>  Configures the AssertionBuilder to issue one-time-use tokens - i.e. tokens that should only be used/trusted once.</summary>
        /// <param name="oneTimeUse">if set to <c>true the AssertionBuilder will issue one-time-use tokens.</c></param>
        /// <returns>The AssertionmBuilder (fluent API)</returns>
        public AssertionBuilder OneTimeUse(bool oneTimeUse = true)
        {
            _oneTimeUse = oneTimeUse;
            return this;
        }


        /// <summary>Configures the AssertionBuilder to issue tokens to specific audiences.</summary>
        /// <param name="audiences">The audiences.</param>
        /// <returns>The AssertionBuilder (fluent API)</returns>
        public AssertionBuilder AudienceRestriction(params Uri[] audiences)
        {
            _audienceRestriction = audiences.Any() ? new Saml2AudienceRestriction(audiences) : null;
            return this;
        }

        /// <summary>Configures the AssertionBuilder to use a specific X509 Subject Name</summary>
        /// <param name="x509SubjectName">Name of the X509 subject.</param>
        /// <returns>The AssertionBuilder (fluent API)</returns>
        public AssertionBuilder SubjectNameId(string x509SubjectName)
        {
            _subjectNameId = x509SubjectName != null ? new Saml2NameIdentifier(x509SubjectName, new Uri("urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName")) : null;
            return this;
        }

        /// <summary>Bearers the subject confirmation.</summary>
        /// <returns>The AssertionBuilder (fluent API)</returns>
        /// <exception cref="ArgumentNullException">proofCertificate</exception>
        public AssertionBuilder BearerSubjectConfirmation()
        {
            _subjectConfirmation =
                new Saml2SubjectConfirmation(new Uri("urn:oasis:names:tc:SAML:2.0:cm:bearer"))
                {
                };
            return this;
        }

        /// <summary>  Configures the certificate used for signing tokens</summary>
        /// <param name="signingCertificate">The signing certificate.</param>
        /// <returns>The AssertionBuilder (fluent API)</returns>
        public AssertionBuilder SigningCertificate(X509Certificate2 signingCertificate)
        {
            _signingCredentials = signingCertificate != null ? new X509SigningCredentials(signingCertificate) : null;
            return this;
        }

        /// <summary>
        /// Configures the AssertionBuilder to encrypt issued tokens using the specified certificate. Tokens will be encrypted using the certificate's public key, which will ensure that only a holder of the private key can decrypt the token.
        /// </summary>
        /// <param name="encryptionCertificate">The encryption certificate.</param>
        /// <returns>The AssertionBuilder (fluent API)</returns>
        public AssertionBuilder EncryptionCertificate(X509Certificate2 encryptionCertificate)
        {
            _encryptionCredentials = encryptionCertificate != null ? new X509EncryptingCredentials(encryptionCertificate) : null;
            return this;
        }


        public AssertionBuilder NoAuthentication()
        {
            _authnStatement = null;
            return this;
        }

        /// <summary>
        /// Registers when and how the user was authenticated
        /// </summary>
        /// <param name="sessionIndex">Index of the session.</param>
        /// <param name="authnInstant">The authn instant.</param>
        /// <param name="sessionNotOnOrAfter">The session not on or after.</param>
        /// <param name="classReference"></param>
        /// <param name="address">The address.</param>
        /// <param name="dnsName">Name of the DNS.</param>
        /// <returns></returns>
        public AssertionBuilder Authentication(string sessionIndex, DateTime authnInstant, DateTime? sessionNotOnOrAfter, Uri classReference = null, string address = null, string dnsName = null)
        {
            _authnStatement = new Saml2AuthenticationStatement(new Saml2AuthenticationContext(classReference ?? new Uri("urn:oasis:names:tc:SAML:2.0:ac:classes:X509")))
            {
                AuthenticationInstant = authnInstant.ToUniversalTime(),
                SessionIndex = sessionIndex,
                SessionNotOnOrAfter = sessionNotOnOrAfter?.ToUniversalTime(),
                SubjectLocality = (address != null || dnsName != null) ? new Saml2SubjectLocality()
                {
                    Address = address,
                    DnsName = dnsName,
                } : null,
            };
            return this;
        }

        /// <summary>  Builds an assertion (issues a token)</summary>
        /// <returns>The issued assertion (token)</returns>
        public Saml2Assertion Build()
        {
            return Build(DateTime.UtcNow);
        }

        /// <summary>  Builds an assertion as if it is issued at the specified instant. The instant will be built into the assertion and will also be used in <em>not before</em> restriction.</summary>
        /// <param name="issueInstant">The issue instant.</param>
        /// <returns>The issued assertion</returns>
        /// <exception cref="InvalidOperationException">Duration must be specified</exception>
        public Saml2Assertion Build(DateTime issueInstant)
        {
            if (!_duration.HasValue) throw new InvalidOperationException("Duration must be specified");
            var assertion = new Saml2Assertion(new Saml2NameIdentifier(_issuerName))
            {
                Id = new Saml2Id("_" + Guid.NewGuid().ToString("N")),
                Subject = new Saml2Subject()
                {
                    SubjectConfirmations = { _subjectConfirmation },
                    NameId = _subjectNameId,
                },
                Conditions = new Saml2Conditions()
                {
                    NotBefore = issueInstant.ToUniversalTime(),
                    NotOnOrAfter = issueInstant.ToUniversalTime().Add(_duration.Value),
                    OneTimeUse = _oneTimeUse,
                },
                SigningCredentials = _signingCredentials,
                EncryptingCredentials = _encryptionCredentials,
                IssueInstant = issueInstant.ToUniversalTime(),
                Issuer = new Saml2NameIdentifier(_issuerName, new Uri("urn:oasis:names:tc:SAML:2.0:nameid-format:entity")),
            };

            if (_audienceRestriction != null) assertion.Conditions.AudienceRestrictions.Add(_audienceRestriction);

            if (_authnStatement != null) assertion.Statements.Add(_authnStatement);

            assertion.Statements.Add(_attributeStatement);

            return assertion;
        }



        /// <summary>  Configures the AssertionBuilder to issue tokens with the specified attribute and value.</summary>
        /// <typeparam name="T">The type of the value that are marshalled by the attribute marshal.</typeparam>
        /// <param name="marshal">The marshal that specifies the attribute (claim type)</param>
        /// <param name="value">The value of the attribute</param>
        /// <returns>The AssertionBuilder (fluent API)</returns>
        public AssertionBuilder WithAttribute<T>(SamlAttributeMarshal<T> marshal, T value)
        {
            _attributeAdapter.SetValue(marshal, value);
            return this;
        }


        /// <summary>  Configures the AssertionBuilder to issue tokens <em>without</em> a given attribute (that was previously configured with a value).</summary>
        /// <typeparam name="T">The type of the value marshalled by the marshaller.</typeparam>
        /// <param name="marshal">  Specifies the attribute (claim type).</param>
        /// <returns>The AssertionBuilder (fluent API)</returns>
        public AssertionBuilder WithNoAttribute<T>(SamlAttributeMarshal<T> marshal)
        {
            _attributeAdapter.RemoveAttribute(marshal);
            return this;
        }


    }
}
