#region

using System;
using System.Text;
using MultiFactorAuthentication.Helpers;
using MultiFactorAuthentication.Interfaces;
using MultiFactorAuthentication.Models;
using OtpSharp;

#endregion

namespace MultiFactorAuthentication.Services
{
    /// <inheritdoc />
    public class TotpAuthenticatorService : ITotpAuthenticatorService
    {
        private readonly IAuthenticationRepository _repository;
        private readonly Authentication _user;

        private TotpAuthenticatorService(Guid userId, IAuthenticationRepository repository)
        {
            _repository = repository;

            _user = _repository.Find(userId);

            if (_user == null)
                throw new InvalidOperationException("User is not registered");
        }

        /// <summary>
        ///     Creates a TotpAuthenticatorService for the specified user 
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <param name="repository">The repository to be used</param>
        /// <returns>The TotpAuthenticatorService</returns>
        protected internal static TotpAuthenticatorService Create(Guid userId, IAuthenticationRepository repository) => new TotpAuthenticatorService(userId, repository);

        /// <inheritdoc />
        public OtpUri OtpUri() => GetOtpUri();

        /// <inheritdoc />
        public bool Enable()
        {
            if (_user.TotpStatus != Status.Disabled)
                return false;

            _user.TotpSecret = TotpCodeGenerator.GenerateCode();
            _user.TotpStatus = Status.Pending;

            _repository.Save(_user);

            return true;
        }

        /// <inheritdoc />
        public bool Disable()
        {
            if (_user.TotpStatus == Status.Disabled)
                return false;

            _user.TotpStatus = Status.Disabled;

            _repository.Save(_user);

            return true;
        }

        /// <inheritdoc />
        public void Reset()
        {
            Disable();
            ResetFailedTotpAttemptCount();
            ResetFailedHotpAttemptCount();
            Enable();
        }

        /// <inheritdoc />
        public void ResetFailedTotpAttemptCount()
        {
            _user.FailedTotpAttemptCount = 0;
            _repository.Save(_user);
        }

        /// <inheritdoc />
        public void ResetFailedHotpAttemptCount()
        {
            _user.FailedHotpAttemptCount = 0;
            _repository.Save(_user);
        }

        /// <inheritdoc />
        public ValidationStatus ValidateRegistration(string code)
        {
            if (_user.TotpStatus != Status.Pending)
                return ValidationStatus.Failed;

            var result = InternalValidate(code);

            if (result != ValidationStatus.Success)
                return result;

            _user.TotpStatus = Status.Validated;
            _repository.Save(_user);

            return result;
        }

        /// <inheritdoc />
        public ValidationStatus Validate(string code) => _user.TotpStatus != Status.Validated ? ValidationStatus.Failed : InternalValidate(code);

        private OtpUri GetOtpUri(PasswordType type = PasswordType.Totp, int counter = 0)
        {
            if (_user.TotpStatus != Status.Pending)
                return null;

            var secret = Base32Encode(_user.TotpSecret);
            var issuer = SettingsService.Instance.Issuer;
            var period = SettingsService.Instance.Period;
            var username = _user.Username;


            var uri = $"otpauth://{type.ToString().ToLower()}/{issuer}:{username}?secret={secret}&issuer={issuer}&period={period}";

            if (type == PasswordType.Hotp)
                uri += $"&counter={counter}";

            return new OtpUri
            {
                Uri = uri,
                Secret = secret
            };
        }

        private static string Base32Encode(string str) => Helpers.Base32.ToBase32String(Encoding.Default.GetBytes(str));

        private ValidationStatus InternalValidate(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException(nameof(code));

            // settings
            var secret = _user.TotpSecret;
            var window = new VerificationWindow(10, 10);
            var period = SettingsService.Instance.Period;
            var future = SettingsService.Instance.Future;
            var past = SettingsService.Instance.Past;
            var maxAttemptCount = SettingsService.Instance.MaxFailedTotpAttemptCount;

            if (_user.FailedTotpAttemptCount >= maxAttemptCount)
            {
                return ValidationStatus.LockedOut;
            }

            // execute totp verification
            var totp = new Totp(Encoding.Default.GetBytes(secret));
            var verified = totp.VerifyTotp(code, out var timeWindowUsed, window);

            // gather results
            if (!verified)
            {
                _user.FailedTotpAttemptCount++;
                _repository.Save(_user);

                return ValidationStatus.InvalidCode;
            }

            if (_user.TimeWindowUsed == timeWindowUsed)
                return ValidationStatus.CodeAlreadyUsed;

            var timeUsed = 621355968000000000L + timeWindowUsed * 10000000L * period;
            var date = new DateTime().AddTicks(timeUsed);

            if (date - DateTime.UtcNow > TimeSpan.FromSeconds(future) ||
                DateTime.UtcNow - date > TimeSpan.FromSeconds(past))
                return ValidationStatus.IncorrectTime;

            // store the timewindow
            _user.TimeWindowUsed = timeWindowUsed;
            _user.FailedHotpAttemptCount = 0;
            _user.FailedTotpAttemptCount = 0;
            _repository.Save(_user);
            
            return ValidationStatus.Success;
        }
    }
}