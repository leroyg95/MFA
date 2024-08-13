#region

using System;
using CM.Sms;
using MultiFactorAuthentication.Exceptions;
using MultiFactorAuthentication.Interfaces;

#endregion

namespace MultiFactorAuthentication.Services.Sms.Providers
{
    internal class CM : ISmsService
    {
        /// <summary>
        ///     Sends a sms using CMTelecom
        /// </summary>
        /// <param name="to">Phonenumber of the recipient</param>
        /// <param name="message">Body of the sms</param>
        /// <exception cref="ConfigurationException">
        ///     When the Token or From are null or whitespace
        /// </exception>
        /// <exception cref="ArgumentNullException">When the <see cref="to" /> or <see cref="message" /> are null or whitespace</exception>
        /// <exception cref="SmsException">When something fails while sending the sms</exception>
        public void Send(string to, string message)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentNullException(nameof(to));
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));

            to = to.Replace("+", "00");

            var token = SettingsService.Instance.CM.Token;
            var from = SettingsService.Instance.CM.From;

            try
            {
                var smsGateway = new SmsGatewayClient(token);
                smsGateway.SendSms(from, to, message);
            }
            catch (Exception e)
            {
                throw new SmsException(e.Message, e);
            }
        }
    }
}