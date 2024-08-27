#region

using System;
using System.Threading.Tasks;
using MessageBird;
using MultiFactorAuthentication.Interfaces;

#endregion

namespace MultiFactorAuthentication.Services.Sms.Providers
{
    public class MessageBird : ISmsService
    {
        public void Send(string to, string message)
        {
            var accessKey = SettingsService.Instance.MessageBird.AccessKey;
            var from = SettingsService.Instance.MessageBird.From;

            to = to.TrimStart('+');
            long.TryParse(to, out var phone);
            var client = Client.CreateDefault(accessKey);
            client.SendMessage(from, message, new[] {phone});
        }

        public Task SendAsync(string to, string message)
        {
            throw new NotImplementedException();
        }
    }
}