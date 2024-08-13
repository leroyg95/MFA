using System;
using System.Collections.Generic;
using System.Linq;
using MultiFactorAuthentication.Interfaces;
using MultiFactorAuthentication.Models;
using MultiFactorAuthentication.Repository;

namespace MultiFactorAuthentication.Services
{
    /// <inheritdoc />
    public class MultifactorAuthenticationService : IMultifactorAuthenticatorService
    {
        private readonly IAuthenticationRepository _repository;

        /// <inheritdoc />
        /// <summary>
        ///     Default constructor, uses default application settings for setting up the database
        /// </summary>
        public MultifactorAuthenticationService()
            : this(SettingsService.Instance.NameOrConnectionString)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Constructor, uses a specified database.
        /// </summary>
        /// <param name="nameOrConnectionString"></param>
        public MultifactorAuthenticationService(string nameOrConnectionString)
            : this(AuthenticationRepository.Create(nameOrConnectionString))
        {
        }

        /// <summary>
        ///     Constructor, uses a specified authenticationRepository
        /// </summary>
        /// <param name="repository">the authentication repostory</param>
        public MultifactorAuthenticationService(IAuthenticationRepository repository) => _repository = repository;

        /// <inheritdoc />
        public void Delete(Guid userId) => _repository.Remove(userId);

        /// <inheritdoc />
        public ISmsAuthenticatorService GetSmsAuthenticatorService(Guid userId, ISmsService smsService = null) => SmsAuthenticatorService.Create(userId, smsService, _repository);

        /// <inheritdoc />
        public ITotpAuthenticatorService GetTotpAuthenticatorService(Guid userId) => TotpAuthenticatorService.Create(userId, _repository);

        /// <inheritdoc />
        public void Register(Guid userId, string username)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(username));

            if (UserExists(userId))
                throw new InvalidOperationException("User already exists");

            var authentication = new Authentication
            {
                UserId = userId,
                Username = username
            };

            _repository.Add(authentication);
        }

        /// <inheritdoc />
        public MultiFactorAuthenticators Registrations(Guid userId)
        {
            var authentication = _repository.Find(userId);

            if (authentication == null)
                throw new Exception("user does not exists");

            var result = MultiFactorAuthenticators.None;

            if (authentication.TotpStatus != Status.Disabled)
                result |= MultiFactorAuthenticators.Totp;

            if (authentication.SmsStatus != Status.Disabled)
                result |= MultiFactorAuthenticators.Sms;

            return result;
        }

        /// <inheritdoc />
        public MultiFactorAuthenticators ValidatedRegistrations(Guid userId)
        {
            var authentication = _repository.Find(userId);

            if (authentication == null)
                throw new Exception("user does not exists");

            var result = MultiFactorAuthenticators.None;

            if (authentication.TotpStatus == Status.Validated)
                result |= MultiFactorAuthenticators.Totp;

            if (authentication.SmsStatus == Status.Validated)
                result |= MultiFactorAuthenticators.Sms;

            return result;
        }

        /// <inheritdoc />
        public bool UserExists(Guid userId) => _repository.Find(userId) != null;

        /// <inheritdoc />
        public void RememberLogin(Guid userId, DateTime date, string identifier)
        {
            var authentication = _repository.Find(userId);

            if (authentication.Devices != null && authentication.Devices.Any())
            {
                var device = authentication.Devices.FirstOrDefault();

                device.ExpiryDate = date.Date;
                device.DeviceToken = identifier;
            }
            else
            {
                authentication.Devices = new List<AuthDevices>()
                {
                    new AuthDevices
                    {
                        ExpiryDate = date.Date,
                        DeviceToken = identifier
                    }
                };
            }

            _repository.Save(authentication);
        }

        /// <inheritdoc />
        public bool IsAuthenticated(Guid userId, string identifier)
        {
            var authentication = _repository.Find(userId);

            return authentication.Devices != null && authentication.Devices.Any() && authentication.Devices
                       .Where(device => device.DeviceToken == identifier)
                       .Any(device => device.ExpiryDate > DateTime.Now);
        }

        /// <inheritdoc />
        public bool IsDisabled(Guid userId)
        {
            var authentication = _repository.Find(userId);
            return authentication == null || authentication.Disabled;
        }

        /// <inheritdoc />
        public void Enable(Guid userId)
        {
            var authentication = _repository.Find(userId);
            authentication.Disabled = false;
            _repository.Save(authentication);
        }

        /// <inheritdoc />
        public void Disable(Guid userId)
        {
            var authentication = _repository.Find(userId);
            authentication.Disabled = true;
            _repository.Save(authentication);
        }

        /// <inheritdoc />
        public void ResetAllFailedAttempts(Guid userId)
        {
            var smsAuthenticatorService = GetSmsAuthenticatorService(userId);
            var totpAuthenticatorService = GetTotpAuthenticatorService(userId);
            smsAuthenticatorService.ResetFailedPhoneNumberAttemptCount();
            smsAuthenticatorService.ResetFailedSmsAttemptCount();
            smsAuthenticatorService.ResetSendSmsCodeCount();
            totpAuthenticatorService.ResetFailedTotpAttemptCount();
            totpAuthenticatorService.ResetFailedHotpAttemptCount();
        }
    }
}