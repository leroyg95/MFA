using System.Threading.Tasks;
using MultiFactorAuthentication.Interfaces;

namespace MultiFactorAuthenticationTests.Mockups
{
    public class SmsServiceMockup : ISmsService
    {
        public void Send(string to, string message)
        {
            LastTo = to;
            LastMessage = message;
        }

        public Task SendAsync(string to, string message)
        {
            LastTo = to;
            LastMessage = message;
            return Task.CompletedTask;
        }

        public string LastTo { get; set; }

        public string LastMessage { get; set; }
    }
}
