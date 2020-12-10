using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;

namespace Digst.OioIdws.WscLocalTokenExample
{

    /// <summary>
    /// A "fake" security token service (STS) which will issue tokens as if issued
    /// by a real local STS. Because this service is in-memory it does not assume
    /// anything about the local/corporate infrastructure.
    /// </summary>
    /// <seealso cref="Digst.OioIdws.WscLocalTokenExample.ILocalTokenService" />
    public class InMemoryLocalTokenService : ILocalTokenService
    {

        private readonly InMemoryLocalTokenServiceConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryLocalTokenService" /> class.
        /// </summary>
        /// <param name="configuration">The configuration parameters for the service</param>
        public InMemoryLocalTokenService(InMemoryLocalTokenServiceConfiguration configuration)
        {
            _configuration = configuration;
        }


        /// <summary>
        /// Issues a token for the specified subject, with the specified attributes and for the specified audience.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="attributes">The attributes.</param>
        /// <param name="audience">The audience.</param>
        /// <returns></returns>
        public SecurityToken Issue(Saml2Subject subject, IEnumerable<Saml2Attribute> attributes, Uri audience)
        {
            var issuer = new Saml2NameIdentifier(_configuration.EntityId);
            var assertion = new Saml2Assertion(issuer)
            {
                Id = new Saml2Id("_" + Guid.NewGuid().ToString("D")),
                SigningCredentials = new X509SigningCredentials(_configuration.TokenSigningCertificate),
                Subject = subject,
                IssueInstant = DateTime.UtcNow,
                Conditions = new Saml2Conditions()
                {
                    NotBefore = DateTime.UtcNow,
                    NotOnOrAfter = DateTime.UtcNow + _configuration.TokenValidDuration,
                },
            };

            if (audience != null) assertion.Conditions.AudienceRestrictions.Add(new Saml2AudienceRestriction(audience));
            
            var attributeStatement = new Saml2AttributeStatement(attributes);
            if (attributeStatement.Attributes.Any()) assertion.Statements.Add(attributeStatement);

            return new Saml2SecurityToken(assertion);
        }

    }
}