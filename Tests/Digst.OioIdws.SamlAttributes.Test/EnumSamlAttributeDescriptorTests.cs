using System;
using System.Linq;
using System.Xml.Linq;
using Digst.OioIdws.SamlAttributes.AttributeAdapters;
using Digst.OioIdws.SamlAttributes.AttributeMarshals;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.SamlAttributes.Test
{
    [TestClass]
    public class EnumSamlAttributeDescriptorTests
    {

        private const string ThreeIsACrowd = "Three's a crowd";
        private const string TwoIsAPair = "Two's a pair";


        private enum MyEnum
        {
            [SamlAttributeValue("1")]
            Value1,

            [SamlAttributeValue(TwoIsAPair)]
            Value2,

            [SamlAttributeValue(ThreeIsACrowd)]
            Value3,
        }


        [TestMethod]
        public void CanSerializeAnEnum()
        {
            // Arrange
            var attributeAdapter = new InMemoryAttributeAdapter();
            var descriptorUnderTest = new EnumSamlAttributeMarshal<MyEnum>("urn:my-enum");

            // Act
            attributeAdapter.SetValue(descriptorUnderTest, MyEnum.Value2);
            var serializedValue = attributeAdapter.GetAttributeValues("urn:my-enum").Single().AttributeValueElement.Value;

            // Assert
            Assert.AreEqual(TwoIsAPair, serializedValue);
        }


        [TestMethod]
        public void CanDeserializeAnEnum()
        {
            // Arrange
            var attributeAdapter = new InMemoryAttributeAdapter();
            var descriptorUnderTest = new EnumSamlAttributeMarshal<MyEnum>("urn:my-enum");

            // Act
            attributeAdapter.SetAttributeValues("urn:my-enum", null, null, new []{new ComplexSamlAttributeValue(new XText(ThreeIsACrowd)) }, null);
            var deserializedValue = attributeAdapter.GetValue(descriptorUnderTest);

            // Assert
            Assert.AreEqual(MyEnum.Value3, deserializedValue);
        }

    }
}
