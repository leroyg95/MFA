#region

using System.Configuration;
using ConfigurationException = MultiFactorAuthentication.Exceptions.ConfigurationException;

#endregion

namespace MultiFactorAuthentication.Services.Settings
{
    public class CMSettings
    {
        private string _from;
        private string _token;

        public string Token
        {
            get
            {
                if (_token == null)
                    _token = ConfigurationManager.AppSettings["cm:token"];
                if (string.IsNullOrWhiteSpace(_token))
                    throw new ConfigurationException("Please set the token in the AppSettings");
                return _token;
            }
        }

        public string From
        {
            get
            {
                if (_from == null)
                    _from = ConfigurationManager.AppSettings["cm:from"];
                if (string.IsNullOrWhiteSpace(_from))
                    throw new ConfigurationException("Please set the from in the AppSettings");
                return _from;
            }
        }
    }
}