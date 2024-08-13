#region

using MultiFactorAuthentication.Models;
using MultiFactorAuthentication.Services;

#endregion

namespace MultiFactorAuthentication.Interfaces
{
    /// <summary>
    ///     An interface for using the TotpAuthenticatorService 
    /// </summary>
    public interface ITotpAuthenticatorService
    {

        /// <summary>
        ///     Retrieve the OtpUri
        /// </summary>
        /// <returns>
        ///     The OtpUri <para/>
        ///     <see c="null"/> if Totp is disabled
        /// </returns>
        OtpUri OtpUri(); 

        /// <summary>
        ///     Enable the Totp Authenticator
        /// </summary>
        bool Enable();

        /// <summary>
        ///     Disable the Totp Authenticator
        /// </summary>
        bool Disable();

        /// <summary>
        ///     Clears failed attempts and sets status to disabled
        /// </summary>
        void Reset();

        /// <summary>
        ///     Reset the failed attempt count for entering totp codes
        /// </summary>
        void ResetFailedTotpAttemptCount();

        /// <summary>
        ///     Reset the failed attempt count for entering hotp codes
        /// </summary>
        void ResetFailedHotpAttemptCount();

        /// <summary>
        ///     Validate the registration.
        /// </summary>
        /// <param name="code">The validation code.</param>
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
        ///     Validate the login
        /// </summary>
        /// <param name="code">The validation code.</param>
        /// <returns>
        ///     <see cref="ValidationStatus.Success"/> if the <see paramref="code" /> is valid. <para/>
        ///     <see cref="ValidationStatus.InvalidCode" /> if the <see paramref="code" /> is is invalid.
        ///     <see cref="ValidationStatus.Failed" /> if the <see paramref="code" /> there is no registration. <para />
        ///     <see cref="ValidationStatus.LockedOut"/> if the user has to many failed attempts. <para />
        ///     <see cref="ValidationStatus.CodeAlreadyUsed" /> if the <see paramref="code" /> is already used. <para />
        ///     <see cref="ValidationStatus.IncorrectTime" /> if the <see paramref="code" /> is not entered in the correct time span.
        /// </returns>
        ValidationStatus Validate(string code);
    }
}