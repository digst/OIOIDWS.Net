using System;

namespace Digst.OioIdws.SamlAttributes
{
    public class SpecVer : IEquatable<SpecVer>
    {
        public string VersionIdentifier { get; }

        public static readonly SpecVer DkSaml20 = new SpecVer("DK-SAML-2.0");

        public SpecVer(string versionIdentifier)
        {
            VersionIdentifier = versionIdentifier ?? throw new ArgumentNullException(nameof(versionIdentifier));
        }

        public bool Equals(SpecVer other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(VersionIdentifier, other.VersionIdentifier);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SpecVer) obj);
        }

        public override int GetHashCode()
        {
            return VersionIdentifier.GetHashCode();
        }

        public override string ToString()
        {
            return VersionIdentifier;
        }
    }
}