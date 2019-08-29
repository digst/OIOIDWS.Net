using System;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.Linq;
using Digst.OioIdws.SamlAttributes.AttributeMarshals;
using Digst.OioIdws.SamlAttributes.BasicPrivilegesModel2;

namespace Digst.OioIdws.SamlAttributes
{
    /// <summary>
    ///     Defines a number of standard attributes used by the OIO-SAMl profile
    /// </summary>
    public static class CommonOioAttributes
    {

        private static readonly XName XsString = XName.Get("string", "http://www.w3.org/2001/XMLSchema");
        private static readonly Uri BasicNameFormat = new Uri("urn:oasis:names:tc:SAML:2.0:attrname-format:basic");

        /// <summary>
        ///     User SurName
        /// 
        ///     SAML name: urn:oid:2.5.4.4
        /// </summary>
        public static SamlAttributeMarshal<string> SurName { get; } = 
            new StringSamlAttributeMarshal("urn:oid:2.5.4.4", BasicNameFormat, XsString);

        /// <summary>
        ///     User CommonName
        /// 
        ///     SAML name: urn:oid:2.5.4.3
        /// </summary>
        public static SamlAttributeMarshal<string> CommonName { get; } = 
            new StringSamlAttributeMarshal("urn:oid:2.5.4.3", BasicNameFormat, XsString);

        /// <summary>
        ///     User ID
        /// 
        ///     SAML name: urn:oid:0.9.2342.19200300.100.1.1
        /// </summary>
        public static SamlAttributeMarshal<string> Uid { get; } = 
            new StringSamlAttributeMarshal("urn:oid:0.9.2342.19200300.100.1.1", BasicNameFormat, XsString);

        /// <summary>
        ///     User email address
        /// 
        ///     SAML name: urn:oid:0.9.2342.19200300.100.1.3
        /// </summary>
        public static SamlAttributeMarshal<string> Email { get; } = 
            new StringSamlAttributeMarshal("urn:oid:0.9.2342.19200300.100.1.3", BasicNameFormat, XsString);

        /// <summary>
        ///     Assurancelevel - indicates the level of confidence in the claimed identity of the user.
        /// 
        ///     SAML name: dk:gov:saml:attribute:AssuranceLevel
        /// </summary>
        public static SamlAttributeMarshal<AssuranceLevel> AssuranceLevel { get; } = 
            new EnumSamlAttributeMarshal<AssuranceLevel>("dk:gov:saml:attribute:AssuranceLevel", BasicNameFormat, XsString);

        /// <summary>
        ///     Specification Version
        ///     The SpecVer attribute tells the Service Provider which version of the OIOSAML profile the assertion was issued
        ///     under.
        ///     The current value is “DK-SAML-2.0”. This makes it easier to change the profile in the future without hurting
        ///     backwards
        ///     compatibility.
        /// 
        ///     SAML name: dk:gov:saml:attribute:SpecVer
        /// </summary>
        public static SamlAttributeMarshal<SpecVer> SpecVer { get; } = 
            new MappingSamlAttributeMarshal<SpecVer>("dk:gov:saml:attribute:SpecVer", x => x.VersionIdentifier, s => new SpecVer(s), BasicNameFormat, XsString);

        /// <summary>
        ///     CVR number
        ///     The cvrNumberIdentifier Attribute is used to represent the organization where the subject is employed
        /// 
        ///     SAML name: dk:gov:saml:attribute:CvrNumberIdentifier
        /// </summary>
        public static SamlAttributeMarshal<string> CvrNumberIdentifier { get; } = 
            new StringSamlAttributeMarshal("dk:gov:saml:attribute:CvrNumberIdentifier", BasicNameFormat, XsString);

        /// <summary>
        ///     Unique account key
        ///     The uniqueAccountKey Attribute contains an account ID that is unique across organizations
        /// 
        ///     SAML name: dk:gov:saml:attribute:UniqueAccountKey
        /// </summary>
        public static SamlAttributeMarshal<string> UniqueAccountKey { get; } = 
            new StringSamlAttributeMarshal("dk:gov:saml:attribute:UniqueAccountKey", BasicNameFormat, XsString);

        /// <summary>
        ///     The bootstrap token (BST)
        ///     In order to facilitate discovery of a Liberty Discovery Service, the DiscoveryEPR attribute defined in [LibDiscov]
        ///     is included as an optional attribute in OIOSAML.
        /// 
        ///     SAML name: urn:liberty:disco:2006-08:DiscoveryEPR
        /// </summary>
        public static SamlAttributeMarshal<XmlDocument> DiscoveryEpr { get; } = 
            new XmlDocumentBase64SamlAttributeMarshal("urn:liberty:disco:2006-08:DiscoveryEPR", BasicNameFormat);

        /// <summary>
        ///     Certificate Serial Number
        /// 
        ///     SAML name: urn:oid:2.5.4.5
        /// </summary>
        public static SamlAttributeMarshal<string> CertificateSerialNumber { get; } = 
            new StringSamlAttributeMarshal("urn:oid:2.5.4.5", BasicNameFormat, XsString);

        /// <summary>
        ///     Organization name
        ///     This attribute is mandatory for companies and employees and contains the name of the organization.
        /// 
        ///     SAML name: urn:oid:2.5.4.10
        /// </summary>
        public static SamlAttributeMarshal<string> OrganizationName { get; } = 
            new StringSamlAttributeMarshal("urn:oid:2.5.4.10", BasicNameFormat, XsString);

        /// <summary>
        ///     Organizational unit. This optional attribute contains the name of the department within an organization.
        /// 
        ///     SAML name: urn:oid:2.5.4.11
        /// </summary>
        public static SamlAttributeMarshal<string> OrganizationUnit { get; } = 
            new StringSamlAttributeMarshal("urn:oid:2.5.4.11", BasicNameFormat, XsString);

        /// <summary>
        ///     The title of an employee
        /// 
        ///     SAML name: urn:oid:2.5.4.12
        /// </summary>
        public static SamlAttributeMarshal<string> Title { get; } = 
            new StringSamlAttributeMarshal("urn:oid:2.5.4.12", BasicNameFormat, XsString);

        /// <summary>
        ///     Postal address.
        ///     The optional postal address contains the address where a company or person is registered
        /// 
        ///     SAML name: urn:oid:2.5.4.16
        /// </summary>
        public static SamlAttributeMarshal<string> PostalAddress { get; } = 
            new StringSamlAttributeMarshal("urn:oid:2.5.4.16", BasicNameFormat, XsString);

        /// <summary>
        ///     OCES pseudonym
        ///
        ///     
        /// </summary>
        public static SamlAttributeMarshal<string> OcesPseudonym { get; } = 
            new StringSamlAttributeMarshal("urn:oid:2.5.4.65", BasicNameFormat, XsString);

        /// <summary>
        ///     Indicates that the certificate used was a youth certificate
        ///     dk:gov:saml:attribute:IsYouthCert
        /// </summary>
        public static SamlAttributeMarshal<bool> IsYouthCert { get; } = 
            new BooleanSamlAttributeMarshal("dk:gov:saml:attribute:IsYouthCert", BasicNameFormat, XsString);

        /// <summary>
        ///     User Certificate
        /// </summary>
        public static SamlAttributeMarshal<X509Certificate2> UserCertificate { get; } = 
            new MappingSamlAttributeMarshal<X509Certificate2>("urn:oid:1.3.6.1.4.1.1466.115.121.1.8",
                cert => Convert.ToBase64String(cert.Export(X509ContentType.Cert)),
                s => new X509Certificate2(Convert.FromBase64String(s)), BasicNameFormat, XsString);

        /// <summary>
        ///     PID number.
        ///     For OCES person certificates, the most interesting attribute is the PID number which contains a unique identifier
        ///     for the person.
        ///     The advantage of PID numbers over CPR numbers is that they can be freely exchanged without risk of
        ///     violating personal data protection acts. A Service Provider receiving a PID number can subsequently ask the user
        ///     for his CPR number and validate the PID-CPR correspondence by contacting the Certificate Authority.
        ///     Alternatively, if the Service Provider is a Government institution with authority to look up CPR numbers it can be
        ///     done directly without user interaction. With this scheme, the Identity Provider is thus able to transfer the CPR
        ///     number indirectly. The CPR number is generally a very useful attribute since many systems use it as identifier or
        ///     primary key.
        ///     The PID number is mandatory if the user has authenticated using a person certificate
        ///     dk:gov:saml:attribute:PidNumberIdentifier
        /// </summary>
        public static SamlAttributeMarshal<string> PidNumberIdentifier { get; } = 
            new StringSamlAttributeMarshal("dk:gov:saml:attribute:PidNumberIdentifier", BasicNameFormat, XsString);

        /// <summary>
        ///     CPR number
        ///     dk:gov:saml:attribute:CprNumberIdentifier
        /// </summary>
        public static SamlAttributeMarshal<string> CprNumberIdentifier { get; } =
            new StringSamlAttributeMarshal("dk:gov:saml:attribute:CprNumberIdentifier", BasicNameFormat, XsString);

        /// <summary>
        ///     Employee Number / RID
        ///     This attribute is mandatory when the user has authenticated with an employee certificate
        ///     dk:gov:saml:attribute:RidNumberIdentifier
        /// </summary>
        public static SamlAttributeMarshal<string> RidNumberIdentifier { get; } =
            new StringSamlAttributeMarshal("dk:gov:saml:attribute:RidNumberIdentifier", BasicNameFormat, XsString);

        /// <summary>
        ///     Certificate Issuer
        ///     Certificate serial numbers are not guaranteed to be unique across OCES CAs. Only the combination of certificate
        ///     issuer and certificate serial number is guaranteed to be unique.
        ///     urn:oid:2.5.29.29
        /// </summary>
        public static SamlAttributeMarshal<string> CertificateIssuer { get; } =
            new StringSamlAttributeMarshal("urn:oid:2.5.29.29", BasicNameFormat, XsString);

        /// <summary>
        ///     Production Unit Identifier
        ///     Danish companies may consist of several production units (produktionsenhed) corresponding to physical locations
        ///     registered in the Danish Company Registry (CVR). This attribute contains the unique identifier of
        ///     the production unit(10 digits) in which the user belongs.
        ///     dk:gov:saml:attribute:ProductionUnitIdentifier
        /// </summary>
        public static SamlAttributeMarshal<string> ProductionUnitIdentifier { get; } =
            new StringSamlAttributeMarshal("dk:gov:saml:attribute:ProductionUnitIdentifier", BasicNameFormat, XsString);

        /// <summary>
        ///     SE number
        ///     Danish companies consist of one or more tax units identified by an SE number (8 digits). SE numbers are issued
        ///     by the Danish Tax Agency, and the attribute below can be used to describe in which SE unit the user belongs.
        ///     dk:gov:saml:attribute:SENumberIdentifier
        /// </summary>
        public static SamlAttributeMarshal<string> SeNumberIdentifier { get; } =
            new StringSamlAttributeMarshal("dk:gov:saml:attribute:SENumberIdentifier", BasicNameFormat, XsString);

        /// <summary>
        ///     User Administrator indicator
        ///     Indicates that the user is an administrator of users within the user organization
        /// </summary>
        [Obsolete(@"The use of this attribute is NOT RECOMMENDED. This attribute is going to be deprecated in future versions of [OIO-SAML-SSO].")]
        public static SamlAttributeMarshal<bool> UserAdministratorIndicator { get; } =
            new BooleanSamlAttributeMarshal("dk:gov:saml:attribute:UserAdministratorIndicator", BasicNameFormat, XsString);

        /// <summary>
        ///     Privileges in intermediate model 2 form (see OIO Basic Privilege Provile - OIO-BPP).
        /// </summary>
        public static SamlAttributeMarshal<PrivilegeList> PrivilegesIntermediate { get; } =
            new XmlSerializableBase64SamlAttributeMarshal<PrivilegeList>("dk:gov:saml:attribute:Privileges_intermediate", BasicNameFormat, XsString);
    }
}