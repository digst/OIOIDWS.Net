using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Digst.OioIdws.Common.Attributes;
using Digst.OioIdws.SamlAttributes.AttributeMarshals;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.SamlAttributes.Oio.Test
{
    [TestClass]
    public class DeserializeTests
    {

        public TestContext TestContext { get; set; }

        [TestMethod, DeploymentItem("AssertionWithEveryOioAttribute.xml")]
        public void CanReadStringBasedAttributes()
        {

            // Arrange

            var list = new Dictionary<string, string>()
            {
                {"urn:oid:2.5.4.4","Beeblebrox"},
                {"urn:oid:2.5.4.3","Zaphod" },
                {"urn:oid:0.9.2342.19200300.100.1.1","_0f78c1ce-1aaa-47d9-bd07-d0529f156163" },
                {"urn:oid:0.9.2342.19200300.100.1.3","zb@siriuscybernetics.com" },
                {"dk:gov:saml:attribute:CvrNumberIdentifier","20202020" },
                {"dk:gov:saml:attribute:UniqueAccountKey","47f05a8e-1a4b-4273-8760-8a388e76ede6" },
                {"urn:oid:2.5.4.5","serialnumber" },
                {"urn:oid:2.5.4.10","organizationName" },
                {"urn:oid:2.5.4.11","organizationUnit" },
                {"urn:oid:2.5.4.12","title" },
                {"urn:oid:2.5.4.16","postalAddress" },
                {"urn:oid:2.5.4.65","pseudonym" },
                {"dk:gov:saml:attribute:PidNumberIdentifier","1234567890" },
                {"dk:gov:saml:attribute:CprNumberIdentifier","1111111118" },
                {"dk:gov:saml:attribute:RidNumberIdentifier","12345678" },
                {"urn:oid:2.5.29.29","certificateIssuer" },
                {"dk:gov:saml:attribute:ProductionUnitIdentifier","ProductionUnitIdentifier" },
                {"dk:gov:saml:attribute:SENumberIdentifier","SENumberIdentifier" },
            };

            var assertion = ReadAssertion("AssertionWithEveryOioAttribute.xml");
            var attributeStatement = assertion.Statements.OfType<Saml2ComplexAttributeStatement>().Single();
            var manager = new AttributeStatementAttributeAdapter(attributeStatement);

            var accessors = typeof(CommonOioAttributes)
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(x => typeof(SamlAttributeMarshal<string>).IsAssignableFrom(x.FieldType))
                .Select(x => x.GetValue(null)).Cast<SamlAttributeMarshal<string>>().ToDictionary(acc => acc.Name);

            
            // Act and Assert

            foreach (var item in list)
            {
                Assert.IsTrue(accessors.ContainsKey(item.Key),$"No marshal found for attribute name \"{item.Key}\".");
                var accessor = accessors[item.Key];
                var expectedValue = item.Value;
                var actualValue = accessor.GetValue(manager);
                Assert.AreEqual(expectedValue, actualValue);
            }
        }



        [TestMethod, DeploymentItem("AssertionWithEveryOioAttribute.xml")]
        public void CanReadIsYouthCert()
        {
            // Arrange
            var assertion = ReadAssertion("AssertionWithEveryOioAttribute.xml");
            var attributeStatement = assertion.Statements.OfType<Saml2ComplexAttributeStatement>().Single();
            var manager = new AttributeStatementAttributeAdapter(attributeStatement);

            // Act
            var valueUnderTest = manager.GetValue(CommonOioAttributes.IsYouthCert);

            Assert.AreEqual(true, valueUnderTest);

        }



        [TestMethod, DeploymentItem("AssertionWithEveryOioAttribute.xml")]
        public void CanReadUserAdministratorIndicator()
        {
            // Arrange
            var assertion = ReadAssertion("AssertionWithEveryOioAttribute.xml");
            var attributeStatement = assertion.Statements.OfType<Saml2ComplexAttributeStatement>().Single();
            var manager = new AttributeStatementAttributeAdapter(attributeStatement);

            // Act
            var valueUnderTest = manager.GetValue(CommonOioAttributes.UserAdministratorIndicator);

            Assert.AreEqual(false, valueUnderTest);

        }





        [TestMethod, DeploymentItem("AssertionWithEveryOioAttribute.xml")]
        public void CanReadAssuranceLevel()
        {
            // Arrange
            var assertion = ReadAssertion("AssertionWithEveryOioAttribute.xml");
            var attributeStatement = assertion.Statements.OfType<Saml2ComplexAttributeStatement>().Single();
            var manager = new AttributeStatementAttributeAdapter(attributeStatement);

            // Act
            var valueUnderTest = manager.GetValue(CommonOioAttributes.AssuranceLevel);

            Assert.AreEqual(AssuranceLevel.Level3, valueUnderTest);

        }



        [TestMethod, DeploymentItem("AssertionWithEveryOioAttribute.xml")]
        public void CanReadDiscoveryEpr()
        {
            // Arrange
            var assertion = ReadAssertion("AssertionWithEveryOioAttribute.xml");
            var attributeStatement = assertion.Statements.OfType<Saml2ComplexAttributeStatement>().Single();
            var manager = new AttributeStatementAttributeAdapter(attributeStatement);

            // Act
            var valueUnderTest = manager.GetValue(CommonOioAttributes.DiscoveryEpr);

            Assert.AreEqual("Assertion", valueUnderTest.DocumentElement.LocalName);

        }


        [TestMethod, DeploymentItem("AssertionWithEveryOioAttribute.xml")]
        public void CanReadPrivilegesIntermediate()
        {
            // Arrange
            var assertion = ReadAssertion("AssertionWithEveryOioAttribute.xml");
            var attributeStatement = assertion.Statements.OfType<Saml2ComplexAttributeStatement>().Single();
            var adapter = new AttributeStatementAttributeAdapter(attributeStatement);

            // Act
            var privileges = adapter.GetValue(CommonOioAttributes.PrivilegesIntermediate);

            var group = privileges.PrivilegeGroups.Single(pg => pg.Scope == "urn:dk:gov:saml:cvrNumberIdentifier:12345678");
            var hasPrivilege = group.Privileges.Contains("urn:dk:some_domain:myPrivilege1A");

            Assert.IsTrue(hasPrivilege);
        }


        [TestMethod, DeploymentItem("AssertionWithEveryOioAttribute.xml")]
        public void CanReadUserCertificate()
        {
            // Arrange
            var assertion = ReadAssertion("AssertionWithEveryOioAttribute.xml");
            var attributeStatement = assertion.Statements.OfType<Saml2ComplexAttributeStatement>().Single();
            var manager = new AttributeStatementAttributeAdapter(attributeStatement);

            // Act
            var valueUnderTest = manager.GetValue(CommonOioAttributes.UserCertificate);

            // Assert
            Assert.AreEqual("C8C97200D5114F436691369E0F4EE4E9C0A0CF9C", valueUnderTest.Thumbprint.ToUpperInvariant());
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