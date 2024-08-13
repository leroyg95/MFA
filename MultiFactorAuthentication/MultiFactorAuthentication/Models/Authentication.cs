#region

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#endregion

namespace MultiFactorAuthentication.Models
{
    /// <summary>
    ///     Authentication
    /// </summary>
    public class Authentication
    {
        /// <summary>
        ///     External UserId
        /// </summary>
        [Key]
        public Guid UserId { get; set; }

        /// <summary>
        ///     External Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     The registered Phonenumber
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        ///     The Sms secret that is sent to the user.
        /// </summary>
        public string SmsSecret { get; set; }

        /// <summary>
        ///     The datetime when the sms is sent to the user.
        /// </summary>
        public DateTime? SmsIssued { get; set; }

        /// <summary>
        ///     Secret for the TOTP
        /// </summary>
        public string TotpSecret { get; set; }

        /// <summary>
        ///     The latest timewindow that was used.
        /// </summary>
        public long TimeWindowUsed { get; set; }

        /// <summary>
        ///     The smsStatus
        /// </summary>
        public Status SmsStatus { get; set; }

        /// <summary>
        ///     The TotpStatus
        /// </summary>
        public Status TotpStatus { get; set; }

        /// <summary>
        ///     The number of sms codes that have been send without validating
        /// </summary>
        public int SendSmsCodeCount { get; set; }

        /// <summary>
        ///     The number of failed attempts on entering the phone number
        /// </summary>
        public int FailedPhoneNumberAttemptCount { get; set; }

        /// <summary>
        ///     The number of failed attemps on entering the sms code
        /// </summary>
        public int FailedSmsAttemptCount { get; set; }

        /// <summary>
        ///     The number of failed attemps on entering the totp code
        /// </summary>
        public int FailedTotpAttemptCount { get; set; }

        /// <summary>
        ///     The number of failed attemps on entering the hotp code
        /// </summary>
        public int FailedHotpAttemptCount { get; set; }

        /// <summary>
        ///     List of devices where the user has logged on to and wants to remember
        /// </summary>
        public virtual List<AuthDevices> Devices { get; set; }

        /// <summary>
        ///     Indicates if user has to use TFA
        /// </summary>
        public bool Disabled { get; set; }
    }
}