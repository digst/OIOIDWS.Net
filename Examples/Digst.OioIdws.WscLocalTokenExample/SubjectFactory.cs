using System;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace Digst.OioIdws.WscLocalTokenExample
{
    /// <summary>
    /// Produces subjects and subject confirmation specifications for the NemLog-in federation.
    /// </summary>
    public class SubjectFactory
    {
        public static Saml2Subject PersistentIdentifierSubject(string identifier, Saml2SubjectConfirmation subjectConfirmation)
        {
            var nameId = new Saml2NameIdentifier(identifier, new Uri("urn:oasis:names:tc:SAML:2.0:nameid-format:persistent"));
            return new Saml2Subject(nameId) { SubjectConfirmations = { subjectConfirmation } };
        }

        /// <summary>
        /// Generates an X509Subject in accordance with the danish certificate policy standard.
        /// This is a crude method which does not validate the input parameters. Using characters
        /// such as , (comma) or = (equals) will almost certainly generate invalid subject names.
        /// It is generally NOT advisable to synthesize X509 subject names like this in a production
        /// environment. Instead the X509 subject name should be retrieved through some other means
        /// e.g. by reading it from an employee (MOCES) certificate or retrieving it from a service.
        /// </summary>
        /// <param name="employeeRid">The employee RID (role-identifier). Uniquely identifies the employee within the organization.</param>
        /// <param name="employeeName">Name of the employee.</param>
        /// <param name="organizationCvr">The organization CVR number.</param>
        /// <param name="organizationName">Name of the organization.</param>
        /// <param name="subjectConfirmation">The desired subject confirmation method.</param>
        /// <param name="country">The country.</param>
        public static Saml2Subject X509Subject(string employeeRid, string employeeName, string organizationCvr, string organizationName, Saml2SubjectConfirmation subjectConfirmation, string country = "DK")
        {
            var identifier = $"C={country},O={organizationName} // CVR:{organizationCvr},CN={employeeName},Serial=CVR:{organizationCvr}-RID:{employeeRid}";
            var nameId = new Saml2NameIdentifier(identifier, new Uri("urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName"));
            return new Saml2Subject(nameId) { SubjectConfirmations = { subjectConfirmation } };
        }

        /// <summary>
        /// Get a holder-of-key confirmation method based on a certificate.
        /// </summary>
        /// <param name="heldKey">The held key. The presenter/caller of a service must prove
        /// access to the private key of this certificate, e.g. by signing the request using the key.</param>
        public static Saml2SubjectConfirmation HolderOfKeySubjectConfirmation(X509Certificate2 heldKey)
        {
            return new Saml2SubjectConfirmation(new Uri("urn:oasis:names:tc:SAML:2.0:cm:holder-of-key"))
            {
                SubjectConfirmationData = new Saml2SubjectConfirmationData()
                {
                    KeyIdentifiers = { new SecurityKeyIdentifier(new X509RawDataKeyIdentifierClause(heldKey)) }
                }
            };
        }

        /// <summary>
        /// Get a bearer confirmation method. Any bearer of a token with a subject this confirmation
        /// should be trusted.
        /// </summary>
        /// <returns></returns>
        public static Saml2SubjectConfirmation BearerSubjectConfirmation()
        {
            return new Saml2SubjectConfirmation(new Uri("urn:oasis:names:tc:SAML:2.0:cm:bearer"));
        }

    }
}