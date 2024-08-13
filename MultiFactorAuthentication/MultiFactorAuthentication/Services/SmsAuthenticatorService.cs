#region

using System;
using System.Text.RegularExpressions;
using MultiFactorAuthentication.Helpers;
using MultiFactorAuthentication.Interfaces;
using MultiFactorAuthentication.Models;
using MultiFactorAuthentication.Services.Sms;

#endregion

namespace MultiFactorAuthentication.Services
{
    /// <inheritdoc />
    public class SmsAuthenticatorService : ISmsAuthenticatorService
    {
        private readonly IAuthenticationRepository _repository;
        private readonly ISmsService _smsService;
        private readonly Authentication _user;

        private SmsAuthenticatorService(Guid userId, ISmsService smsService, IAuthenticationRepository repository)
        {
            Message = SettingsService.Instance.SmsMessage;

            _smsService = smsService;
            _repository = repository;
            _user = _repository.Find(userId);

            if (_user == null)
                throw new InvalidOperationException("User does not exist");
        }

        /// <summary>
        ///     Creates a SmsAuthenticatorService for the specified user with the specified sms provider
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <param name="smsService">The sms service to be used <see cref="ISmsService" /></param>
        /// <param name="repository">The repository to be used</param>
        /// <returns>The SmsAuthentictorService</returns>
        protected internal static SmsAuthenticatorService Create(Guid userId, ISmsService smsService,
            IAuthenticationRepository repository) => new SmsAuthenticatorService(userId,
            smsService ?? SmsProviderFactory.Create(SettingsService.Instance.SmsService), repository);

        /// <inheritdoc />
        public string Phone => _user.PhoneNumber;

        /// <inheritdoc />
        public string Message { get; set; }

        /// <inheritdoc />
        public void Register(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentNullException(nameof(phone));

            if (!IsValidPhone(phone))
                throw new InvalidOperationException("Phone is not a valid number");

            if (_user.SmsStatus != Status.Disabled)
                throw new InvalidOperationException("There is already a phone registered");

            _user.SmsStatus = Status.Pending;
            _user.PhoneNumber = phone;

            _repository.Save(_user);
        }

        /// <inheritdoc />
        public void ChangePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentNullException(nameof(phone));

            if (!IsValidPhone(phone))
                throw new InvalidOperationException("Phone is not a valid number");

            _user.SmsStatus = Status.Pending;
            _user.PhoneNumber = phone;

            _repository.Save(_user);
        }

        /// <inheritdoc />
        public bool IsValidPhone(string phone) => Regex.IsMatch(phone, @"^\+[1-9]\d{8,14}$");

        /// <inheritdoc />
        public bool Disable()
        {
            if (_user.SmsStatus == Status.Disabled)
                return false;

            _user.SmsStatus = Status.Disabled;

            _repository.Save(_user);
            return true;
        }

        /// <inheritdoc />
        public void Reset()
        {
            ResetFailedPhoneNumberAttemptCount();
            ResetFailedSmsAttemptCount();
            ResetSendSmsCodeCount();
            Disable();
        }

        /// <inheritdoc />
        public ValidationStatus SendSecurityCode()
        {
            if (_user.SmsStatus == Status.Disabled)
                return ValidationStatus.Failed;

            if (_user.SendSmsCodeCount >= 5)
                return ValidationStatus.LockedOut;

            var code = SmsCodeGenerator.GenerateCode();
            var message = string.Format(Message, code);
            var now = DateTime.Now;

            _user.SmsIssued = now;
            _user.SmsSecret = code;
            _user.SendSmsCodeCount++;

            _repository.Save(_user);

            _smsService.Send(_user.PhoneNumber, message);

            return ValidationStatus.Success;
        }

        /// <inheritdoc />
        public ValidationStatus VerifyPhoneNumber(string phone)
        {
            if (_user.FailedPhoneNumberAttemptCount >= SettingsService.Instance.MaxFailedPhoneAttemptCount)
                return ValidationStatus.LockedOut;

            ValidationStatus result;

            if (phone == Phone)
            {
                _user.FailedPhoneNumberAttemptCount = 0;
                result = ValidationStatus.Success;
            }
            else
            {
                _user.FailedPhoneNumberAttemptCount++;
                result = ValidationStatus.Failed;
            }

            _repository.Save(_user);
            return result;
        }

        /// <inheritdoc />
        public void ResetFailedSmsAttemptCount()
        {
            _user.FailedSmsAttemptCount = 0;
            _repository.Save(_user);
        }

        /// <inheritdoc />
        public void ResetFailedPhoneNumberAttemptCount()
        {
            _user.FailedPhoneNumberAttemptCount = 0;
            _repository.Save(_user);
        }

        /// <inheritdoc />
        public void ResetSendSmsCodeCount()
        {
            _user.SendSmsCodeCount = 0;
            _repository.Save(_user);
        }

        /// <inheritdoc />
        public ValidationStatus Validate(string code) =>
            _user.SmsStatus == Status.Validated ? InternalValidate(code) : ValidationStatus.Failed;

        /// <inheritdoc />
        public ValidationStatus ValidateRegistration(string code)
        {
            if (_user.SmsStatus != Status.Pending)
                return ValidationStatus.Failed;

            var result = InternalValidate(code);

            if (result != ValidationStatus.Success)
                return result;

            _user.SmsStatus = Status.Validated;
            _repository.Save(_user);
            return result;
        }

        private ValidationStatus InternalValidate(string code)
        {
            if (_user.SmsIssued == null || string.IsNullOrEmpty(_user.SmsSecret))
                return ValidationStatus.Failed;

            if (_user.FailedSmsAttemptCount >= SettingsService.Instance.MaxFailedSmsAttemptCount)
                return ValidationStatus.LockedOut;

            var clean = false;
            ValidationStatus result;
            var now = DateTime.Now;

            // Check if datetime is in range
            if (_user.SmsIssued <= now && now <= _user.SmsIssued.Value.Add(SettingsService.Instance.SmsWindow))
            {
                // Check if code is correct
                if (code == _user.SmsSecret)
                {
                    clean = true;
                    result = ValidationStatus.Success;
                    _user.FailedSmsAttemptCount = 0;
                    _user.SendSmsCodeCount = 0;
                }
                else
                {
                    _user.FailedSmsAttemptCount++;
                    result = ValidationStatus.InvalidCode;
                }
            }
            else // datetime is not within range
            {
                clean = true;
                result = ValidationStatus.IncorrectTime;
            }

            if (clean)
            {
                _user.SmsIssued = null;
                _user.SmsSecret = null;
            }

            _repository.Save(_user);

            return result;
        }
    }
}