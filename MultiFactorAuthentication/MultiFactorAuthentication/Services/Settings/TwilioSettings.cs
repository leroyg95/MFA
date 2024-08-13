#region

using System;
using System.Configuration;

#endregion

namespace MultiFactorAuthentication.Services.Settings
{
    public class TwilioSettings
    {
        private string _accountSid;
        private string _authToken;
        private string _phoneNumber;

        public string AccountSid
        {
            get
            {
                if (_accountSid == null)
                    _accountSid = ConfigurationManager.AppSettings["twilio:accountSid"];
                if (string.IsNullOrWhiteSpace(_accountSid))
                    throw new ArgumentNullException("Please set the accountSid in the AppSettings");
                return _accountSid;
            }
        }

        public string AuthToken
        {
            get
            {
                if (_authToken == null)
                    _authToken = ConfigurationManager.AppSettings["twilio:authToken"];
                if (string.IsNullOrWhiteSpace(_authToken))
                    throw new ArgumentNullException("Please set the authToken in the AppSettings");
                return _authToken;
            }
        }

        public string PhoneNumber
        {
            get
            {
                if (_phoneNumber == null)
                    _phoneNumber = ConfigurationManager.AppSettings["twilio:phoneNumber"];
                if (string.IsNullOrWhiteSpace(_phoneNumber))
                    throw new ArgumentNullException("Please set the phoneNumber in the AppSettings");
                return _phoneNumber;
            }
        }
    }
}