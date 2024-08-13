#region

using System.Security.Cryptography;
using System.Text;

#endregion

namespace MultiFactorAuthentication.Helpers
{
    public class CodeGenerator
    {
        /// <summary>
        ///     Generates a code with a specific length out of a specific charset
        /// </summary>
        /// <remarks>
        ///     See <see cref="SmsCodeGenerator" /> for creating sms codes.
        ///     See <see cref="TotpCodeGenerator" /> for creating TOTP secrets.
        /// </remarks>
        /// <param name="length">The length of the code</param>
        /// <param name="charset">The characters to pick from</param>
        /// <returns>The code</returns>
        public static string GenerateCode(int length, string charset)
        {
            var result = new StringBuilder();
            var rng = new RNGCryptoServiceProvider();
            var randombytes = new byte[length];
            rng.GetBytes(randombytes);

            foreach (var t in randombytes)
                result.Append(charset[t % charset.Length]);

            return result.ToString();
        }
    }
}