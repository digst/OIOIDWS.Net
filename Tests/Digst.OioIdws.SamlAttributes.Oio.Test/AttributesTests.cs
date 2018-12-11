using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.SamlAttributes.Oio.Test
{
    [TestClass]
    public class AttributesTests
    {
        [TestMethod]
        public void SurNameSerializeAndDeserialize()
        {

            // Arrange
            var attributeProvider = new InMemoryAttributeAdapter();
            var valueUnderTest = "Beeblebrox";

            // Act: SetValue the value and get it again
            attributeProvider.SetValue(CommonOioAttributes.SurName, valueUnderTest);
            var reflectedValue = attributeProvider.GetValue(CommonOioAttributes.SurName);

            // Assert
            Assert.AreEqual(valueUnderTest, reflectedValue);
        }



    }
}
