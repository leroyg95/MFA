#region

using System.Configuration;

#endregion

namespace MultiFactorAuthentication.Helpers
{
    public class TotpCodeGenerator : CodeGenerator
    {
        private static int _totpLength;
        private static string _totpCharset;

        private static int TOTPLength
        {
            get
            {
                if (_totpLength != 0) return _totpLength;

                var length = ConfigurationManager.AppSettings["codegenerator:totplength"];


                _totpLength = string.IsNullOrWhiteSpace(length) ? 32 : int.Parse(length);
                return _totpLength;
            }
        }

        private static string TOTPCharset
        {
            get
            {
                if (_totpCharset == null)
                    _totpCharset = ConfigurationManager.AppSettings["codegenerator:totpcharset"];

                if (string.IsNullOrWhiteSpace(_totpCharset))
                    _totpCharset = "abcdefghjkmnpqrstuvwxyzABCDEFGHJKMNPQRSTUVWXYZ23456789";

                return _totpCharset;
            }
        }

        /// <summary>
        ///     Generates a secret for the authenticator app.
        /// </summary>
        /// <remarks>
        ///     Settings can be changed from the AppSettings
        ///     codegenerator:totplength for the amount of characters (Default = 32),
        ///     codegenerator:totpcharset for a custom charset (Default = alphanumeric).
        /// </remarks>
        /// <returns>The code</returns>
        public static string GenerateCode()
        {
            return GenerateCode(TOTPLength, TOTPCharset);
        }
    }
}