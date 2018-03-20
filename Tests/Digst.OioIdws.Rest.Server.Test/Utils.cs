using System;
using System.IO;

namespace Digst.OioIdws.Rest.Server.Test
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

        public static string FromBase64(string str)
        {
            var bytes = Convert.FromBase64String(str);
            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
