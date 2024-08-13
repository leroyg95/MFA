using System;
using MultiFactorAuthentication.Models;

namespace MultiFactorAuthentication.Interfaces
{
    /// <summary>
    ///     An interface for using the SmsAuthenticatorService 
    /// </summary>
    public interface ISmsAuthenticatorService
    {
        /// <summary>
        ///     Get the phone that is associated with the user.
        /// </summary>
        string Phone { get; }

        /// <summary>
        ///     The sms message to be send to the user.
        ///     Use "{0}" to indicate the sms code.
        /// </summary>
        string Message { get; set; }

        /// <summary>
        ///     Enables 2FA via SMS for the specified user.
        /// </summary>
        /// <param name="phone">The phonenumber of the user</param>
        /// <exception cref="ArgumentNullException">
        ///     When the <see paramref="phone" /> is null or whitespace
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     When the <see paramref="phone" /> is invalid
        ///     When the SmsStatus is not Status.Disabled
        /// </exception>
        void Register(string phone);

        /// <summary>
        ///     Change the users phone number and set status to pending
        /// </summary>
        /// <param name="phone">The phonenumber of the user</param>
        /// <exception cref="ArgumentNullException">
        ///     When the <see paramref="Phone" /> is null or whitespace
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     When the <see paramref="phone" /> is invalid
        ///     When the SmsStatus is not Status.Disabled
        /// </exception>
        void ChangePhone(string phone);

        /// <summary>
        ///     Disables sms for the current user
        /// </summary>
        /// <returns>
        ///     True if the status has changed. <para/>
        ///     False if it is already disabled.
        /// </returns>
        bool Disable();

        /// <summary>
        ///     Clears failed attempts, and count of sens Sms'. Sets status to disabled.
        /// </summary>
        void Reset();

        /// <summary>
        ///     Reset the failed attempt count for entering sms codes
        /// </summary>
        void ResetFailedSmsAttemptCount();

        /// <summary>
        ///     Reset the failed attempt count for entering phone numbers
        /// </summary>
        void ResetFailedPhoneNumberAttemptCount();

        /// <summary>
        ///     Reset the failed attempt count for sending sms codes
        /// </summary>
        void ResetSendSmsCodeCount();

        /// <summary>
        ///     Send the security code to the user.
        /// </summary>
        /// <returns>
        ///     <see cref="ValidationStatus.Success"/> if the code has been send. <para/>
        ///     <see cref="ValidationStatus.Failed" /> if the sms service is disabled. <para />
        ///     <see cref="ValidationStatus.LockedOut"/> if the user has to many failed attempts. 
        /// </returns>
        ValidationStatus SendSecurityCode();

        /// <summary>
        ///     Verify the phonenumber of the user
        /// </summary>
        /// <param name="phone">The phonenumber</param>
        /// <returns>
        ///     <see cref="ValidationStatus.Success"/> if the <see paramref="phone" /> is correct. <para/>
        ///     <see cref="ValidationStatus.Failed" /> if the <see paramref="phone" /> is incorrect. <para />
        ///     <see cref="ValidationStatus.LockedOut"/> if the user has to many failed attempts. 
        /// </returns>
        ValidationStatus VerifyPhoneNumber(string phone);

        /// <summary>
        ///     Validate the code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>
        ///     <see cref="ValidationStatus.Success"/> if the <see paramref="code" /> is valid. <para/>
        ///     <see cref="ValidationStatus.InvalidCode" /> if the <see paramref="code" /> is is invalid.
        ///     <see cref="ValidationStatus.Failed" /> if the <see paramref="code" /> there is no registration. <para />
        ///     <see cref="ValidationStatus.LockedOut"/> if the user has to many failed attempts. <para />
        ///     <see cref="ValidationStatus.CodeAlreadyUsed" /> if the <see paramref="code" /> is already used. <para />
        ///     <see cref="ValidationStatus.IncorrectTime" /> if the <see paramref="code" /> is not entered in the correct time span.
        /// </returns>
        ValidationStatus Validate(string code);

        /// <summary>
        ///     Validates the security code after the user has been registered
        /// </summary>
        /// <param name="code">The code</param>
        /// <returns>
        ///     <see cref="ValidationStatus.Success"/> if the <see paramref="code" /> is valid. <para/>
        ///     <see cref="ValidationStatus.InvalidCode" /> if the <see paramref="code" /> is is invalid.
        ///     <see cref="ValidationStatus.Failed" /> if the <see paramref="code" /> there is no registration. <para />
        ///     <see cref="ValidationStatus.LockedOut"/> if the user has to many failed attempts. <para />
        ///     <see cref="ValidationStatus.CodeAlreadyUsed" /> if the <see paramref="code" /> is already used. <para />
        ///     <see cref="ValidationStatus.IncorrectTime" /> if the <see paramref="code" /> is not entered in the correct time span.
        /// </returns>
        ValidationStatus ValidateRegistration(string code);

        /// <summary>
        ///     Checks if the phonenumber has a valid format (E.164)
        /// </summary>
        /// <param name="phone">The phonenumber to validate</param>
        /// <returns>True if the phone is valid</returns>
        bool IsValidPhone(string phone);
    }
}