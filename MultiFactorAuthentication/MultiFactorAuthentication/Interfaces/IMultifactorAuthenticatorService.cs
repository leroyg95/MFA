#region

using System;
using MultiFactorAuthentication.Models;

#endregion

namespace MultiFactorAuthentication.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMultifactorAuthenticatorService
    {
        /// <summary>
        ///     Register a user
        /// </summary>
        /// <param name="userId">The user.</param>
        /// <param name="username">The username.</param>
        void Register(Guid userId, string username);

        /// <summary>
        ///     Checks if the user is registered.
        /// </summary>
        /// <param name="userId">The user.</param>
        /// <returns>true if the user exists, false if the user does not exists.</returns>
        bool UserExists(Guid userId);

        /// <summary>
        ///     Delete the user from the MultifactorAuthenticationService.
        /// </summary>
        /// <param name="userId">The user.</param>
        void Delete(Guid userId);

        /// <summary>
        ///     Find the registrations that are enabled for the user.
        /// </summary>
        /// <returns>
        ///     The multifactorAuthenticators available for the user.
        /// </returns>
        MultiFactorAuthenticators Registrations(Guid userId);

        /// <summary>
        ///     Find the validated registrations of the user.
        /// </summary>
        /// <returns>
        ///     The validated multifactorAuthenticators of the user.
        /// </returns>
        MultiFactorAuthenticators ValidatedRegistrations(Guid userId);

        /// <summary>
        ///     Retrieves the SmsAuthenticator for the user.
        /// </summary>
        /// <param name="userId">The user.</param>
        /// <param name="smsService">The smsservice (optional)</param>
        /// <returns>The SmsAuthenticator for the user.</returns>
        ISmsAuthenticatorService GetSmsAuthenticatorService(Guid userId, ISmsService smsService);

        /// <summary>
        ///     Retrieves the TotpAuthenticator for the user.
        /// </summary>
        /// <param name="userId">The user.</param>
        /// <returns>The TotpAuthenticator for the user.</returns>
        ITotpAuthenticatorService GetTotpAuthenticatorService(Guid userId);

        /// <summary>
        ///     To not have to login for a given period
        /// </summary>
        /// <param name="userId">The user</param>
        /// <param name="date">The date until the user is authenticated</param>
        /// <param name="identifier">An unique identifier for the device the user is on</param>
        void RememberLogin(Guid userId, DateTime date, string identifier);

        /// <summary>
        ///     Checks if the user is authenticated on a device
        /// </summary>
        /// <param name="userId">The user</param>
        /// <param name="identifier">An unique identifier for the device the user is on</param>
        /// <returns></returns>
        bool IsAuthenticated(Guid userId, string identifier);

        /// <summary>
        ///     Checks if the user has to use Two Factor Authentication
        /// </summary>
        /// <param name="userId">The user id (guid)</param>
        /// <returns></returns>
        bool IsDisabled(Guid userId);

        /// <summary>
        ///     Sets the value indicating Two Factor Authentication is disabled to false
        /// </summary>
        /// <param name="userId"></param>
        void Enable(Guid userId);

        /// <summary>
        ///      Sets the value indicating Two Factor Authentication is disabled to true
        /// </summary>
        /// <param name="userId"></param>
        void Disable(Guid userId);

        /// <summary>
        ///     Sets all failed attempt fields back to 0;
        /// </summary>
        /// <param name="userId"></param>
        void ResetAllFailedAttempts(Guid userId);
    }
}