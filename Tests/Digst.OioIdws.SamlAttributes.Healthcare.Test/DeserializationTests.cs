using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Digst.OioIdws.Healthcare.Common;
using Digst.OioIdws.Healthcare.SamlAttributes;
using Digst.OioIdws.SamlAttributes.AttributeAdapters;
using Digst.OioIdws.SamlAttributes.AttributeMarshals;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.SamlAttributes.Healthcare.Test
{
    [TestClass]
    public class DeserializationTests
    {

        public TestContext TestContext { get; set; }




        [TestMethod, DeploymentItem("AssertionWithEveryHealthcareAttribute.xml")]
        public void CanReadAllSimpleValues()
        {
            var token = ReadAssertion("AssertionWithEveryHealthcareAttribute.xml");
            var attributeManager = new AttributeStatementAttributeAdapter(token.Statements.OfType<Saml2ComplexAttributeStatement>().Single());

            AssertCanReadValue(attributeManager, "dk:healthcare:saml:attribute:UserAuthorizationCode", "7AD6T");
            AssertCanReadValue(attributeManager, "dk:healthcare:saml:attribute:UserEducationCode", "5166");
            AssertCanReadValue(attributeManager, "dk:healthcare:saml:attribute:UserEducationType", "Sygeplejerske");
            AssertCanReadValue(attributeManager, "dk:healthcare:saml:attribute:HasUserAuthorization", true);
            AssertCanReadValue(attributeManager, "urn:oasis:names:tc:xspa:1.0:subject:subject-id", "Hansine Jensen");
            AssertCanReadValue(attributeManager, "urn:oasis:names:tc:xspa:1.0:subject:organization", "Region Hovedstaden");
            AssertCanReadValue(attributeManager, "urn:oasis:names:tc:xspa:1.0:subject:organization-id", "https://www.regionh.dk");
            AssertCanReadValue(attributeManager, "urn:ihe:iti:xca:2010:homeCommunityId", "urn:oid:1.2.208");
            AssertCanReadValue(attributeManager, "urn:oasis:names:tc:xspa:1.0:subject:npi", "33257872");
            AssertCanReadValue(attributeManager, "urn:ihe:iti:bppc:2007:docid", "https://example.org/consentdocument");
            AssertCanReadValue(attributeManager, "urn:ihe:iti:xua:2012:acp", "oid:exampleoid");
            AssertCanReadValue(attributeManager, "urn:oasis:names:tc:xacml:2.0:resource:resource-id", "2512484916^^^&1.2.208.176.1.2&ISO");
            AssertCanReadValue(attributeManager, "dk:healthcare:saml:attribute:IsRegisteredPharmacist", true);
            AssertCanReadValue(attributeManager, "dk:healthcare:saml:attribute:ECprNumberIdentifier", "251248A89F");
            AssertCanReadValue(attributeManager, "dk:healthcare:saml:attribute:FidNumberIdentifier", "76794884");
            AssertCanReadValue(attributeManager, "dk:healthcare:saml:attribute:OneTimePseudonym", "1b2948e1-5aee-4c22-aed9-23za");
            AssertCanReadValue(attributeManager, "dk:healthcare:saml:attribute:PersistentPseudonym", "1b2948e1-5aee-4c22-aed9-23za");
            AssertCanReadValue(attributeManager, "dk:healthcare:saml:attribute:SoleProprietorshipCVR", true);
            AssertCanReadValue(attributeManager, "dk:healthcare:saml:attribute:IsOver15", true);
            AssertCanReadValue(attributeManager, "dk:healthcare:saml:attribute:IsOver18", false);
            AssertCanReadValue(attributeManager, "dk:healthcare:saml:attribute:OnBehalfOf", "urn:dk:healthcare:saml:actThroughDelegationByAuthorizedHealthcareProfessional:userAuthorization:AuthorizationCode:341KY:EducationCode:5501");
            AssertCanReadValue(attributeManager, "dk:healthcare:saml:attribute:SystemName", "Det Fælles Medicinkort");
            AssertCanReadValue(attributeManager, "dk:healthcare:saml:attribute:SystemID", "FMK");
            AssertCanReadValue(attributeManager, "dk:healthcare:saml:attribute:SystemVersion", "3.2.1b");
            AssertCanReadValue(attributeManager, "dk:healthcare:saml:attribute:SystemVendorName", "IT-system leverandøren i Nørregade");
            AssertCanReadValue(attributeManager, "dk:healthcare:saml:attribute:SystemVendorID", "ITN");
            AssertCanReadValue(attributeManager, "dk:healthcare:saml:attribute:SystemOperationsOrganisationName", "Driftsorganisationen på bakken");
            AssertCanReadValue(attributeManager, "dk:healthcare:saml:attribute:SystemOperationsOrganisationID", "DPB");
            AssertCanReadValue(attributeManager, "dk:healthcare:saml:attribute:SystemUsingOrganisationName", "Region Midtjylland");
            AssertCanReadValue(attributeManager, "dk:healthcare:saml:attribute:SystemUsingOrganisationID", "RM");
        }



        public void AssertCanReadValue<T>(AttributeAdapter attributeAdapter, string attributeName, T expectedValue)
        {
            var accessor = (SamlAttributeMarshal<T>)typeof(CommonHealthcareAttributes).GetProperties(BindingFlags.Static | BindingFlags.Public).Select(x => x.GetValue(null)).OfType<SamlAttributeMarshal>().Single(x => x.Name == attributeName);
            var actualValue = accessor.GetValue(attributeAdapter);
            Assert.AreEqual(expectedValue, actualValue);
        }


        /// <summary>
        /// Determines whether this instance can read a token with a purposeOfUse element attribute in an AttributeStatement.
        /// </summary>
        [TestMethod, DeploymentItem("AssertionWithEveryHealthcareAttribute.xml")]
        public void CanReadPurposeOfUse()
        {
            // Arrange: Create a token with an AttributeStatement with a {purposeOfUse} element in an {AttributeValue} element

            var token = ReadAssertion("AssertionWithEveryHealthcareAttribute.xml");

            // Act

            var attributeManager = new AttributeStatementAttributeAdapter(token.Statements.OfType<Saml2ComplexAttributeStatement>().Single());
            var purposeOfUse = attributeManager.GetValue(CommonHealthcareAttributes.PurposeOfUse);

            // Assert

            Assert.AreEqual(PurposeOfUse.Treatment, purposeOfUse);
            Assert.IsTrue(PurposeOfUse.Treatment.Equals(purposeOfUse));
            Assert.AreEqual("Flollop", purposeOfUse.CodeSystemName);
            Assert.AreEqual("Behandling", purposeOfUse.DisplayName);
        }



        [TestMethod, DeploymentItem("AssertionWithEveryHealthcareAttribute.xml")]
        public void CanReadUserType()
        {
            // Arrange: Create a token with an AttributeStatement with a {purposeOfUse} element in an {AttributeValue} element

            var token = ReadAssertion("AssertionWithEveryHealthcareAttribute.xml");

            // Act

            var attributeManager = new AttributeStatementAttributeAdapter(token.Statements.OfType<Saml2ComplexAttributeStatement>().Single());
            var actual = attributeManager.GetValue(CommonHealthcareAttributes.UserType);

            // Assert

            Assert.AreEqual(UserType.Employee, actual);
        }







        private Saml2Assertion ReadAssertion(string fileName)
        {
            var xml = new FileInfo($@"{TestContext.DeploymentDirectory}\{fileName}").OpenText().ReadToEnd();
            var xr = XmlReader.Create(new StringReader(xml));
            var handler = new ExtendedSaml2SecurityTokenHandler
            {
                Configuration = new SecurityTokenHandlerConfiguration()
                {
                }
            };
            var token = (Saml2SecurityToken)handler.ReadToken(xr);
            return token.Assertion;
        }




    }
}
