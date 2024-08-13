#region

using System;
using MultiFactorAuthentication.Exceptions;
using MultiFactorAuthentication.Interfaces;
using Spryng;
using Spryng.Models.Sms;

#endregion

namespace MultiFactorAuthentication.Services.Sms.Providers
{
    internal class Spryng : ISmsService
    {
        /// <summary>
        ///     Sends a sms using Spryng
        /// </summary>
        /// <param name="to">Phonenumber of the recipient</param>
        /// <param name="message">Body of the sms</param>
        /// <exception cref="ConfigurationException">
        ///     When the Username, Password or From are null or
        ///     whitespace
        /// </exception>
        /// <exception cref="ArgumentNullException">When the <see cref="to" /> or <see cref="message" /> are null or whitespace</exception>
        /// <exception cref="SmsException">When something fails while sending the sms</exception>
        public void Send(string to, string message)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentNullException(nameof(to));
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));

            var username = SettingsService.Instance.Spryng.Username;
            var password = SettingsService.Instance.Spryng.Password;
            var from = SettingsService.Instance.Spryng.From;

            var client = new SpryngHttpClient(username, password);

            to = to.TrimStart('+');

            var request = new SmsRequest
            {
                Destinations = new[] {to},
                Sender = from,
                Body = message
            };

            try
            {
                client.ExecuteSmsRequest(request);
            }
            catch (Exception e)
            {
                throw new SmsException(e.Message, e);
            }
        }
    }
}