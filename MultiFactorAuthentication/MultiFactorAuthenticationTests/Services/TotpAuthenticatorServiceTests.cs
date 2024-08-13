using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiFactorAuthentication.Models;
using MultiFactorAuthentication.Services;
using MultiFactorAuthenticationTests.Mockups;
using OtpSharp;

namespace MultiFactorAuthenticationTests.Services
{
    [TestClass()]
    public class TotpAuthenticatorServiceTests
    {

        private AuthenticationRepositoryMockup _repo;

        #region Additional test attributes
        [TestInitialize]
        public void MyTestInitialize()
        {
            _repo = new AuthenticationRepositoryMockup();
        }

        #endregion

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CannotCreateTotpAuthenticatorServiceWithInvalidUser()
        {
            // Arrange
            var userId = new Guid();

            // Act
            TotpAuthenticatorService.Create(userId, _repo);
        }

        [TestMethod]
        public void CreateTotpAuthenticatorWithRegisteredUser()
        {
            // Arrange
            var userId = new Guid();
            const string username = "user";

            var multifactorAuthenticatorService = new MultifactorAuthenticationService(_repo);
            multifactorAuthenticatorService.Register(userId, username);

            // Act
            var totpAuthenticatorService = multifactorAuthenticatorService.GetTotpAuthenticatorService(userId);

            // Assert
            Assert.IsNotNull(totpAuthenticatorService);
            Assert.IsNull(totpAuthenticatorService.OtpUri());
        }

        [TestMethod]
        public void CanCreateQrCode()
        {
            // Arrange
            var userId = new Guid();
            const string username = "user";

            var multifactorAuthenticatorService = new MultifactorAuthenticationService(_repo);
            multifactorAuthenticatorService.Register(userId, username);

            // Act
            var totpAuthenticatorService = multifactorAuthenticatorService.GetTotpAuthenticatorService(userId);
            totpAuthenticatorService.Enable();

            var otpUri = totpAuthenticatorService.OtpUri();
            var qrcode = QRCodeGenerator.Generate(otpUri.Uri);

            // Assert
            Assert.IsNotNull(qrcode);
        }

        [TestMethod]
        public void CannotSendToDisabledUser()
        {
            // Arrange
            var userId = new Guid();
            const string username = "user";

            var multifactorAuthenticatorService = new MultifactorAuthenticationService(_repo);
            multifactorAuthenticatorService.Register(userId, username);

            // Act
            var totpAuthenticatorService = multifactorAuthenticatorService.GetTotpAuthenticatorService(userId);
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

            var multifactorAuthenticatorService = new MultifactorAuthenticationService(_repo);
            multifactorAuthenticatorService.Register(userId, username);

            // Act
            var totpAuthenticatorService = multifactorAuthenticatorService.GetTotpAuthenticatorService(userId);
            totpAuthenticatorService.Enable();

            var secret = _repo.Find(userId).TotpSecret;
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

            var multifactorAuthenticatorService = new MultifactorAuthenticationService(_repo);
            multifactorAuthenticatorService.Register(userId, username);

            // Act
            var totpAuthenticatorService = multifactorAuthenticatorService.GetTotpAuthenticatorService(userId);
            totpAuthenticatorService.Enable();

            var secret = _repo.Find(userId).TotpSecret;
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

            var multifactorAuthenticatorService = new MultifactorAuthenticationService(_repo);
            multifactorAuthenticatorService.Register(userId, username);

            // Act
            var totpAuthenticatorService = multifactorAuthenticatorService.GetTotpAuthenticatorService(userId);
            totpAuthenticatorService.Enable();

            var secret = _repo.Find(userId).TotpSecret;
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

            var multifactorAuthenticatorService = new MultifactorAuthenticationService(_repo);
            multifactorAuthenticatorService.Register(userId, username);

            // Act
            var totpAuthenticatorService = multifactorAuthenticatorService.GetTotpAuthenticatorService(userId);
            totpAuthenticatorService.Enable();

            var secret = _repo.Find(userId).TotpSecret;
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

            var user = _repo.Find(new Guid());
            Assert.AreEqual(user.FailedTotpAttemptCount, 1);
        }

        [TestMethod]
        public void UserIsLockedOutAfterXFailedAttempts()
        {
            // Arrange
            var userId = new Guid();

            CanValidateRegistration();

            var multifactorAuthenticatorService = new MultifactorAuthenticationService(_repo);
            var totpAuthenticatorService = multifactorAuthenticatorService.GetTotpAuthenticatorService(userId);
            totpAuthenticatorService.Enable();

            var user = _repo.Find(userId);

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