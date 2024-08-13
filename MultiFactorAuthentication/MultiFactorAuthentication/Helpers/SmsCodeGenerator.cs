#region

using System.Configuration;

#endregion

namespace MultiFactorAuthentication.Helpers
{
    public class SmsCodeGenerator : CodeGenerator
    {
        private static int _smsLength;
        private static string _smsCharset;

        private static int SmsLength
        {
            get
            {
                if (_smsLength != 0) return _smsLength;

                var length = ConfigurationManager.AppSettings["codegenerator:smslength"];
                _smsLength = string.IsNullOrWhiteSpace(length) ? 6 : int.Parse(length);
                return _smsLength;
            }
        }

        private static string SmsCharset
        {
            get
            {
                if (_smsCharset == null)
                    _smsCharset = ConfigurationManager.AppSettings["codegenerator:smscharset"];

                if (string.IsNullOrWhiteSpace(_smsCharset))
                    _smsCharset = "0123456789";

                return _smsCharset;
            }
        }

        /// <summary>
        ///     Generates a code to send via sms.
        /// </summary>
        /// <remarks>
        ///     Settings can be changed from the AppSettings
        ///     codegenerator:smslength for the amount of digits (Default = 6),
        ///     codegenerator:smscharset for a custom charset (Default = numeric).
        /// </remarks>
        /// <returns>The code</returns>
        public static string GenerateCode()
        {
            return GenerateCode(SmsLength, SmsCharset);
        }
    }
}