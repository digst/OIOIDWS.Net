using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Digst.OioIdws.Healthcare.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.SamlAttributes.Healthcare.Test
{
    [TestClass]
    public class UserAuthorizationListTests
    {


        public TestContext TestContext { get; set; }

        [TestMethod]
        public void CanDeserialize()
        {
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<uap:UserAuthorizationList xmlns:uap=""urn:dk:healthcare:saml:user_authorization_profile:1.0"">
    <uap:UserAuthorization>
        <uap:AuthorizationCode>341KY</uap:AuthorizationCode>
        <uap:EducationCode>7170</uap:EducationCode>
        <uap:EducationType>Læge</uap:EducationType>
    </uap:UserAuthorization>
    <uap:UserAuthorization>
        <uap:AuthorizationCode>7AD6T</uap:AuthorizationCode>
    	<uap:EducationCode>5433</uap:EducationCode>
        <uap:EducationType>Tandlæge</uap:EducationType>
    </uap:UserAuthorization>
</uap:UserAuthorizationList>
";
            var xr = XmlReader.Create(new StringReader(xml));
            var ser = new XmlSerializer(typeof(UserAuthorizationList));

            var actual = (UserAuthorizationList)ser.Deserialize(xr);

            Assert.AreEqual(2, actual.UserAuthorizations.Count);

            Assert.AreEqual("341KY", actual.UserAuthorizations[0].AuthorizationCode);
            Assert.AreEqual("7170", actual.UserAuthorizations[0].EducationCode);
            Assert.AreEqual("Læge", actual.UserAuthorizations[0].EducationType);

            Assert.AreEqual("7AD6T", actual.UserAuthorizations[1].AuthorizationCode);
            Assert.AreEqual("5433", actual.UserAuthorizations[1].EducationCode);
            Assert.AreEqual("Tandlæge", actual.UserAuthorizations[1].EducationType);
        }


        [TestMethod]
        public void CanSerialize()
        {
            var valueUnderTest = new UserAuthorizationList()
            {
                UserAuthorizations = new Collection<UserAuthorization>()
                {
                    new UserAuthorization()
                    {
                        AuthorizationCode = "341KY",
                        EducationCode = "7170",
                        EducationType = "Læge",
                    }
                }
            };
            var uap = XNamespace.Get("urn:dk:healthcare:saml:user_authorization_profile:1.0");
            var serializer = new XmlSerializer(typeof(UserAuthorizationList));

            // Act
            var doc = new XDocument();
            using (var w = doc.CreateWriter())
            {
                serializer.Serialize(w, valueUnderTest, new XmlSerializerNamespaces(
                    new XmlQualifiedName[]
                    {
                        new XmlQualifiedName("", "urn:dk:healthcare:saml:user_authorization_profile:1.0")
                    }));
            }


            // Assert
            var expected = new XElement(uap + "UserAuthorizationList",
                    new XElement(uap + "UserAuthorization",
                        new XElement(uap + "AuthorizationCode", "341KY"),
                        new XElement(uap + "EducationCode", "7170"),
                        new XElement(uap + "EducationType", "Læge")
                    )
                );

            Assert.AreEqual(expected.ToString(), doc.Root.ToString());

        }
    }
}