using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.Linq;
using Digst.OioIdws.Common.Utils;
using Digst.OioIdws.Healthcare.Common;
using Digst.OioIdws.Healthcare.SamlAttributes;
using Digst.OioIdws.SamlAttributes.AttributeAdapters;
using Digst.OioIdws.SamlAttributes.AttributeMarshals;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.SamlAttributes.Healthcare.Test
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



        private AssertionBuilder GetAssertionBuilder()
        {
            var builder = new AssertionBuilder("myEntityId")
                .HolderOfKeySubjectConfirmation(MocesCertificate)
                .SigningCertificate(WebServiceConsumerCertificate)
                .Duration(TimeSpan.FromHours(8))
                .SubjectNameId(MocesCertificate.SubjectName.Name);
            return builder;
        }


        public void CanSerializeToSpecificAttributeValue<T>(SamlAttributeMarshal<T> marshaller, T value)
        {
            // Arrange
            var assertion = GetAssertionBuilder();
            var doc = new XDocument();

            // Act
            using (var xw = doc.CreateWriter())
            {
                new ExtendedSaml2SecurityTokenHandler().WriteToken(xw, new Saml2SecurityToken(assertion.WithAttribute(marshaller, value).Build()));
            }

            // Assert
            var actualElement = new XElement(doc.Root
                .Elements(Saml2Constants.ElementNames.AttributeStatementXName).Single()
                .Elements(Saml2Constants.ElementNames.AttributeXName).Single());

            var brutto = XElement.Load(Path.Combine(TestContext.DeploymentDirectory, "BruttoAttributeStatement.xml"));
            var expectedElement = new XElement(brutto.Elements(Saml2Constants.ElementNames.AttributeXName).Single(x => x.Attribute("Name")?.Value == marshaller.Name));
            //expectedElement.Add(new XAttribute(XNamespace.Xml+"xmlns" ,Saml2Constants.Namespaces.SamlString));

            foreach (var element in actualElement.DescendantsAndSelf())
            {
                element.ReplaceAttributes(element.Attributes().OrderBy(x => x.Name.ToString()).ToList());
            }

            foreach (var element in expectedElement.DescendantsAndSelf())
            {
                element.ReplaceAttributes(element.Attributes().OrderBy(x => x.Name.ToString()).ToList());
            }

            Assert.AreEqual(expectedElement.ToString(SaveOptions.None), actualElement.ToString(SaveOptions.None));
        }


        [TestMethod, DeploymentItem("BruttoAttributeStatement.xml")]
        public void CanCreateAttributeValueFromPurposeOfUse()
        {

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.PurposeOfUse, PurposeOfUse.Treatment);

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.SubjectId, "Hansine Jensen");

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.ECprNumberIdentifier, "251248A89F");

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.FidNumberIdentifier, "76794884");

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.OneTimePseudonym, "1b2948e1-5aee-4c22-aed9-23za");

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.PersistentPseudonym, "1b2948e1-5aee-4c22-aed9-23za");

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.SoleProprietorshipCvr, true);

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.ChildrenInCustody, new []{ "urn:dk:gov:saml:cprNumberIdentifier:1212120001", "urn:dk:gov:saml:cprNumberIdentifier:1212120002" } );

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.IsOver15, true);

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.IsOver18, false);

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.OnBehalfOf, "urn:dk:healthcare:saml:actThroughDelegationByAuthorizedHealthcareProfessional:userAuthorization:AuthorizationCode:341KY:EducationCode:5501");

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.UserType, UserType.Employee);

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.SubjectOrganization, "Region Hovedstaden");

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.SubjectOrganizationId, "https://www.regionh.dk");

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.HomeCommunityId, "urn:oid:1.2.208");

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.SubjectNpi, "33257872");

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.SubjectProviderIdentifier, new []
            {
                new SubjectProviderIdentifier("1.2.208.176.1.1", "8041000016000^Sydvestjysk Sygehus", "Sundhedsvæsenets Organisationsregister (SOR)"),
                new SubjectProviderIdentifier("1.2.208.176.2.3", "5501^Sydvestjysk Sygehus", "Sygehus-afdelingsklassifikation (SHAK)"),
            });

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.UserAuthorizationCode, "7AD6T");

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.UserEducationCode, "5166");

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.UserAuthorizations, new UserAuthorizationList()
            {
                UserAuthorizations = new Collection<UserAuthorization>()
                {
                    new UserAuthorization()
                    {
                        AuthorizationCode = "341KY",
                        EducationCode = "7170",
                        EducationType = "Læge",
                    },
                    new UserAuthorization()
                    {
                        AuthorizationCode = "7AD6T",
                        EducationCode = "5433",
                        EducationType = "Tandlæge",
                    }
                }
            });

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.HasUserAuthorization, true);

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.SubjectRole, new []
            {
                new SubjectRole("7170","1.2.208.176.1.3","Autorisationsregister","Læge"),
                new SubjectRole("5433","1.2.208.176.1.3","Autorisationsregister","Tandlæge"),
            });

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.IsRegisteredPharmacist, true);

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.PatientResourceId, "2512484916^^^&1.2.208.176.1.2&ISO");

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.EvidenceForPatientInCare, EvidenceForPatientInCare.DegreeAPlusPlus);

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.GivenConsent, GivenConsent.None);

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.SystemName, "Det Fælles Medicinkort");

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.SystemId, "FMK");

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.SystemVersion, "3.2.1b");

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.SystemVendorName, "IT-system leverandøren i Nørregade");

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.SystemVendorId, "ITN");

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.SystemOperationsOrganisationName, "Driftsorganisationen på bakken");

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.SystemOperationsOrganisationId, "DPB");

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.SystemUsingOrganisationName, "Region Midtjylland");

            CanSerializeToSpecificAttributeValue(CommonHealthcareAttributes.SystemUsingOrganisationId, "RM");

        }

        [TestMethod]
        public void CanCreateAssertionWithAllhealthcareAttributes()
        {
            // Arrange
            var assertion = GetAssertionBuilder().Build();
            var adapter = new AttributeStatementAttributeAdapter(assertion.Statements.OfType<Saml2ComplexAttributeStatement>().Single());

            // Act

            adapter.SetValue(CommonHealthcareAttributes.PurposeOfUse, PurposeOfUse.Treatment);

            adapter.SetValue(CommonHealthcareAttributes.SubjectId, "Hansine Jensen");

            adapter.SetValue(CommonHealthcareAttributes.ECprNumberIdentifier, "251248A89F");

            adapter.SetValue(CommonHealthcareAttributes.FidNumberIdentifier, "76794884");

            adapter.SetValue(CommonHealthcareAttributes.OneTimePseudonym, "1b2948e1-5aee-4c22-aed9-23za");

            adapter.SetValue(CommonHealthcareAttributes.PersistentPseudonym, "1b2948e1-5aee-4c22-aed9-23za");

            adapter.SetValue(CommonHealthcareAttributes.SoleProprietorshipCvr, true);

            var childrenInCustody = new[]
            {
                "urn:dk:gov:saml:cprNumberIdentifier:1212120001",
                "urn:dk:gov:saml:cprNumberIdentifier:1212120002"
            };
            adapter.SetValue(CommonHealthcareAttributes.ChildrenInCustody, childrenInCustody);

            adapter.SetValue(CommonHealthcareAttributes.IsOver15, true);

            adapter.SetValue(CommonHealthcareAttributes.IsOver18, true);

            adapter.SetValue(CommonHealthcareAttributes.OnBehalfOf, "urn:dk:healthcare:saml:actThroughDelegationByAuthorizedHealthcareProfessional:userAuthorization:AuthorizationCode:341KY:EducationCode:5501");

            adapter.SetValue(CommonHealthcareAttributes.UserType, UserType.Employee);

            adapter.SetValue(CommonHealthcareAttributes.SubjectOrganization, "Region Hovedstaden");

            adapter.SetValue(CommonHealthcareAttributes.SubjectOrganizationId, "https://www.regionh.dk");

            adapter.SetValue(CommonHealthcareAttributes.HomeCommunityId, "urn:oid:1.2.208");

            adapter.SetValue(CommonHealthcareAttributes.SubjectNpi, "33257872");

            var subjectProviderIdentifiers = new[]{
                new SubjectProviderIdentifier("1.2.208.176.1.1", "8041000016000^Sydvestjysk Sygehus", "Sundhedsvæsenets Organisationsregister (SOR)"),
                new SubjectProviderIdentifier("1.2.208.176.2.3", "5501^Sydvestjysk Sygehus", "Sygehus-afdelingsklassifikation (SHAK)")
            };
            adapter.SetValue(CommonHealthcareAttributes.SubjectProviderIdentifier, subjectProviderIdentifiers);

            adapter.SetValue(CommonHealthcareAttributes.UserAuthorizationCode, "7AD6T");

            adapter.SetValue(CommonHealthcareAttributes.UserEducationCode, "5166");

            adapter.SetValue(CommonHealthcareAttributes.UserEducationType, "Sygeplejerske");

            var userAuthorizationList = new UserAuthorizationList()
            {
                UserAuthorizations = new Collection<UserAuthorization>()
                {
                    new UserAuthorization()
                    {
                        AuthorizationCode = "341KY",
                        EducationCode = "7170",
                        EducationType = "Læge",
                    },
                    new UserAuthorization()
                    {
                        AuthorizationCode = "7AD6T",
                        EducationCode = "5433",
                        EducationType = "Tandlæge",
                    }
                }
            };
            adapter.SetValue(CommonHealthcareAttributes.UserAuthorizations, userAuthorizationList);

            adapter.SetValue(CommonHealthcareAttributes.HasUserAuthorization, false);

            var subjectRoles = new[]{
                new SubjectRole("7170", "1.2.208.176.1.3", "Autorisationsregister", "Læge"),
                new SubjectRole("5433","1.2.208.176.1.3","Autorisationsregister","Tandlæge"),
            };
            adapter.SetValue(CommonHealthcareAttributes.SubjectRole, subjectRoles);

            adapter.SetValue(CommonHealthcareAttributes.IsRegisteredPharmacist, false);

            adapter.SetValue(CommonHealthcareAttributes.PatientPrivacyPolicyIdentifier, "flop");

            adapter.SetValue(CommonHealthcareAttributes.PatientPrivacyPolicyAcknowledgementDocument, "https://www.example.org");

            adapter.SetValue(CommonHealthcareAttributes.PatientResourceId, "2512484916^^^&1.2.208.176.1.2&ISO");

            adapter.SetValue(CommonHealthcareAttributes.EvidenceForPatientInCare, EvidenceForPatientInCare.DegreeAPlusPlus);

            adapter.SetValue(CommonHealthcareAttributes.GivenConsent, GivenConsent.None);

            adapter.SetValue(CommonHealthcareAttributes.SystemName, "Det Fælles Medicinkort");

            adapter.SetValue(CommonHealthcareAttributes.SystemId, "FMK");

            adapter.SetValue(CommonHealthcareAttributes.SystemVersion, "3.2.1b");

            adapter.SetValue(CommonHealthcareAttributes.SystemVendorName, "IT-system leverandøren i Nørregade");

            adapter.SetValue(CommonHealthcareAttributes.SystemVendorId, "ITN");

            adapter.SetValue(CommonHealthcareAttributes.SystemOperationsOrganisationName, "Driftsorganisationen på bakken");

            adapter.SetValue(CommonHealthcareAttributes.SystemOperationsOrganisationId, "DPB");

            adapter.SetValue(CommonHealthcareAttributes.SystemUsingOrganisationName, "Region Midtjylland");

            adapter.SetValue(CommonHealthcareAttributes.SystemUsingOrganisationId, "RM");


            var doc = IssueToken(assertion);


            // Assert

            TestContext.WriteLine(doc.ToString(SaveOptions.None));


        }


    }
}
