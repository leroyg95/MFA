using System;
using System.Collections.Specialized;
using System.Configuration.Fakes;
using System.Text;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiFactorAuthentication.Interfaces;
using MultiFactorAuthentication.Models;
using MultiFactorAuthentication.Services;
using MultiFactorAuthenticationTests.Mockups;
using OtpSharp;

namespace MultiFactorAuthenticationTests.Services
{
    [TestClass]
    public class TotpAuthenticatorServiceTests
    {
        private IAuthenticationRepository _repository;
        private MultifactorAuthenticationService _multifactorAuthenticationService;
        private static NameValueCollection _appSettings = new NameValueCollection();

        [TestInitialize]
        public void MyTestInitialize()
        {
            using(GetDefaultShimsContext())
            {
                _appSettings.Add( "app:issuer", "fake" );
                _appSettings.Add( "app:db", "db" );

                _repository = new AuthenticationRepositoryMockup();
                _multifactorAuthenticationService = new MultifactorAuthenticationService(_repository);
            }
        }
        
        public IDisposable GetDefaultShimsContext()
        {
            var result = ShimsContext.Create();

            ShimConfigurationManager.AppSettingsGet = () => _appSettings;

            return result;
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CannotCreateTotpAuthenticatorServiceWithInvalidUser()
        {
            // Arrange
            var userId = new Guid();

            // Act
            TotpAuthenticatorService.Create(userId, _repository);
        }

        [TestMethod]
        public void CreateTotpAuthenticatorWithRegisteredUser()
        {
            // Arrange
            var userId = new Guid();
            const string username = "user";

            _multifactorAuthenticationService.Register(userId, username);

            // Act
            var totpAuthenticatorService = _multifactorAuthenticationService.GetTotpAuthenticatorService(userId);

            // Assert
            Assert.IsNotNull(totpAuthenticatorService);
            Assert.IsNull(totpAuthenticatorService.OtpUri());
        }

        [TestMethod]
        public void CanCreateQrCode()
        {
            using (GetDefaultShimsContext())
            {
                _appSettings.Add("app:issuer", "fake");
                // Arrange
                var userId = new Guid();
                const string username = "user";

                _multifactorAuthenticationService.Register(userId, username);

                // Act
                var totpAuthenticatorService = _multifactorAuthenticationService.GetTotpAuthenticatorService(userId);
                totpAuthenticatorService.Enable();

                var otpUri = totpAuthenticatorService.OtpUri();
                var qrcode = QRCodeGenerator.Generate(otpUri.Uri);

                // Assert
                Assert.IsNotNull(qrcode);
            }
        }

        [TestMethod]
        public void CannotSendToDisabledUser()
        {
            // Arrange
            var userId = new Guid();
            const string username = "user";

            _multifactorAuthenticationService.Register(userId, username);

            // Act
            var totpAuthenticatorService = _multifactorAuthenticationService.GetTotpAuthenticatorService(userId);
            totpAuthenticatorService.Enable();
            totpAuthenticatorService.Disable();

            // Assert
            Assert.IsNull(totpAuthenticatorService.OtpUri());
        }

        [TestMethod]
        public void CanValidateRegistration()
        {
            // Arrange
            var userId = new Guid();
            const string username = "user";

            _multifactorAuthenticationService.Register(userId, username);

            // Act
            var totpAuthenticatorService = _multifactorAuthenticationService.GetTotpAuthenticatorService(userId);
            totpAuthenticatorService.Enable();

            var secret = _repository.Find(userId).TotpSecret;
            var totp = new Totp(Encoding.Default.GetBytes(secret));
            var code = totp.ComputeTotp();
            var result = totpAuthenticatorService.ValidateRegistration(code);

            // Assert
            Assert.AreEqual(result, ValidationStatus.Success);
        }


        [TestMethod]
        public void CannotValidateBeforeRegistrationValidation()
        {
            // Arrange
            var userId = new Guid();
            const string username = "user";

            _multifactorAuthenticationService.Register(userId, username);

            // Act
            var totpAuthenticatorService = _multifactorAuthenticationService.GetTotpAuthenticatorService(userId);
            totpAuthenticatorService.Enable();

            var secret = _repository.Find(userId).TotpSecret;
            var totp = new Totp(Encoding.Default.GetBytes(secret));
            var code = totp.ComputeTotp();

            var result = totpAuthenticatorService.Validate(code);

            // Assert
            Assert.AreEqual(result, ValidationStatus.Failed);
        }

        [TestMethod]
        public void CanValidateAfterRegistrationValidation()
        {
            // Arrange
            var userId = new Guid();
            const string username = "user";

            _multifactorAuthenticationService.Register(userId, username);

            // Act
            var totpAuthenticatorService = _multifactorAuthenticationService.GetTotpAuthenticatorService(userId);
            totpAuthenticatorService.Enable();

            var secret = _repository.Find(userId).TotpSecret;
            var totp = new Totp(Encoding.Default.GetBytes(secret));
            var code = totp.ComputeTotp();

            totpAuthenticatorService.ValidateRegistration(code);

            code = totp.ComputeTotp(DateTime.UtcNow.AddSeconds(30));
            var result = totpAuthenticatorService.Validate(code);

            // Assert
            Assert.AreEqual(result, ValidationStatus.Success);
        }

        [TestMethod]
        public void CannotValidateWithWrongCode()
        {
            // Arrange
            var userId = new Guid();
            const string username = "user";

            _multifactorAuthenticationService.Register(userId, username);

            // Act
            var totpAuthenticatorService = _multifactorAuthenticationService.GetTotpAuthenticatorService(userId);
            totpAuthenticatorService.Enable();

            var secret = _repository.Find(userId).TotpSecret;
            var totp = new Totp(Encoding.Default.GetBytes(secret));
            var code = totp.ComputeTotp();
            var result = totpAuthenticatorService.ValidateRegistration(code + "1");

            // Assert
            Assert.AreEqual(result, ValidationStatus.InvalidCode);
        }

        [TestMethod]
        public void WrongCodeIncrementsFailedAttempts()
        {
            CannotValidateWithWrongCode();

            var user = _repository.Find(new Guid());
            Assert.AreEqual(user.FailedTotpAttemptCount, 1);
        }

        [TestMethod]
        public void UserIsLockedOutAfterXFailedAttempts()
        {
            // Arrange
            var userId = new Guid();

            CanValidateRegistration();

            var totpAuthenticatorService = _multifactorAuthenticationService.GetTotpAuthenticatorService(userId);
            totpAuthenticatorService.Enable();

            var user = _repository.Find(userId);

            var code = user.SmsSecret;
            totpAuthenticatorService.Validate(code + "1"); //1
            totpAuthenticatorService.Validate(code + "1"); //2
            totpAuthenticatorService.Validate(code + "1"); //3
            totpAuthenticatorService.Validate(code + "1"); //4
            var result = totpAuthenticatorService.Validate(code + "1"); //5
            Assert.AreNotEqual(result, ValidationStatus.LockedOut);

            result = totpAuthenticatorService.Validate(code + "1"); //6
            Assert.AreEqual(result, ValidationStatus.LockedOut);
        }
    }
}