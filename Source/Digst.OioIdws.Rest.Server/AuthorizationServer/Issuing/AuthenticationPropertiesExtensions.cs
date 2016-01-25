using System;
using Microsoft.Owin.Security;

namespace Digst.OioIdws.Rest.Server.AuthorizationServer.Issuing
{
    internal static class AuthenticationPropertiesExtensions
    {
        internal static void Value(this AuthenticationProperties obj, string value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            obj.Dictionary["value"] = value;
        }

        internal static string Value(this AuthenticationProperties obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            string value;
            obj.Dictionary.TryGetValue("value", out value);
            return value;
        }
    }
}
