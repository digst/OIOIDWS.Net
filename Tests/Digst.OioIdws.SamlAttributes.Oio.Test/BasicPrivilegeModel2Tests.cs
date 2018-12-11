using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Digst.OioIdws.Common.Attributes.BasicPrivilegesModel2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.SamlAttributes.Oio.Test
{
    [TestClass]
    public class BasicPrivilegeModel2Tests
    {
        [TestMethod]
        public void Serialize()
        {
            var list = new PrivilegeList()
            {
                PrivilegeGroups = new List<PrivilegeGroup>()
                {
                    new PrivilegeGroup()
                    {
                        Scope = "urn:dk:gov:saml:cvrNumberIdentifier:12345678",
                        Privileges = new List<string>()
                        {
                            "urn:dk:some_domain:myPrivilege1A",
                            "urn:dk:some_domain:myPrivilege1B"
                        }
                    },
                    new PrivilegeGroup()
                    {
                        Scope = "rn:dk:gov:saml:seNumberIdentifier:27384223",
                        Privileges = new List<string>()
                        {
                            "urn:dk:some_domain:myPrivilege1C",
                            "urn:dk:some_domain:myPrivilege1D"
                        }
                    }

                }
            };

            var ser = new XmlSerializer(typeof(PrivilegeList));

            var sw = new StringWriter();
            using (var xw = XmlWriter.Create(sw, new XmlWriterSettings() {Encoding = Encoding.UTF8}))
            {
                ser.Serialize(xw, list, new XmlSerializerNamespaces(new[]{ new XmlQualifiedName("", "http://itst.dk/oiosaml/basic_privilege_profile") }));
            }

            var actual = sw.ToString();

            var expected = "<?xml version=\"1.0\" encoding=\"utf-16\"?><PrivilegeList xmlns=\"http://itst.dk/oiosaml/basic_privilege_profile\"><PrivilegeGroup Scope=\"urn:dk:gov:saml:cvrNumberIdentifier:12345678\"><Privilege>urn:dk:some_domain:myPrivilege1A</Privilege><Privilege>urn:dk:some_domain:myPrivilege1B</Privilege></PrivilegeGroup><PrivilegeGroup Scope=\"rn:dk:gov:saml:seNumberIdentifier:27384223\"><Privilege>urn:dk:some_domain:myPrivilege1C</Privilege><Privilege>urn:dk:some_domain:myPrivilege1D</Privilege></PrivilegeGroup></PrivilegeList>";

            Assert.AreEqual(expected,actual);
        }



        [TestMethod]
        public void Deserialize()
        {
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<bpp:PrivilegeList
xmlns:bpp=""http://itst.dk/oiosaml/basic_privilege_profile"">
<bpp:PrivilegeGroup Scope=""urn:dk:gov:saml:cvrNumberIdentifier:12345678"">
<bpp:Privilege>urn:dk:some_domain:myPrivilege1A</bpp:Privilege>
<bpp:Privilege>urn:dk:some_domain:myPrivilege1B</bpp:Privilege>
</bpp:PrivilegeGroup>
<bpp:PrivilegeGroup Scope=""urn:dk:gov:saml:seNumberIdentifier:27384223"">
<bpp:Privilege>urn:dk:some_domain:myPrivilege1C</bpp:Privilege>
<bpp:Privilege>urn:dk:some_domain:myPrivilege1D</bpp:Privilege>
</bpp:PrivilegeGroup>
</bpp:PrivilegeList>";

            var ser = new XmlSerializer(typeof(PrivilegeList));

            var expected = new PrivilegeList()
            {
                PrivilegeGroups = new List<PrivilegeGroup>()
                {
                    new PrivilegeGroup()
                    {
                        Scope = "urn:dk:gov:saml:cvrNumberIdentifier:12345678",
                        Privileges = new List<string>()
                        {
                            "urn:dk:some_domain:myPrivilege1A",
                            "urn:dk:some_domain:myPrivilege1B"
                        }
                    },
                    new PrivilegeGroup()
                    {
                        Scope = "urn:dk:gov:saml:seNumberIdentifier:27384223",
                        Privileges = new List<string>()
                        {
                            "urn:dk:some_domain:myPrivilege1C",
                            "urn:dk:some_domain:myPrivilege1D"
                        }
                    }

                }
            };


            var actual = (PrivilegeList)ser.Deserialize(new StringReader(xml));

            Assert.AreEqual(expected.PrivilegeGroups.Count, actual.PrivilegeGroups.Count);
            Assert.AreEqual(expected.PrivilegeGroups[0].Scope, actual.PrivilegeGroups[0].Scope);
            Assert.AreEqual(expected.PrivilegeGroups[1].Scope, actual.PrivilegeGroups[1].Scope);

            Assert.AreEqual(expected.PrivilegeGroups[0].Privileges.Count, actual.PrivilegeGroups[0].Privileges.Count);
            Assert.AreEqual(expected.PrivilegeGroups[1].Privileges.Count, actual.PrivilegeGroups[1].Privileges.Count);

            Assert.AreEqual(expected.PrivilegeGroups[0].Privileges[0], actual.PrivilegeGroups[0].Privileges[0]);
            Assert.AreEqual(expected.PrivilegeGroups[0].Privileges[1], actual.PrivilegeGroups[0].Privileges[1]);
            Assert.AreEqual(expected.PrivilegeGroups[1].Privileges[0], actual.PrivilegeGroups[1].Privileges[0]);
            Assert.AreEqual(expected.PrivilegeGroups[1].Privileges[1], actual.PrivilegeGroups[1].Privileges[1]);

        }


    }
}
