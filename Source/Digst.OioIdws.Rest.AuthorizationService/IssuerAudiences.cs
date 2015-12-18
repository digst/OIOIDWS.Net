using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Digst.OioIdws.Rest.AuthorizationService
{
    public class IssuerAudiences : IEnumerable<Uri>
    {
        readonly Collection<Uri> _audiences = new Collection<Uri>();

        public IssuerAudiences(string issuerThumbprint, string issuerFriendlyName)
        {
            if (issuerThumbprint == null)
            {
                throw new ArgumentNullException(nameof(issuerThumbprint));
            }
            if (issuerFriendlyName == null)
            {
                throw new ArgumentNullException(nameof(issuerFriendlyName));
            }
            IssuerThumbprint = issuerThumbprint.Replace(" ", "").ToLowerInvariant();
            IssuerFriendlyName = issuerFriendlyName;
        }

        public string IssuerThumbprint { get; private set; }
        public string IssuerFriendlyName { get; private set; }

        public IssuerAudiences Audience(Uri audience)
        {
            if (audience == null)
            {
                throw new ArgumentNullException(nameof(audience));
            }

            _audiences.Add(audience);
            return this;
        }

        public IEnumerator<Uri> GetEnumerator()
        {
            return _audiences.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}