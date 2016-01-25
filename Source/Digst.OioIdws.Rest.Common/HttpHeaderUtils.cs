using System.Collections.Generic;
using System.Linq;

namespace Digst.OioIdws.Rest.Common
{
    public static class HttpHeaderUtils
    {
        public static IDictionary<string, string> ParseOAuthSchemeParameter(string parameters)
        {
            return parameters
                .Split(',')
                .Select(p => p.Trim('\n', '\r', ' ').Split('='))
                .Where(x => x.Length == 2)
                .ToDictionary(x => x[0], x => x[1].Trim('"'));
        }
    }
}
