using System;

namespace Digst.OioIdws.Healthcare.Common
{
    /// <summary>
    /// An attribute used to express a patient’s given consent in relation to the subject (or the user the subject is acting on behalf of) and the user’s organization. 
    /// Given consent origins either from then service consumer itself (as a claim), or from a central register.
    /// This attribute will not define whether a service consumer may access the data it requests at the service provider, but it helps identify if access should be prohibited. Typically, this attribute will be used along with a purpose of use
    /// </summary>
    public class GivenConsent : IEquatable<GivenConsent>
    {
        public string SamlAttributeValue { get; }

        public GivenConsent(string samlAttributeValue)
        {
            SamlAttributeValue = samlAttributeValue ?? throw new ArgumentNullException(nameof(samlAttributeValue));
        }

        /// <summary>
        /// Neither positive nor negative consent regarding the user or the user’s organization has been registered
        /// </summary>
        public static GivenConsent None = new GivenConsent("urn:dk:healthcare:consent:none");

        /// <summary>
        /// Negative consent against the user or the user’s organization has been registered.
        /// </summary>
        public static GivenConsent Negative= new GivenConsent("urn:dk:healthcare:consent:negative");

        /// <summary>
        /// Positive consent regarding the user or the user’s organization has been registered.
        /// </summary>
        public static GivenConsent Positive = new GivenConsent("urn:dk:healthcare:consent:positive");

        /// <summary>
        /// No negative consent against the user or the user’s organization has been registered, but there exist relevant data specific (negative) consent declarations.
        /// </summary>
        public static GivenConsent DataSpecific = new GivenConsent("urn:dk:healthcare:consent:data_specific");

        /// <inheritdoc />
        public bool Equals(GivenConsent other)
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
            return Equals((GivenConsent) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return SamlAttributeValue.GetHashCode();
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        public static bool operator ==(GivenConsent left, GivenConsent right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        public static bool operator !=(GivenConsent left, GivenConsent right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return SamlAttributeValue;
        }
    }
}