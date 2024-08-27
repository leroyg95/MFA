using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiFactorAuthentication.Helpers;
using MultiFactorAuthentication.Interfaces;
using MultiFactorAuthentication.Models;
using MultiFactorAuthentication.Services;
using MultiFactorAuthenticationTests.Mockups;
using System;
using System.Collections.Specialized;
using System.Configuration.Fakes;

namespace MultiFactorAuthenticationTests.Services
{
    /// <summary>
    /// Summary description for MultifactorAuthenticationServiceTests
    /// </summary>
    [TestClass]
    public class MultifactorAuthenticationServiceTests
    {

        private IAuthenticationRepository _repository;
        private MultifactorAuthenticationService _multifactorAuthenticationService;
        private static NameValueCollection _appSettings = new NameValueCollection();

        // Use TestInitialize to run code before running each test 
        [TestInitialize]
        public void MyTestInitialize()
        {
            using(GetDefaultShimsContext())
            {
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
        public void Register()
        {
            // Arrange
            var userId = new Guid();
            var username = "username";
            var service = new MultifactorAuthenticationService( _repository );

            // Act
            service.Register( userId, username );

            // Assert
            var authentication = _repository.Find( userId );

            Assert.IsNotNull( authentication );
            Assert.AreEqual( userId, authentication.UserId );
            Assert.AreEqual( username, authentication.Username );
        }

        [TestMethod]
        [ExpectedException( typeof( InvalidOperationException ) )]
        public void CannotRegisterTwice()
        {
            // Arrange
            var userId = new Guid();
            var username = "username";
            var service = new MultifactorAuthenticationService( _repository );

            // Act
            service.Register( userId, username );
            service.Register( userId, username );
        }

        [TestMethod]
        public void NoRegistrationsAfterFreshRegistration()
        {
            // Arrange
            var userId = new Guid();
            var username = "username";
            var service = new MultifactorAuthenticationService( _repository );

            // Act
            service.Register( userId, username );
            var registrations = service.Registrations( userId );

            // Assert
            Assert.AreEqual( MultiFactorAuthenticators.None, registrations );
        }

        [TestMethod]
        public void UserExistsAfterRegistration()
        {
            // Arrange
            var userId = new Guid();
            var username = "username";
            var service = new MultifactorAuthenticationService( _repository );

            // Act
            service.Register( userId, username );
            var userExists = service.UserExists( userId );

            // Assert
            Assert.IsTrue( userExists );
        }

        [TestMethod]
        public void UserDoesNotExistsInitially()
        {
            // Arrange
            var userId = new Guid();
            var service = new MultifactorAuthenticationService( _repository );

            // Act
            var userExists = service.UserExists( userId );

            // Assert
            Assert.IsFalse( userExists );
        }

        [TestMethod]
        public void UserDoesNotExistsAfterDeletion()
        {
            // Arrange
            var userId = new Guid();
            var username = "username";
            var service = new MultifactorAuthenticationService( _repository );

            // Act
            service.Register( userId, username );
            service.Delete( userId );
            var userExists = service.UserExists( userId );

            // Assert
            Assert.IsFalse( userExists );
        }


        [TestMethod]
        public void UserHasSmsMultiFactorAuthenticator()
        {
            // Arrange
            var userId = new Guid();
            var username = "username";
            var service = new MultifactorAuthenticationService( _repository );

            // Act
            service.Register( userId, username );

            var user = _repository.Find( userId );
            user.SmsStatus = Status.Pending;

            _repository.Save( user );

            var registrations = service.Registrations( userId );

            // Assert
            Assert.AreEqual( MultiFactorAuthenticators.Sms, registrations );
        }

        [TestMethod]
        public void UserHasTotpMultiFactorAuthenticator()
        {
            // Arrange
            var userId = new Guid();
            var username = "username";

            var service = new MultifactorAuthenticationService( _repository );

            // Act
            service.Register( userId, username );

            var user = _repository.Find( userId );
            user.TotpStatus = Status.Pending;
            _repository.Save( user );

            var registrations = service.Registrations( userId );

            // Assert
            Assert.AreEqual( MultiFactorAuthenticators.Totp, registrations );
        }


        [TestMethod]
        public void UserHasBothMultiFactorAuthenticators()
        {
            // Arrange
            var userId = new Guid();
            var username = "username";

            var service = new MultifactorAuthenticationService( _repository );

            // Act
            service.Register( userId, username );

            var user = _repository.Find( userId );
            user.TotpStatus = Status.Pending;
            user.SmsStatus = Status.Pending;

            _repository.Save( user );

            var registrations = service.Registrations( userId );

            // Assert
            Assert.IsTrue( registrations.HasFlag( MultiFactorAuthenticators.Totp ) );
            Assert.IsTrue( registrations.HasFlag( MultiFactorAuthenticators.Sms ) );
        }

        [TestMethod]
        public void RememberLoginTest()
        {
            var userId = Guid.NewGuid();
            var username = "username";
            var identifier = CodeGenerator.GenerateCode( 128, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" );
            var expiryDate = DateTime.Now.AddDays( 30 );


            // Act
            _multifactorAuthenticationService.Register( userId, username );

            _multifactorAuthenticationService.RememberLogin( userId, expiryDate, identifier );

            // Assert

            Assert.IsTrue( _multifactorAuthenticationService.IsAuthenticated( userId, identifier ) );
        }

        [TestMethod]
        public void GenerateCodeIsCorrect()
        {
            var identifier = CodeGenerator.GenerateCode( 16, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" );

            Assert.AreEqual( identifier.Length, 16 );
        }

        [TestMethod]
        public void TfaDisabledWhenUserNotExisting()
        {
            // Arrange
            var userId = new Guid();
            var service = new MultifactorAuthenticationService( _repository );

            // Act
            var tfaDisabled = service.IsDisabled( userId );

            // Assert
            Assert.IsTrue( tfaDisabled );
        }

        [TestMethod]
        public void TfaNotDisabledOnNewUser()
        {
            // Arrange
            var userId = new Guid();
            var username = "username";
            var service = new MultifactorAuthenticationService( _repository );
            service.Register( userId, username );

            // Act
            var tfaDisabled = service.IsDisabled( userId );

            // Assert
            Assert.IsFalse( tfaDisabled );
        }

        [TestMethod]
        public void DisableTfa()
        {
            // Arrange
            var userId = new Guid();
            var username = "username";
            var service = new MultifactorAuthenticationService( _repository );
            service.Register( userId, username );

            // Act
            service.Disable( userId );

            // Assert
            var tfaDisabled = service.IsDisabled( userId );
            Assert.IsTrue( tfaDisabled );
        }

        [TestMethod]
        public void EnableTfa()
        {
            // Arrange
            var userId = new Guid();
            var username = "username";
            var service = new MultifactorAuthenticationService( _repository );
            service.Register( userId, username );
            service.Disable( userId );

            // Act
            service.Enable( userId );

            // Assert
            var tfaDisabled = service.IsDisabled( userId );
            Assert.IsFalse( tfaDisabled );
        }

        [TestMethod]
        public void ResetAllFailedAttempts()
        {
            // Arrange
            var userId = new Guid();
            var username = "username";
            var service = new MultifactorAuthenticationService( _repository );
            service.Register( userId, username );
            var authentication = _repository.Find( userId );

            authentication.FailedHotpAttemptCount = 2;
            authentication.FailedPhoneNumberAttemptCount = 3;
            authentication.FailedSmsAttemptCount = 5;
            authentication.FailedTotpAttemptCount = 4;
            authentication.SendSmsCodeCount = 3;
            _repository.Save( authentication );

            // Act
            service.ResetAllFailedAttempts( userId );

            // Assert
            Assert.AreEqual( authentication.FailedHotpAttemptCount, 0 );
            Assert.AreEqual( authentication.FailedPhoneNumberAttemptCount, 0 );
            Assert.AreEqual( authentication.FailedSmsAttemptCount, 0 );
            Assert.AreEqual( authentication.FailedTotpAttemptCount, 0 );
            Assert.AreEqual( authentication.SendSmsCodeCount, 0 );
        }
    }
}
