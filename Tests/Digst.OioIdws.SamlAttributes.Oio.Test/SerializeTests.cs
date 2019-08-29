using System;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.Linq;
using Digst.OioIdws.Common.Utils;
using Digst.OioIdws.SamlAttributes.AttributeAdapters;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.SamlAttributes.Oio.Test
{
    [TestClass]
    public class SerializeTests
    {

        public TestContext TestContext { get; set; }


        private static readonly X509Certificate2 WebServiceConsumerCertificate;

        private static readonly X509Certificate2 MocesCertificate;


        static SerializeTests()
        {
            var machineStore = new System.Security.Cryptography.X509Certificates.X509Store(StoreLocation.LocalMachine);
            machineStore.Open(OpenFlags.ReadOnly);
            WebServiceConsumerCertificate = machineStore.Certificates
                .Find(X509FindType.FindByThumbprint, "0e6dbcc6efaaff72e3f3d824e536381b26deecf5", true)
                .Cast<X509Certificate2>().Single();
            machineStore.Close();

            var userStore = new X509Store(StoreLocation.CurrentUser);
            userStore.Open(OpenFlags.ReadOnly);
            MocesCertificate = userStore.Certificates
                .Find(X509FindType.FindByThumbprint, "c8c97200d5114f436691369e0f4ee4e9c0a0cf9c", true)
                .Cast<X509Certificate2>().Single();
            userStore.Close();

        }

        private static Saml2Assertion GetAssertion()
        {
            var builder = new AssertionBuilder("myEntityId")
                .HolderOfKeySubjectConfirmation(MocesCertificate)
                .SigningCertificate(WebServiceConsumerCertificate)
                .Duration(TimeSpan.FromHours(8))
                .Authentication("42", DateTime.UtcNow, null, dnsName: "localhost", address: "127.0.0.1")
                .SubjectNameId(MocesCertificate.SubjectName.Name);

            var assertion = builder.Build(DateTime.UtcNow);

            return assertion;
        }


        private static XDocument IssueToken(Saml2Assertion assertion)
        {
            var tokenHandler = new ExtendedSaml2SecurityTokenHandler();
            var doc = new XDocument();
            using (var xw = doc.CreateWriter())
            {
                tokenHandler.WriteToken(xw, new Saml2SecurityToken(assertion));
            }
            return doc;
        }


        [TestMethod]
        public void CanCreateAssertionWithAllOioAttributes()
        {
            var assertion = GetAssertion();
            var adapter = new AttributeStatementAttributeAdapter(assertion.Statements.OfType<Saml2ComplexAttributeStatement>().Single());

            adapter.SetValue(CommonOioAttributes.SurName, "Jensen");
            adapter.SetValue(CommonOioAttributes.CommonName, "Hans Jensen");
            adapter.SetValue(CommonOioAttributes.Uid, "JMogensen");
            adapter.SetValue(CommonOioAttributes.Email, "jens@email.dk");

            adapter.SetValue(CommonOioAttributes.AssuranceLevel, AssuranceLevel.Level2);
            adapter.SetValue(CommonOioAttributes.SpecVer, SpecVer.DkSaml20);

            adapter.SetValue(CommonOioAttributes.CvrNumberIdentifier, "20688092");
            adapter.SetValue(CommonOioAttributes.UniqueAccountKey, "xri://@DK-XRI*19-43-70-19/Borger*($d/2005-08-02T16:16:42+01:00Z)/OJEN");

            var bst = new XmlDocument();
            bst.LoadXml("<test/>");
            adapter.SetValue(CommonOioAttributes.DiscoveryEpr, bst);

            adapter.SetValue(CommonOioAttributes.CertificateSerialNumber, "234-2345-76745-23");

            adapter.SetValue(CommonOioAttributes.OrganizationName, "Pelles Pølsefabrik");
            adapter.SetValue(CommonOioAttributes.OrganizationUnit, "Kvalitetsafdelingen");
            adapter.SetValue(CommonOioAttributes.Title, "Chefkontrollant");
            adapter.SetValue(CommonOioAttributes.PostalAddress, "Kvægtorvet 5, 2150 Kødbyen");

            adapter.SetValue(CommonOioAttributes.OcesPseudonym, "mister x");
            adapter.SetValue(CommonOioAttributes.IsYouthCert, true);
            adapter.SetValue(CommonOioAttributes.UserCertificate, MocesCertificate);

            adapter.SetValue(CommonOioAttributes.PidNumberIdentifier, "9802-2002-2-9142544");
            adapter.SetValue(CommonOioAttributes.CprNumberIdentifier, "2702681273");
            adapter.SetValue(CommonOioAttributes.RidNumberIdentifier, "2342-345623423");

            adapter.SetValue(CommonOioAttributes.CertificateIssuer, "CN=TDC OCES CA,O=TDC,C=DK");


            adapter.SetValue(CommonOioAttributes.ProductionUnitIdentifier, "1202332283");
            adapter.SetValue(CommonOioAttributes.SeNumberIdentifier, "12092018");
            adapter.SetValue(CommonOioAttributes.UserAdministratorIndicator, false);

            var doc = IssueToken(assertion);

            TestContext.WriteLine(doc.ToString(SaveOptions.None));

            Assert.AreEqual("Assertion", doc.Root.Name.LocalName);
        }
    }
}
