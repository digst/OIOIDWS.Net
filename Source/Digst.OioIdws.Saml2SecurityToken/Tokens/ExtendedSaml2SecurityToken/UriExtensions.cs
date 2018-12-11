using System;

namespace Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken
{
    internal static class UriExtensions {

        public static Uri ToUri(this string value)
        {
            return new Uri(value);
        }

    }
}