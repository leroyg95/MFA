#region

using System.Configuration;
using System.Linq;
using ConfigurationException = MultiFactorAuthentication.Exceptions.ConfigurationException;

#endregion

namespace MultiFactorAuthentication.Services.Settings
{
    public class SpryngSettings
    {
        private string _username, _password, _from;

        public string Username
        {
            get
            {
                if (_username == null)
                    _username = ConfigurationManager.AppSettings["spryng:username"];
                if (string.IsNullOrWhiteSpace(_username))
                    throw new ConfigurationException("Please set the from in the AppSettings");

                return _username;
            }
        }

        public string Password
        {
            get
            {
                if (_password == null)
                    _password = ConfigurationManager.AppSettings["spryng:password"];
                if (string.IsNullOrWhiteSpace(_password))
                    throw new ConfigurationException("Please set the from in the AppSettings");

                return _password;
            }
        }

        public string From
        {
            get
            {
                if (_from == null)
                    _from = ConfigurationManager.AppSettings["spryng:from"];
                if (string.IsNullOrWhiteSpace(_from))
                    throw new ConfigurationException("Please set the from in the AppSettings");
                if (_from.All(char.IsDigit) && _from.Length > 14)
                    throw new ConfigurationException("From cannot be more than 14 digits");
                if (_from.All(char.IsLetterOrDigit) && _from.Length > 11)
                    throw new ConfigurationException("From cannot be more than 11 characters");

                return _from;
            }
        }
    }
}