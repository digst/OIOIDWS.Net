using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Digst.OioIdws.Healthcare.Common;
using Digst.OioIdws.SamlAttributes.AttributeMarshals;

namespace Digst.OioIdws.Healthcare.SamlAttributes
{


    /// <summary>
    /// Attribute marshals for the common healthcare attributes. (ref: Common SAML attributes for Healthcare Version 0.9).
    /// </summary>
    public static class CommonHealthcareAttributes
    {

        /// <summary>
        /// User authorization code
        ///
        /// SAML name: dk:healthcare:saml:attribute:UserAuthorizationCode
        /// </summary>
        public static SamlAttributeMarshal<string> UserAuthorizationCode { get; } = 
            new StringSamlAttributeMarshal("dk:healthcare:saml:attribute:UserAuthorizationCode");

        /// <summary>
        /// The user education code
        ///
        /// SAML name: dk:healthcare:saml:attribute:UserEducationCode
        /// </summary>
        public static SamlAttributeMarshal<string> UserEducationCode { get; } = 
            new StringSamlAttributeMarshal("dk:healthcare:saml:attribute:UserEducationCode");

        /// <summary>
        /// The user education type
        ///
        /// SAML name: dk:healthcare:saml:attribute:UserEducationType
        /// </summary>
        public static SamlAttributeMarshal<string> UserEducationType { get; } = 
            new StringSamlAttributeMarshal("dk:healthcare:saml:attribute:UserEducationType");

        /// <summary>
        /// The user authorizations. This attribute contains a list of healthcare authorizations the user has been granted by the Department of Health.
        /// The attribute MUST NOT contain other authorizationvalues besides the values from the Department of Health’s
        /// authorization register.
        ///
        /// SAML name: dk:healthcare:saml:attribute:UserAuthorizations
        /// </summary>
        public static SamlAttributeMarshal<UserAuthorizationList> UserAuthorizations { get; } = 
            new XmlSerializableBase64SamlAttributeMarshal<UserAuthorizationList>("dk:healthcare:saml:attribute:UserAuthorizations");

        /// <summary>
        /// This attribute reflects whether the subject denoted by this assertion has been granted a healthcare authorization by the Department of Health.
        ///
        /// SAML name: dk:healthcare:saml:attribute:HasUserAuthorization
        /// </summary>
        public static SamlAttributeMarshal<bool> HasUserAuthorization { get; } = 
            new BooleanSamlAttributeMarshal("dk:healthcare:saml:attribute:HasUserAuthorization");

        /// <summary>
        /// The subject ID
        ///
        /// SAML name: urn:oasis:names:tc:xspa:1.0:subject:subject-id
        /// </summary>
        public static SamlAttributeMarshal<string> SubjectId { get; } = 
            new StringSamlAttributeMarshal("urn:oasis:names:tc:xspa:1.0:subject:subject-id");

        /// <summary>
        /// The organization.
        ///
        /// SAML name: urn:oasis:names:tc:xspa:1.0:subject:organization
        /// </summary>
        public static SamlAttributeMarshal<string> SubjectOrganization { get; } =
            new StringSamlAttributeMarshal("urn:oasis:names:tc:xspa:1.0:subject:organization");

        /// <summary>
        /// The organization identifier
        ///
        /// SAML name: urn:oasis:names:tc:xspa:1.0:subject:organization-id
        /// </summary>
        public static SamlAttributeMarshal<string> SubjectOrganizationId { get; } =
            new StringSamlAttributeMarshal("urn:oasis:names:tc:xspa:1.0:subject:organization-id");

        /// <summary>
        /// The home community identifier
        ///
        /// SAML name: urn:ihe:iti:xca:2010:homeCommunityId
        /// </summary>
        public static SamlAttributeMarshal<string> HomeCommunityId { get; } = 
            new StringSamlAttributeMarshal("urn:ihe:iti:xca:2010:homeCommunityId");

        /// <summary>
        /// The subject non persistent identifier
        ///
        /// SAML name: urn:oasis:names:tc:xspa:1.0:subject:npi
        /// </summary>
        public static SamlAttributeMarshal<string> SubjectNpi { get; } = 
            new StringSamlAttributeMarshal("urn:oasis:names:tc:xspa:1.0:subject:npi");

        /// <summary>
        /// The subject provider identifier
        ///
        /// SAML name: urn:ihe:iti:xua:2017:subject:provider-identifier
        /// </summary>
        public static SamlAttributeMarshal<IEnumerable<SubjectProviderIdentifier>> SubjectProviderIdentifier { get; } = 
            new MultiXmlSerializableAttributeMarshal<SubjectProviderIdentifier>("urn:ihe:iti:xua:2017:subject:provider-identifier");

        /// <summary>
        /// The subject role
        ///
        /// SAML name: urn:oasis:names:tc:xacml:2.0:subject:role
        /// </summary>
        public static SamlAttributeMarshal<IEnumerable<SubjectRole>> SubjectRole { get; } = 
            new MultiXmlSerializableAttributeMarshal<SubjectRole>("urn:oasis:names:tc:xacml:2.0:subject:role");

        /// <summary>
        /// The patient privacy policy acknowledgement document
        ///
        /// SAML name: urn:ihe:iti:bppc:2007:docid
        /// </summary>
        public static SamlAttributeMarshal<string> PatientPrivacyPolicyAcknowledgementDocument { get; } = 
            new StringSamlAttributeMarshal("urn:ihe:iti:bppc:2007:docid");

        /// <summary>
        /// The patient privacy policy identifier
        /// 
        /// SAML name: urn:ihe:iti:xua:2012:acp
        /// </summary>
        public static SamlAttributeMarshal<string> PatientPrivacyPolicyIdentifier { get; } = 
            new StringSamlAttributeMarshal("urn:ihe:iti:xua:2012:acp");

        /// <summary>
        /// The patient identifier
        ///
        /// SAML name: urn:oasis:names:tc:xacml:2.0:resource:resource-id
        /// </summary>
        public static SamlAttributeMarshal<string> PatientResourceId { get; } = 
            new StringSamlAttributeMarshal("urn:oasis:names:tc:xacml:2.0:resource:resource-id");

        /// <summary>
        /// The purpose of use. The usage context for the assertion in relation to the patient 
        ///
        /// SAML name: urn:oasis:names:tc:xspa:1.0:subject:purposeofuse
        /// </summary>
        public static SamlAttributeMarshal<PurposeOfUse> PurposeOfUse { get; } = 
            new XmlSerializableSamlAttributeMarshal<PurposeOfUse>("urn:oasis:names:tc:xspa:1.0:subject:purposeofuse");

        /// <summary>
        /// This attribute is used to denote that user is a registered pharmacists in the Danish Pharmacist Register (‘Apoteksregister’).
        ///
        /// SAML name: dk:healthcare:saml:attribute:IsRegisteredPharmacist
        /// </summary>
        public static SamlAttributeMarshal<bool> IsRegisteredPharmacist { get; } = 
            new BooleanSamlAttributeMarshal("dk:healthcare:saml:attribute:IsRegisteredPharmacist");

        /// <summary>
        /// The evidence for patient in care
        ///
        /// SAML name: dk:healthcare:saml:attribute:EvidenceForPatientInCare
        /// </summary>
        public static SamlAttributeMarshal<EvidenceForPatientInCare> EvidenceForPatientInCare { get; } = 
            new MappingSamlAttributeMarshal<EvidenceForPatientInCare>("dk:healthcare:saml:attribute:EvidenceForPatientInCare", 
                value => value.SamlAttributeValue, s => new EvidenceForPatientInCare(s));

        /// <summary>
        /// The ECPR number identifier
        ///
        /// SAML name: dk:healthcare:saml:attribute:ECprNumberIdentifier
        /// </summary>
        public static SamlAttributeMarshal<string> ECprNumberIdentifier { get; } = 
            new StringSamlAttributeMarshal("dk:healthcare:saml:attribute:ECprNumberIdentifier");

        /// <summary>
        /// The fid number identifier
        ///
        /// SAML name: dk:healthcare:saml:attribute:FidNumberIdentifier
        /// </summary>
        public static SamlAttributeMarshal<string> FidNumberIdentifier { get; } = 
            new StringSamlAttributeMarshal("dk:healthcare:saml:attribute:FidNumberIdentifier");

        /// <summary>
        /// The one time pseudonym
        ///
        /// SAML name: dk:healthcare:saml:attribute:OneTimePseudonym
        /// </summary>
        public static SamlAttributeMarshal<string> OneTimePseudonym { get; } = 
            new StringSamlAttributeMarshal("dk:healthcare:saml:attribute:OneTimePseudonym");

        /// <summary>
        /// The persistent pseudonym
        ///
        /// SAML name: dk:healthcare:saml:attribute:PersistentPseudonym
        /// </summary>
        public static SamlAttributeMarshal<string> PersistentPseudonym { get; } = 
            new StringSamlAttributeMarshal("dk:healthcare:saml:attribute:PersistentPseudonym");

        /// <summary>
        /// The sole proprietorship CVR
        ///
        /// SAML name: dk:healthcare:saml:attribute:SoleProprietorshipCVR
        /// </summary>
        public static SamlAttributeMarshal<bool> SoleProprietorshipCvr { get; } = 
            new BooleanSamlAttributeMarshal("dk:healthcare:saml:attribute:SoleProprietorshipCVR");

        /// <summary>
        /// The children in custody
        ///
        /// SAML name: dk:healthcare:saml:attribute:ChildrenInCustody
        /// </summary>
        public static SamlAttributeMarshal<IEnumerable<string>> ChildrenInCustody { get; } = 
            new MultiStringSamlAttributeMarshal("dk:healthcare:saml:attribute:ChildrenInCustody");

        /// <summary>
        /// An attribute used to denote whether the users (the Subject) age is above 15 years old.
        ///
        /// SAML name: dk:healthcare:saml:attribute:IsOver15
        /// </summary>
        public static SamlAttributeMarshal<bool> IsOver15 { get; } = 
            new BooleanSamlAttributeMarshal("dk:healthcare:saml:attribute:IsOver15");

        /// <summary>
        /// An attribute used to denote whether the users (the Subject) age is above 18 years old.
        /// 
        /// SAML name: dk:healthcare:saml:attribute:IsOver18
        /// </summary>
        public static SamlAttributeMarshal<bool> IsOver18 { get; } = 
            new BooleanSamlAttributeMarshal("dk:healthcare:saml:attribute:IsOver18");

        /// <summary>
        /// This attribute is used to denote that the subject of the assertion is acting on behalf of another user.
        /// Acting on behalf of another user is normally bounded in national legislation, this attribute MUST
        /// refer to the precise legislation.
        ///
        /// SAML name: dk:healthcare:saml:attribute:OnBehalfOf
        /// </summary>
        public static SamlAttributeMarshal<string> OnBehalfOf { get; } = 
            new StringSamlAttributeMarshal("dk:healthcare:saml:attribute:OnBehalfOf");

        /// <summary>
        /// The user type
        ///
        /// SAML name: dk:healthcare:saml:attribute:UserType
        /// </summary>
        public static SamlAttributeMarshal<UserType> UserType { get; } = 
            new EnumSamlAttributeMarshal<UserType>("dk:healthcare:saml:attribute:UserType");

        /// <summary>
        /// The given consent
        ///
        /// SAML name: dk:healthcare:saml:attribute:GivenConsent
        /// </summary>
        public static SamlAttributeMarshal<GivenConsent> GivenConsent { get; } = 
            new MappingSamlAttributeMarshal<GivenConsent>("dk:healthcare:saml:attribute:GivenConsent", v => v.SamlAttributeValue, s => new GivenConsent(s));

        /// <summary>
        /// The system name
        ///
        /// SAML name: dk:healthcare:saml:attribute:SystemName
        /// </summary>
        public static SamlAttributeMarshal<string> SystemName { get; } = 
            new StringSamlAttributeMarshal("dk:healthcare:saml:attribute:SystemName");

        /// <summary>
        /// The system identifier
        ///
        /// SAML name: dk:healthcare:saml:attribute:SystemID
        /// </summary>
        public static SamlAttributeMarshal<string> SystemId { get; } = 
            new StringSamlAttributeMarshal("dk:healthcare:saml:attribute:SystemID");

        /// <summary>
        /// The system version
        ///
        /// SAML name: dk:healthcare:saml:attribute:SystemVersion
        /// </summary>
        public static SamlAttributeMarshal<string> SystemVersion { get; } = 
            new StringSamlAttributeMarshal("dk:healthcare:saml:attribute:SystemVersion");

        /// <summary>
        /// The system vendor name
        ///
        /// SAML name: dk:healthcare:saml:attribute:SystemVendorName
        /// </summary>
        public static SamlAttributeMarshal<string> SystemVendorName { get; } = 
            new StringSamlAttributeMarshal("dk:healthcare:saml:attribute:SystemVendorName");

        /// <summary>
        /// The system vendor identifier
        ///
        /// SAML name: dk:healthcare:saml:attribute:SystemVendorID
        /// </summary>
        public static SamlAttributeMarshal<string> SystemVendorId { get; } = 
            new StringSamlAttributeMarshal("dk:healthcare:saml:attribute:SystemVendorID");

        /// <summary>
        /// The system operations organisation name
        ///
        /// SAML name: dk:healthcare:saml:attribute:SystemOperationsOrganisationName
        /// </summary>
        public static SamlAttributeMarshal<string> SystemOperationsOrganisationName { get; } = 
            new StringSamlAttributeMarshal("dk:healthcare:saml:attribute:SystemOperationsOrganisationName");

        /// <summary>
        /// The system operations organisation identifier
        ///
        /// SAML name: dk:healthcare:saml:attribute:SystemOperationsOrganisationID
        /// </summary>
        public static SamlAttributeMarshal<string> SystemOperationsOrganisationId { get; } = 
            new StringSamlAttributeMarshal("dk:healthcare:saml:attribute:SystemOperationsOrganisationID");

        /// <summary>
        /// The using system organisation name
        ///
        /// SAML name: dk:healthcare:saml:attribute:SystemUsingOrganisationName
        /// </summary>
        public static SamlAttributeMarshal<string> SystemUsingOrganisationName { get; } = 
            new StringSamlAttributeMarshal("dk:healthcare:saml:attribute:SystemUsingOrganisationName");

        /// <summary>
        /// The using system organisation identifier
        ///
        /// SAML name: dk:healthcare:saml:attribute:SystemUsingOrganisationID
        /// </summary>
        public static SamlAttributeMarshal<string> SystemUsingOrganisationId { get; } = 
            new StringSamlAttributeMarshal("dk:healthcare:saml:attribute:SystemUsingOrganisationID");


    }
}