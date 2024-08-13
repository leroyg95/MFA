#region

using System;

#endregion

namespace MultiFactorAuthentication.Models
{
    /// <summary>
    ///     Specificies which authenticators are activated
    /// </summary>
    [Flags]
    public enum MultiFactorAuthenticators
    {
        /// <summary>
        ///     No authenticators active
        /// </summary>
        None = 0,
        /// <summary>
        ///     Sms authenticator active
        /// </summary>
        Sms = 1,
        /// <summary>
        ///     Totp authenticator active
        /// </summary>
        Totp = 2,
        /// <summary>
        ///     Hotp authenticator active
        /// </summary>
        Hotp = 4
    }

    /// <summary>
    ///     Specifies the password type for one-time passwords
    /// </summary>
    public enum PasswordType
    {
        /// <summary>
        ///     Time-based One-time Password
        /// </summary>
        Totp,
        /// <summary>
        ///     HMAC-based One-time Password
        /// </summary>
        Hotp
    }

    /// <summary>
    ///     Specifies a number of statusses for validation
    /// </summary>
    public enum ValidationStatus
    {
        /// <summary>
        ///     The validation failed
        /// </summary>
        Failed,
        /// <summary>
        ///     The validation succeeded
        /// </summary>
        Success,
        /// <summary>
        ///     The validation was done in an incorrect time window
        /// </summary>
        IncorrectTime,
        /// <summary>
        ///     The code was incorrect
        /// </summary>
        InvalidCode,
        /// <summary>
        ///     The code has already been used
        /// </summary>
        CodeAlreadyUsed,
        /// <summary>
        ///     The user is locked out
        /// </summary>
        LockedOut
    }

    /// <summary>
    ///     The different sms services that are implemented
    /// </summary>
    public enum SmsService
    {
        /// <summary>
        ///     Twilio is a cloud communications platform as a service company based in San Francisco, California.
        /// </summary>
        Twilio,
        /// <summary>
        ///     CM is a mobile services company, based in the Netherlands
        /// </summary>
        CM,
        /// <summary>
        ///     Spryng is a Sms services company, based in the Netherlands
        /// </summary>
        Spryng,
        /// <summary>
        ///     MessageBird is a mobile services company, based in the Netherlands
        /// </summary>
        MessageBird
    }

    /// <summary>
    ///     The different statusses for the services
    /// </summary>
    public enum Status
    {
        /// <summary>
        ///     The service is disabled
        /// </summary>
        Disabled,
        /// <summary>
        ///     The service is pending
        /// </summary>
        Pending,
        /// <summary>
        ///     The service is validated
        /// </summary>
        Validated
    }
}