using System;

namespace Digst.OioIdws.Healthcare.Common
{
    /// <summary>
    /// An attribute stating the evidence that the subject of the assertion (or the user the subject is acting on behalf of) has the patient in care (‘Behandlingsrelation’)
    /// </summary>
    public class EvidenceForPatientInCare : IEquatable<EvidenceForPatientInCare>
    {

        /// <summary>
        /// Gets the saml attribute value.
        /// </summary>
        /// <value>
        /// The saml attribute value.
        /// </value>
        public string SamlAttributeValue { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EvidenceForPatientInCare"/> class.
        /// </summary>
        /// <param name="samlAttributeValue">The saml attribute value.</param>
        /// <exception cref="ArgumentNullException">samlAttributeValue</exception>
        public EvidenceForPatientInCare(string samlAttributeValue)
        {
            SamlAttributeValue = samlAttributeValue ?? throw new ArgumentNullException(nameof(samlAttributeValue));
        }

        /// <summary>
        /// Used as a declaration for having the patient in care by an initiating actor, that the consumer of the assertion MAY trust
        /// </summary>
        public static EvidenceForPatientInCare PatientInCare = new EvidenceForPatientInCare("urn:dk:healthcare:local:patient_in_care ");

        /// <summary>
        /// Used to express the A++ degree of evidence verified against the Danish Evidence for Patient in Care Service (‘Behandlingsrelationsservice’)
        /// </summary>
        public static EvidenceForPatientInCare DegreeAPlusPlus = new EvidenceForPatientInCare("urn:dk:healthcare:brs:A++");

        /// <summary>
        /// Used to express the A degree of evidence verified against the Danish Evidence for Patient in Care Service (‘Behandlingsrelationsservice’)
        /// </summary>
        public static EvidenceForPatientInCare DegreeA = new EvidenceForPatientInCare("urn:dk:healthcare:brs:A");

        /// <summary>
        /// Used to express the B degree of evidence verified against the Danish Evidence for Patient in Care Service (‘Behandlingsrelationsservice’)
        /// </summary>
        public static EvidenceForPatientInCare DegreeB = new EvidenceForPatientInCare("urn:dk:healthcare:brs:B");

        /// <summary>
        /// Used to express the C degree of evidence verified against the Danish Evidence for Patient in Care Service (‘Behandlingsrelationsservice’)
        /// </summary>
        public static EvidenceForPatientInCare DegreeC = new EvidenceForPatientInCare("urn:dk:healthcare:brs:C");

        /// <summary>
        /// Used to express the D degree of evidence verified against the Danish Evidence for Patient in Care Service (‘Behandlingsrelationsservice’)
        /// </summary>
        public static EvidenceForPatientInCare DegreeD = new EvidenceForPatientInCare("urn:dk:healthcare:brs:D");

        /// <summary>
        /// Used to express the E degree of evidence verified against the Danish Evidence for Patient in Care Service (‘Behandlingsrelationsservice’)
        /// </summary>
        public static EvidenceForPatientInCare DegreeE = new EvidenceForPatientInCare("urn:dk:healthcare:brs:E");


        /// <inheritdoc />
        public bool Equals(EvidenceForPatientInCare other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(SamlAttributeValue, other.SamlAttributeValue);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EvidenceForPatientInCare)obj);
        }


        /// <inheritdoc />
        public override int GetHashCode()
        {
            return SamlAttributeValue.GetHashCode();
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        public static bool operator ==(EvidenceForPatientInCare left, EvidenceForPatientInCare right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        public static bool operator !=(EvidenceForPatientInCare left, EvidenceForPatientInCare right)
        {
            return !Equals(left, right);
        }
    }
}