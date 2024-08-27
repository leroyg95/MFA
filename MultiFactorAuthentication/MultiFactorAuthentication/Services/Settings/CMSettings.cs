#region

using System;
using System.Configuration;
using ConfigurationException = MultiFactorAuthentication.Exceptions.ConfigurationException;

#endregion

namespace MultiFactorAuthentication.Services.Settings
{
    public class CMSettings
    {
        private string _from;
        private Guid? _token;
        private Uri _url;

        public Guid Token
        {
            get
            {
                if (_token != null) return _token.Value;
                try
                {
                    _token = Guid.Parse(ConfigurationManager.AppSettings["cm:token"]);
                }
                catch (Exception)
                {
                    throw new ConfigurationException("Please set the CM token in the AppSettings and make sure it's a valid GUID");
                }

                return _token.Value;
            }
        }

        public string From
        {
            get
            {
                if (_from == null)
                    _from = ConfigurationManager.AppSettings["cm:from"];
                if (string.IsNullOrWhiteSpace(_from))
                    throw new ConfigurationException("Please set the CM from in the AppSettings");
                return _from;
            }
        }

        public Uri Url
        {
            get
            {
                if (_url != null) return _url;
                try
                {
                    _url = new Uri(ConfigurationManager.AppSettings["cm:url"]);
                }
                catch (Exception)
                {
                    throw new ConfigurationException("Please set the CM url in the AppSettings and make sure it's a valid URI");
                }

                return _url;
            }
        }
    }
}