#region

using System;
using MultiFactorAuthentication.Interfaces;
using MultiFactorAuthentication.Models;

#endregion

namespace MultiFactorAuthentication.Services.Sms
{
    /// <summary>
    ///     A SmsProvider factory
    /// </summary>
    public static class SmsProviderFactory
    {
        /// <summary>
        ///     Construct a sms service
        /// </summary>
        /// <param name="smsService">the sms service</param>
        /// <returns>A new sms service</returns>
        public static ISmsService Create(SmsService smsService)
        {
            switch (smsService)
            {
                case SmsService.Twilio:
                    return new Providers.Twilio();
                case SmsService.MessageBird:
                    return new Providers.MessageBird();
                case SmsService.CM:
                    return new Providers.CM();
                case SmsService.Spryng:
                    return new Providers.Spryng();
                default:
                    throw new InvalidOperationException("smsService is invalid");
            }
        }
    }
}