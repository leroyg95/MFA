#region

using System;
using System.Configuration;

#endregion

namespace MultiFactorAuthentication.Services.Settings
{
    public class MessageBirdSettings
    {
        private string _accessKey, _from;

        public string AccessKey
        {
            get
            {
                if (_accessKey == null)
                    _accessKey = ConfigurationManager.AppSettings["messagebird:accesskey"];
                if (string.IsNullOrWhiteSpace(_accessKey))
                    throw new ArgumentNullException("Please set the accessKey in the AppSettings");
                return _accessKey;
            }
        }

        public string From
        {
            get
            {
                if (_from == null)
                    _from = ConfigurationManager.AppSettings["cm:from"];
                if (string.IsNullOrWhiteSpace(_from))
                    throw new ArgumentNullException("Please set the from in the AppSettings");
                return _from;
            }
        }
    }
}