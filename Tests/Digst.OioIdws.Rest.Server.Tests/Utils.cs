using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digst.OioIdws.Rest.AuthorizationService.Tests
{
    public static class Utils
    {
        public static string ToBase64(string str)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(str);
                }
                return Convert.ToBase64String(stream.ToArray());
            }
        }
    }
}
