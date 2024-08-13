using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiFactorAuthentication.Models;
using MultiFactorAuthentication.Services;
using MultiFactorAuthenticationTests.Mockups;
using System;

namespace MultiFactorAuthenticationTests.Services
{
    [TestClass]
    public class SmsAuthenticatorServiceTests
    {
        private AuthenticationRepositoryMockup _repo;
        private SmsServiceMockup _smsService;

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        [TestInitialize]
        public void MyTestInitialize()
        {
            _smsService = new SmsServiceMockup();
            _repo = new AuthenticationRepositoryMockup();
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CannotCreateSmsAuthenticatorServiceWithInvalidUser()
        {
            // Arrange
            var userId = new Guid();

            // Act
            SmsAuthenticatorService.Create(userId, _smsService, _repo);
        }

        [TestMethod]
        public void CreateSmsAuthenticatorWithRegisteredUser()
        {
            // Arrange
            var userId = new Guid();
            const string username = "user";

            var multifactorAuthenticatorService = new MultifactorAuthenticationService(_repo);
            multifactorAuthenticatorService.Register(userId, username);

            // Act
            var smsAuthenticatorService = multifactorAuthenticatorService.GetSmsAuthenticatorService(userId, _smsService);

            // Assert
            Assert.IsNotNull(smsAuthenticatorService);
            Assert.IsNull(smsAuthenticatorService.Phone);
        }

        [TestMethod]
        public void RegisterNewUser()
        {
            // Arrange
            var userId = new Guid();
            const string username = "user";
            const string phone = "+31612345678";

            var multifactorAuthenticatorService = new MultifactorAuthenticationService(_repo);
            multifactorAuthenticatorService.Register(userId, username);

            // Act
            var smsAuthenticatorService = multifactorAuthenticatorService.GetSmsAuthenticatorService(userId, _smsService);
            smsAuthenticatorService.Register(phone);

            // Assert
            Assert.AreEqual(phone, smsAuthenticatorService.Phone);
            Assert.AreEqual(MultiFactorAuthenticators.Sms, multifactorAuthenticatorService.Registrations(userId));
        }

        [TestMethod]
        public void ChangePhoneNumber()
        {
            // Arrange
            var userId = new Guid();
            var username = "user";
            const string phone = "+31612345678";
            const string newPhone = "+31612345000";

            var multifactorAuthenticatorService = new MultifactorAuthenticationService(_repo);
            multifactorAuthenticatorService.Register(userId, username);

            // Act
            var smsAuthenticatorService = multifactorAuthenticatorService.GetSmsAuthenticatorService(userId, _smsService);
            smsAuthenticatorService.Register(phone);

            if (string.Equals(smsAuthenticatorService.Phone, phone))
                smsAuthenticatorService.ChangePhone(newPhone);

            // Assert
            Assert.AreEqual(newPhone, smsAuthenticatorService.Phone);
            Assert.AreEqual(MultiFactorAuthenticators.Sms, multifactorAuthenticatorService.Registrations(userId));
        }

        [TestMethod]
        public void CanSendToNewUser()
        {
            // Arrange
            var userId = new Guid();
            const string username = "user";
            const string phone = "+31612345678";

            var multifactorAuthenticatorService = new MultifactorAuthenticationService(_repo);
            multifactorAuthenticatorService.Register(userId, username);

            // Act
            var smsAuthenticatorService = multifactorAuthenticatorService.GetSmsAuthenticatorService(userId, _smsService);
            smsAuthenticatorService.Register(phone);
            smsAuthenticatorService.SendSecurityCode();

            // Assert
            Assert.AreEqual(phone, _smsService.LastTo);
            Assert.IsNotNull(_smsService.LastMessage);

            var code = _repo.Find(userId).SmsSecret;
            var message = string.Format(smsAuthenticatorService.Message, code);

            Assert.AreEqual(message, _smsService.LastMessage);
        }

        [TestMethod]
        public void CannotSendToDisabledUser()
        {
            // Arrange
            var userId = new Guid();
            const string username = "user";
            const string phone = "+31612345678";

            var multifactorAuthenticatorService = new MultifactorAuthenticationService(_repo);
            multifactorAuthenticatorService.Register(userId, username);

            // Act
            var smsAuthenticatorService = multifactorAuthenticatorService.GetSmsAuthenticatorService(userId, _smsService);
            smsAuthenticatorService.Register(phone);
            smsAuthenticatorService.Disable();
            smsAuthenticatorService.SendSecurityCode();

            // Assert
            Assert.IsNull(_smsService.LastTo);
            Assert.IsNull(_smsService.LastMessage);
        }

        [TestMethod]
        public void CanValidateRegistration()
        {
            // Arrange
            var userId = new Guid();
            const string username = "user";
            const string phone = "+31612345678";

            var multifactorAuthenticatorService = new MultifactorAuthenticationService(_repo);
            multifactorAuthenticatorService.Register(userId, username);

            // Act
            var smsAuthenticatorService = multifactorAuthenticatorService.GetSmsAuthenticatorService(userId, _smsService);
            smsAuthenticatorService.Register(phone);
            smsAuthenticatorService.SendSecurityCode();

            var code = _repo.Find(userId).SmsSecret;
            var result = smsAuthenticatorService.ValidateRegistration(code);

            // Assert
            Assert.AreEqual(result, ValidationStatus.Success);
        }


        [TestMethod]
        public void CannotValidateBeforeRegistrationValidation()
        {
            // Arrange
            var userId = new Guid();
            const string username = "user";
            const string phone = "+31612345678";

            var multifactorAuthenticatorService = new MultifactorAuthenticationService(_repo);
            multifactorAuthenticatorService.Register(userId, username);

            // Act
            var smsAuthenticatorService = multifactorAuthenticatorService.GetSmsAuthenticatorService(userId, _smsService);
            smsAuthenticatorService.Register(phone);
            smsAuthenticatorService.SendSecurityCode();

            var code = _repo.Find(userId).SmsSecret;
            var result = smsAuthenticatorService.Validate(code);

            // Assert
            Assert.AreEqual(result, ValidationStatus.Failed);
        }

        [TestMethod]
        public void CanValidateAfterRegistrationValidation()
        {
            // Arrange
            var userId = new Guid();
            const string username = "user";
            const string phone = "+31612345678";

            var multifactorAuthenticatorService = new MultifactorAuthenticationService(_repo);
            multifactorAuthenticatorService.Register(userId, username);

            // Act
            var smsAuthenticatorService = multifactorAuthenticatorService.GetSmsAuthenticatorService(userId, _smsService);
            smsAuthenticatorService.Register(phone);

            smsAuthenticatorService.SendSecurityCode();
            var code = _repo.Find(userId).SmsSecret;
            smsAuthenticatorService.ValidateRegistration(code);

            smsAuthenticatorService.SendSecurityCode();
            code = _repo.Find(userId).SmsSecret;
            var result = smsAuthenticatorService.Validate(code);

            // Assert
            Assert.AreEqual(result, ValidationStatus.Success);
        }

        [TestMethod]
        public void CannotValidateWithWrongCode()
        {
            // Arrange
            var userId = new Guid();
            const string username = "user";
            const string phone = "+31612345678";

            var multifactorAuthenticatorService = new MultifactorAuthenticationService(_repo);
            multifactorAuthenticatorService.Register(userId, username);

            // Act
            var smsAuthenticatorService = multifactorAuthenticatorService.GetSmsAuthenticatorService(userId, _smsService);
            smsAuthenticatorService.Register(phone);

            smsAuthenticatorService.SendSecurityCode();
            var code = _repo.Find(userId).SmsSecret;
            var result = smsAuthenticatorService.ValidateRegistration(code + "1");

            // Assert
            Assert.AreEqual(result, ValidationStatus.InvalidCode);
        }

        [TestMethod]
        public void WrongCodeIncrementsFailedAttempts()
        {
            CannotValidateWithWrongCode();

            var user = _repo.Find(new Guid());
            Assert.AreEqual(user.FailedSmsAttemptCount, 1);
        }

        [TestMethod]
        public void UserIsLockedOutAfterXFailedAttempts()
        {
            // Arrange
            var userId = new Guid();

            CanValidateRegistration();

            var multifactorAuthenticatorService = new MultifactorAuthenticationService(_repo);
            var smsAuthenticatorService = multifactorAuthenticatorService.GetSmsAuthenticatorService(userId, _smsService);
            smsAuthenticatorService.SendSecurityCode();

            var user = _repo.Find(userId);

            var code = user.SmsSecret;
            smsAuthenticatorService.Validate(code + "1"); //1
            smsAuthenticatorService.Validate(code + "1"); //2
            smsAuthenticatorService.Validate(code + "1"); //3
            smsAuthenticatorService.Validate(code + "1"); //4
            var result = smsAuthenticatorService.Validate(code + "1"); //5
            Assert.AreNotEqual(result, ValidationStatus.LockedOut);

            result = smsAuthenticatorService.Validate(code + "1"); //6
            Assert.AreEqual(result, ValidationStatus.LockedOut);
        }

        [TestMethod]
        public void CanVerifyPhoneNumberTest()
        {
            // Arrange
            var userId = new Guid();
            const string username = "user";
            const string phone = "+31612345678";

            var multifactorAuthenticatorService = new MultifactorAuthenticationService(_repo);
            multifactorAuthenticatorService.Register(userId, username);

            // Act
            var smsAuthenticatorService = multifactorAuthenticatorService.GetSmsAuthenticatorService(userId, _smsService);
            smsAuthenticatorService.Register(phone);

            var result = smsAuthenticatorService.VerifyPhoneNumber(phone);

            // Assert
            Assert.AreEqual(result, ValidationStatus.Success);
        }

        [TestMethod]
        public void CannotVerifyIncorrectPhoneNumberTest()
        {
            // Arrange
            var userId = new Guid();
            const string username = "user";
            const string phone = "+31612345678";

            var multifactorAuthenticatorService = new MultifactorAuthenticationService(_repo);
            multifactorAuthenticatorService.Register(userId, username);

            // Act
            var smsAuthenticatorService = multifactorAuthenticatorService.GetSmsAuthenticatorService(userId, _smsService);
            smsAuthenticatorService.Register(phone);

            var result = smsAuthenticatorService.VerifyPhoneNumber(phone + "1");

            // Assert
            Assert.AreEqual(result, ValidationStatus.Failed);
        }

        [TestMethod]
        public void UserIsLockedOutAfterVerifyPhoneNumberTest()
        {
            // Arrange
            var userId = new Guid();
            const string username = "user";
            const string phone = "+31612345678";

            var multifactorAuthenticatorService = new MultifactorAuthenticationService(_repo);
            multifactorAuthenticatorService.Register(userId, username);

            // Act
            var smsAuthenticatorService = multifactorAuthenticatorService.GetSmsAuthenticatorService(userId, _smsService);
            smsAuthenticatorService.Register(phone);

            smsAuthenticatorService.VerifyPhoneNumber(phone + "1"); //1
            smsAuthenticatorService.VerifyPhoneNumber(phone + "1"); //2
            smsAuthenticatorService.VerifyPhoneNumber(phone + "1"); //3
            smsAuthenticatorService.VerifyPhoneNumber(phone + "1"); //4
            smsAuthenticatorService.VerifyPhoneNumber(phone + "1"); //5
            var result = smsAuthenticatorService.VerifyPhoneNumber(phone + "1");

            var user = _repo.Find(userId);

            // Assert
            Assert.AreEqual(ValidationStatus.LockedOut, result);
            Assert.AreEqual(5, user.FailedPhoneNumberAttemptCount);
        }

        [TestMethod]
        public void UserIsLockedOutAfterSendPhoneNumberTest()
        {
            // Arrange
            var userId = new Guid();
            const string username = "user";
            const string phone = "+31612345678";

            var multifactorAuthenticatorService = new MultifactorAuthenticationService(_repo);
            multifactorAuthenticatorService.Register(userId, username);

            // Act
            var smsAuthenticatorService = multifactorAuthenticatorService.GetSmsAuthenticatorService(userId, _smsService);
            smsAuthenticatorService.Register(phone);

            smsAuthenticatorService.SendSecurityCode();
            smsAuthenticatorService.SendSecurityCode();
            smsAuthenticatorService.SendSecurityCode();
            smsAuthenticatorService.SendSecurityCode();
            smsAuthenticatorService.SendSecurityCode();
            var result = smsAuthenticatorService.SendSecurityCode();

            // Assert
            Assert.AreEqual(result, ValidationStatus.LockedOut);
        }

        [TestMethod]
        public void SendSecurityCodeCountIsResetAfterSuccess()
        {
            // Arrange
            var userId = new Guid();
            const string username = "user";
            const string phone = "+31612345678";

            var multifactorAuthenticatorService = new MultifactorAuthenticationService(_repo);
            multifactorAuthenticatorService.Register(userId, username);

            // Act
            var smsAuthenticatorService = multifactorAuthenticatorService.GetSmsAuthenticatorService(userId, _smsService);
            smsAuthenticatorService.Register(phone);

            smsAuthenticatorService.SendSecurityCode();
            smsAuthenticatorService.SendSecurityCode();
            smsAuthenticatorService.SendSecurityCode();
            var result = smsAuthenticatorService.SendSecurityCode();

            var user = _repo.Find(userId);

            // Assert
            Assert.AreEqual(result, ValidationStatus.Success);
            Assert.AreEqual(4, user.SendSmsCodeCount);

            smsAuthenticatorService.ValidateRegistration(user.SmsSecret);

            Assert.AreEqual(0, user.SendSmsCodeCount);
        }
    }
}