﻿using System;

namespace Digst.OioIdws.Rest.Common
{
    public static class AccessTokenTypeParser
    {
        public static AccessTokenType? FromString(string str)
        {
            if ("bearer".Equals(str, StringComparison.OrdinalIgnoreCase))
            {
                return AccessTokenType.Bearer;
            }

            if ("holder-of-key".Equals(str, StringComparison.OrdinalIgnoreCase))
            {
                return AccessTokenType.HolderOfKey;
            }

            return null;
        }

        public static string ToString(AccessTokenType type)
        {
            if (type == AccessTokenType.Bearer)
            {
                return "Bearer";
            }

            if (type == AccessTokenType.HolderOfKey)
            {
                return "Holder-of-key";
            }

            return null;
        }
    }
}
