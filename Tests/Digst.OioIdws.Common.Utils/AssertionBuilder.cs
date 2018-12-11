using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Digst.OioIdws.SamlAttributes;
using Digst.OioIdws.SamlAttributes.AttributeMarshals;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;

namespace Digst.OioIdws.Common.Utils
{
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

        public AssertionBuilder(string issuerName)
        {
            _issuerName = issuerName;
            _attributeStatement = new Saml2ComplexAttributeStatement();
            _attributeAdapter = new AttributeStatementAttributeAdapter(_attributeStatement);
        }

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

        public AssertionBuilder Duration(TimeSpan duration)
        {
            _duration = duration;
            return this;
        }

        public AssertionBuilder OneTimeUse(bool oneTimeUse = true)
        {
            _oneTimeUse = oneTimeUse;
            return this;
        }


        public AssertionBuilder AudienceRestriction(params Uri[] audiences)
        {
            _audienceRestriction = audiences.Any() ? new Saml2AudienceRestriction(audiences) : null;
            return this;
        }

        public AssertionBuilder SubjectNameId(string x509SubjectName)
        {
            _subjectNameId = x509SubjectName != null ? new Saml2NameIdentifier(x509SubjectName, new Uri("urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName")) : null;
            return this;
        }

        public AssertionBuilder BearerSubjectConfirmation(X509Certificate2 proofCertificate)
        {
            if (proofCertificate == null) throw new ArgumentNullException(nameof(proofCertificate));
            _subjectConfirmation =
                new Saml2SubjectConfirmation(new Uri("urn:oasis:names:tc:SAML:2.0:cm:bearer"))
                {
                };
            return this;
        }

        public AssertionBuilder SigningCertificate(X509Certificate2 signingCertificate)
        {
            _signingCredentials = signingCertificate != null ? new X509SigningCredentials(signingCertificate) : null;
            return this;
        }

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

        public Saml2Assertion Build()
        {
            return Build(DateTime.UtcNow);
        }

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



        public AssertionBuilder WithAttribute<T>(SamlAttributeMarshal<T> marshal, T value)
        {
            _attributeAdapter.SetValue(marshal, value);
            return this;
        }


        public AssertionBuilder WithNoAttribute<T>(SamlAttributeMarshal<T> marshal)
        {
            _attributeAdapter.RemoveAttribute(marshal);
            return this;
        }


    }
}
