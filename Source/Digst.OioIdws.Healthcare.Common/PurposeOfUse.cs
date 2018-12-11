using System.Xml.Serialization;

namespace Digst.OioIdws.Healthcare.Common
{
    /// <summary>
    /// The usage context for the assertion in relation to the patient  
    /// </summary>
    [XmlRoot("PurposeOfUse", Namespace = ns)]
    public class PurposeOfUse : Hl7CodedElement
    {
        // ReSharper disable once InconsistentNaming

        /// <summary>
        /// Constructor to support XML serialization
        /// </summary>
        protected PurposeOfUse() : base()
        {

        }


        public PurposeOfUse(string code, string codeSystem = "urn:oasis:names:tc:xspa:1.0", string codeSystemName = null, string displayName = null) : base(code, codeSystem, codeSystemName, displayName)
        {
        }


        /// <summary>
        /// Standard value TREATMENT
        /// The subject of the assertion has the patient in care and uses the assertion to access data relevant to treating the patient
        /// </summary>
        public static readonly PurposeOfUse Treatment = new PurposeOfUse("TREATMENT", "urn:oasis:names:tc:xspa:1.0");

        /// <summary>
        /// Standard value EMERGENCY
        /// 
        /// The subject of the assertion has the patient in care and in an emergency uses the assertion to access data relevant to treating
        /// the patient overriding any existing negative consent. This value MUST only be used to express that the subject of the assertion
        /// is employing the rules governing ‘værdispring’ as laid out in the Danish Healthcare Law .
        /// </summary>
        public static readonly PurposeOfUse Emergency = new PurposeOfUse("EMERGENCY", "urn:oasis:names:tc:xspa:1.0");

        /// <summary>
        /// Standard value REQUEST
        ///
        /// Employed when the subject of the assertion is a citizen or a non-healthcare professional holding of procuration (‘fuldmagt’) from another citizen.
        /// REQUEST can also be used when a “second opinion” is needed.
        /// </summary>
        public static readonly PurposeOfUse Request = new PurposeOfUse("REQUEST", "urn:oasis:names:tc:xspa:1.0");
    }
}