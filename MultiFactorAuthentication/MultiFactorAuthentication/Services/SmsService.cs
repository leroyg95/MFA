using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiFactorAuthentication.Interfaces;

namespace MultiFactorAuthentication.Services
{
    public class SmsService
    {
        public static ISmsService Twilio { get; } = new Sms.Twilio();

    }
}
