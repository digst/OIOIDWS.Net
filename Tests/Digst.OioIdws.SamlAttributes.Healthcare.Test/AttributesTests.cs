using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Xml.Linq;
using Digst.OioIdws.Healthcare.Common;
using Digst.OioIdws.Healthcare.SamlAttributes;
using Digst.OioIdws.SamlAttributes.AttributeMarshals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.SamlAttributes.Healthcare.Test
{
    [TestClass]
    public class AttributesTests
    {
        [TestMethod]
        public void PurposeOfUseSerializeAndDeserialize()
        {

            // Arrange
            var attributeProvider = new InMemoryAttributeAdapter();
            var valueUnderTest = PurposeOfUse.Treatment;

            // Act: SetValue the value and get it again
            attributeProvider.SetValue(CommonHealthcareAttributes.PurposeOfUse, valueUnderTest);
            var reflectedValue = attributeProvider.GetValue(CommonHealthcareAttributes.PurposeOfUse);

            // Assert
            Assert.AreEqual("TREATMENT", reflectedValue.Code);


        }


        [TestMethod]
        public void CanSetComplexAttributeValue()
        {
            // Arrange
            var attributeProvider = new InMemoryAttributeAdapter();
            var valueUnderTest = PurposeOfUse.Treatment;

            attributeProvider.SetValue(CommonHealthcareAttributes.PurposeOfUse, valueUnderTest);
            var complexValue = attributeProvider.GetAttributeValues(CommonHealthcareAttributes.PurposeOfUse.Name);

            Assert.AreEqual(@"<AttributeValue xmlns=""urn:oasis:names:tc:SAML:2.0:assertion""><PurposeOfUse xsi:type=""CE"" code=""TREATMENT"" codeSystem=""urn:oasis:names:tc:xspa:1.0"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""urn:hl7-org:v3"" /></AttributeValue>",
                complexValue.Single().AttributeValueElement.ToString(SaveOptions.DisableFormatting));
        }


        [TestMethod]
        public void MultiStringSerializeAndDeserialize()
        {
            // Arrange
            var attributeProvider = new InMemoryAttributeAdapter();
            var valueUnderTest = new[] { "A", "B", "C" };
            var accessor = new MultiStringSamlAttributeMarshal("flollop");

            // Act: SetValue the value and get it again
            attributeProvider.SetValue(accessor, valueUnderTest);
            var reflectedValue = attributeProvider.GetValue(accessor).ToList();

            // Assert
            CollectionAssert.AreEqual(valueUnderTest, reflectedValue);

        }



        [TestMethod]
        public void ProviderIdentifier()
        {
            // Arrange
            var attributeProvider = new InMemoryAttributeAdapter();
            var valueUnderTest = new[]{
                new SubjectProviderIdentifier("Slartibartfast", "42,", "Sirius Cybernetics Corporation", true),
                new SubjectProviderIdentifier("Wowbagger", "54,", "Sirius Cybernetics Corporation", true),
            };
            var accessor = CommonHealthcareAttributes.SubjectProviderIdentifier;

            // Act: SetValue the value and get it again
            attributeProvider.SetValue(accessor, valueUnderTest);
            var reflectedValue = attributeProvider.GetValue(accessor).ToList();

            // Assert
            CollectionAssert.AreEqual(valueUnderTest, reflectedValue);

        }




        [TestMethod]
        public void SubjectRolesSerializeAndDeserialize()
        {
            // Arrange
            var attributeProvider = new InMemoryAttributeAdapter();
            var valueUnderTest = new[]{
                new SubjectRole("7170", "1.2.208.176.1.3", "Autorisationsregister", "Læge"),
                new SubjectRole("5433", "1.2.208.176.1.3-A", "Autorisationsregister-A", "Tandlæge"),
            };
            

            // Act: SetValue the value and get it again
            attributeProvider.SetValue(CommonHealthcareAttributes.SubjectRole, valueUnderTest);
            var reflectedValue = attributeProvider.GetValue(CommonHealthcareAttributes.SubjectRole).ToList();

            // Assert
            CollectionAssert.AreEqual(valueUnderTest, reflectedValue);
        }



        [TestMethod]
        public void Base64EncodedXElementSerializeAndDeserialize()
        {
            // Arrange
            var attributeProvider = new InMemoryAttributeAdapter();
            var valueUnderTest = XElement.Parse(@"
<bpp:PrivilegeList xmlns:bpp=""http://itst.dk/oiosaml/basic_privilege_profile"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" >
  <PrivilegeGroup Scope=""urn:dk:gov:saml:cprNumberIdentifier:2512484916"">
    <Privilege>urn:dk:fmk:view</Privilege>
    <Privilege>urn:dk:minlog:view</Privilege>
  </PrivilegeGroup>
  <PrivilegeGroup Scope=""urn:dk:gov:saml:cprNumberIdentifier:1111111118"">
    <Privilege>urn:dk:fmk:view</Privilege>
  </PrivilegeGroup>
</bpp:PrivilegeList>");
            var accessor = new XElementBase64SamlAttributeMarshal("dk:gov:saml:attribute:Privileges_intermediate");

            // Act: SetValue the value and get it again
            attributeProvider.SetValue(accessor, valueUnderTest);
            var reflectedValue = attributeProvider.GetValue(accessor);

            // Assert
            Assert.AreEqual(valueUnderTest.ToString(SaveOptions.DisableFormatting), reflectedValue.ToString(SaveOptions.DisableFormatting));

        }
    }
}
