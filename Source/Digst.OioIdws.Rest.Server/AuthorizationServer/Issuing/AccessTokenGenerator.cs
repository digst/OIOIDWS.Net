using System.Security.Cryptography;

namespace Digst.OioIdws.Rest.Server.AuthorizationServer.Issuing
{
    /// <summary>
    /// Inspired by http://stackoverflow.com/questions/19298801/generating-random-string-using-rngcryptoserviceprovider
    /// </summary>
    internal class AccessTokenGenerator : IAccessTokenGenerator
    {
        private static readonly char[] AvailableCharacters =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_'
        };

        /// <summary>
        /// Generates a token with at least 64 bits of entropy
        /// </summary>
        /// <returns></returns>
        public string GenerateAccesstoken()
        {
            //The random byte generates 256 possible values, that are split into 64 available characters, giving us 6 bits of entropy per character
            //20 charactes * 6 bits = 120 bits entropy
            var length = 20; 

            char[] identifier = new char[length];
            byte[] randomData = new byte[length];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomData);
            }

            for (int idx = 0; idx < identifier.Length; idx++)
            {
                int pos = randomData[idx] % AvailableCharacters.Length;
                identifier[idx] = AvailableCharacters[pos];
            }

            return new string(identifier);
        }
    }
}