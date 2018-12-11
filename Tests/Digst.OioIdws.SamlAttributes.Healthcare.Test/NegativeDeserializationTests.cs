using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Xml;
using Digst.OioIdws.Healthcare.Common;
using Digst.OioIdws.Healthcare.SamlAttributes;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.SamlAttributes.Healthcare.Test
{
    [TestClass]
    public class NegativeDeserializationTests
    {
        private AttributeStatementAttributeAdapter _attributeAdapter;


        public TestContext TestContext { get; set; }



        [TestInitialize]
        public void Initialize()
        {
            var xml = new FileInfo($@"{TestContext.DeploymentDirectory}\AssertionWithInvalidAttributeValues.xml").OpenText().ReadToEnd();
            var xr = XmlReader.Create(new StringReader(xml));
            var handler = new ExtendedSaml2SecurityTokenHandler
            {
                Configuration = new SecurityTokenHandlerConfiguration()
                {
                }
            };
            var token = (Saml2SecurityToken)handler.ReadToken(xr);
            _attributeAdapter = new AttributeStatementAttributeAdapter(token.Assertion.Statements.OfType<Saml2ComplexAttributeStatement>().Single());
        }


        [TestMethod, DeploymentItem("AssertionWithInvalidAttributeValues.xml")]
        public void FailureToReadUserAuthorizations()
        {
            try
            {
                var value = _attributeAdapter.GetValue(CommonHealthcareAttributes.UserAuthorizations);
                Assert.Fail("Exception was not thrown");
            }
            catch (InvalidOperationException x) when (x.Message.StartsWith("There is an error in XML document"))
            {
            }
        }

        [TestMethod, DeploymentItem("AssertionWithInvalidAttributeValues.xml")]
        public void FailureToReadHasUserAuthorization()
        {
            try
            {
                var value = _attributeAdapter.GetValue(CommonHealthcareAttributes.HasUserAuthorization);
                Assert.Fail("Exception was not thrown");
            }
            catch (ArgumentOutOfRangeException x) when (x.Message.StartsWith("Expected either \"true\" or \"false\""))
            {
            }
        }

        [TestMethod, DeploymentItem("AssertionWithInvalidAttributeValues.xml")]
        public void FailureToReadSubjectProviderIdentifier()
        {
            try
            {
                var value = _attributeAdapter.GetValue(CommonHealthcareAttributes.SubjectProviderIdentifier).ToList();
                Assert.Fail($"Exception was not thrown");
            }
            catch (InvalidOperationException x) when (x.Message.StartsWith("There is an error in XML document"))
            {
            }
        }

        [TestMethod, DeploymentItem("AssertionWithInvalidAttributeValues.xml")]
        public void FailureToReadSubjectRole()
        {
            try
            {
                var value = _attributeAdapter.GetValue(CommonHealthcareAttributes.SubjectRole).ToList();
                Assert.Fail($"Exception was not thrown");
            }
            catch (InvalidOperationException x) when (x.Message.StartsWith("There is an error in XML document"))
            {
            }
        }

        [TestMethod, DeploymentItem("AssertionWithInvalidAttributeValues.xml")]
        public void FailureToReadPurposeOfUse()
        {
            try
            {
                var value = _attributeAdapter.GetValue(CommonHealthcareAttributes.PurposeOfUse);
                Assert.Fail("Exception was not thrown");
            }
            catch (InvalidOperationException x) when (x.Message.StartsWith("There is an error in XML document"))
            {
            }
        }


        [TestMethod, DeploymentItem("AssertionWithInvalidAttributeValues.xml")]
        public void FailureToReadIsRegisteredPharmacist()
        {
            try
            {
                var value = _attributeAdapter.GetValue(CommonHealthcareAttributes.IsRegisteredPharmacist);
                Assert.Fail("Exception was not thrown");
            }
            catch (ArgumentOutOfRangeException x) when (x.Message.StartsWith("Expected either \"true\" or \"false\""))
            {
            }
        }


        [TestMethod, DeploymentItem("AssertionWithInvalidAttributeValues.xml")]
        public void FailureToReadEvidenceForPatientInCare()
        {
            var value = _attributeAdapter.GetValue(CommonHealthcareAttributes.EvidenceForPatientInCare);
            Assert.AreEqual(new EvidenceForPatientInCare("urn:dk:healthcare:brs:Q++"), value);
        }


        [TestMethod, DeploymentItem("AssertionWithInvalidAttributeValues.xml")]
        public void FailureToReadSoleProprietorshipCvr()
        {
            try
            {
                var value = _attributeAdapter.GetValue(CommonHealthcareAttributes.SoleProprietorshipCvr);
                Assert.Fail("Exception was not thrown");
            }
            catch (ArgumentOutOfRangeException x) when (x.Message.StartsWith("Expected either \"true\" or \"false\""))
            {
            }
        }


        [TestMethod, DeploymentItem("AssertionWithInvalidAttributeValues.xml")]
        public void FailureToReadIsOver15()
        {
            try
            {
                var value = _attributeAdapter.GetValue(CommonHealthcareAttributes.IsOver15);
                Assert.Fail("Exception was not thrown");
            }
            catch (ArgumentOutOfRangeException x) when (x.Message.StartsWith("Expected either \"true\" or \"false\""))
            {
            }
        }



        [TestMethod, DeploymentItem("AssertionWithInvalidAttributeValues.xml")]
        public void FailureToReadIsOver18()
        {
            try
            {
                var value = _attributeAdapter.GetValue(CommonHealthcareAttributes.IsOver18);
                Assert.Fail("Exception was not thrown");
            }
            catch (ArgumentOutOfRangeException x) when (x.Message.StartsWith("Expected either \"true\" or \"false\""))
            {
            }
        }


        [TestMethod, DeploymentItem("AssertionWithInvalidAttributeValues.xml")]
        public void FailureToReadUserType()
        {
            try
            {
                var value = _attributeAdapter.GetValue(CommonHealthcareAttributes.UserType);
                Assert.Fail("Exception was not thrown");
            }
            catch (KeyNotFoundException x) when (x.Message.StartsWith("The given key was not present in the dictionary."))
            {
            }
        }



        [TestMethod, DeploymentItem("AssertionWithInvalidAttributeValues.xml")]
        public void CanReadNonStandardGivenConsent()
        {
            var value = _attributeAdapter.GetValue(CommonHealthcareAttributes.GivenConsent);
            Assert.AreEqual(new GivenConsent("urn:dk:healthcare:consent:everything"), value);
        }




    }
}