using System;
using Digst.OioIdws.Common.Attributes;
using Digst.OioIdws.SamlAttributes.AttributeMarshals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.SamlAttributes.Test
{
    [TestClass]
    public class EnumSamlAttributeDescriptorTests
    {
        private enum MyEnum
        {
            [SamlAttributeValue("1")]
            Value1,

            [SamlAttributeValue("2")]
            Value2,
        }

        [TestMethod]
        public void CanSerializeAndDeserialize()
        {
            // Arrange
            var mgr = new InMemoryAttributeAdapter();
            var descriptorUnderTest = new EnumSamlAttributeMarshal<MyEnum>("urn:my-enum");

            // Act
            mgr.SetValue(descriptorUnderTest, MyEnum.Value2);
            var deserializedValue = mgr.GetValue(descriptorUnderTest);

            // Assert
            Assert.AreEqual(MyEnum.Value2, deserializedValue);
        }

    }
}
