#region

using System;
using System.Threading.Tasks;
using MultiFactorAuthentication.Interfaces;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

#endregion

namespace MultiFactorAuthentication.Services.Sms.Providers
{
    public class Twilio : ISmsService
    {
        /// <summary>
        /// </summary>
        /// <param name="to">Recipient phonenumber (E.164 format)</param>
        /// <param name="message">Body of the sms</param>
        public void Send(string to, string message)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentNullException(nameof(to));
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));

            var accountSid = SettingsService.Instance.Twilio.AccountSid;
            var authToken = SettingsService.Instance.Twilio.AuthToken;
            var phoneNumber = SettingsService.Instance.Twilio.PhoneNumber;

            TwilioClient.Init(accountSid, authToken);
            MessageResource.Create(
                from: new PhoneNumber(phoneNumber),
                to: new PhoneNumber(to),
                body: message
            );
        }

        public async Task SendAsync(string to, string message)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentNullException(nameof(to));
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));

            var accountSid = SettingsService.Instance.Twilio.AccountSid;
            var authToken = SettingsService.Instance.Twilio.AuthToken;
            var phoneNumber = SettingsService.Instance.Twilio.PhoneNumber;

            TwilioClient.Init(accountSid, authToken);
            await MessageResource.CreateAsync(
                from: new PhoneNumber(phoneNumber),
                to: new PhoneNumber(to),
                body: message
            );
        }
    }
}