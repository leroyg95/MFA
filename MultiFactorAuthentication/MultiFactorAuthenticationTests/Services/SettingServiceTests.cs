using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiFactorAuthentication.Services;
using System;
using System.Collections.Specialized;
using System.Configuration.Fakes;
using MultiFactorAuthentication.Exceptions;
using MultiFactorAuthentication.Services.Settings;

namespace MultiFactorAuthenticationTests.Services
{
    /// <summary>
    /// Summary description for SettingServiceTests
    /// </summary>
    [TestClass]
    public class SettingServiceTests
    {
        private NameValueCollection _appSettings;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

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
            _appSettings = new NameValueCollection();
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion


        public IDisposable GetDefaultShimsContext()
        {
            var result = ShimsContext.Create();
            
            ShimConfigurationManager.AppSettingsGet = () => _appSettings;

            return result;
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NameOrConnectionStringUnavailable()
        {
            using (GetDefaultShimsContext())
            {
                // Arrange 

                // Act
                var result = new SettingsService().NameOrConnectionString;
            }
        }

        [TestMethod]
        public void NameOrConnectionString()
        {
            using (GetDefaultShimsContext())
            {
                // Arrange 
                var connectionString = "database";
                _appSettings.Add("app:db", connectionString);

                // Act
                var result = new SettingsService().NameOrConnectionString;

                // Assert
                Assert.AreEqual("database", result);
            }
        }


        [TestMethod]
        public void SmsMessageUnavailable()
        {
            using (GetDefaultShimsContext())
            {
                // Arrange 

                // Act
                var result = new SettingsService().SmsMessage;

                // Assert
                Assert.AreEqual("Your verification code is: {0}", result);
            }
        }

        [TestMethod]
        public void SmsMessage()
        {
            using (GetDefaultShimsContext())
            {
                // Arrange 
                var smsmessage = "smsmessage";
                _appSettings.Add("app:smsmessage", smsmessage);

                // Act
                var result = new SettingsService().SmsMessage;

                // Assert
                Assert.AreEqual("smsmessage", result);
            }
        }


        [TestMethod]
        public void SmsWindowUnavailable()
        {
            using (GetDefaultShimsContext())
            {
                // Arrange 

                // Act
                var result = new SettingsService().SmsWindow;

                // Assert
                Assert.AreEqual(TimeSpan.FromMinutes(15), result);
            }
        }

        [TestMethod]
        public void SmsWindow()
        {
            using (GetDefaultShimsContext())
            {
                // Arrange 
                var smswindow = "4";
                _appSettings.Add("app:smswindow", smswindow);

                // Act
                var result = new SettingsService().SmsWindow;

                // Assert
                Assert.AreEqual(TimeSpan.FromMinutes(4), result);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IssuerUnavailable()
        {
            using (GetDefaultShimsContext())
            {
                // Arrange 

                // Act
                var result = new SettingsService().Issuer;
            }
        }

        [TestMethod]
        public void Issuer()
        {
            using (GetDefaultShimsContext())
            {
                // Arrange 
                var issuer = "issuer";
                _appSettings.Add("app:issuer", issuer);

                // Act
                var result = new SettingsService().Issuer;

                // Assert
                Assert.AreEqual("issuer", result);
            }
        }


        [TestMethod]
        public void PeriodUnavailable()
        {
            using (GetDefaultShimsContext())
            {
                // Arrange 

                // Act
                var result = new SettingsService().Period;

                // Assert
                Assert.AreEqual(30, result);
            }
        }

        [TestMethod]
        public void Period()
        {
            using (GetDefaultShimsContext())
            {
                // Arrange 
                var smswindow = "4";
                _appSettings.Add("qrcode:period", smswindow);

                // Act
                var result = new SettingsService().Period;

                // Assert
                Assert.AreEqual(4, result);
            }
        }


        [TestMethod]
        public void PastUnavailable()
        {
            using (GetDefaultShimsContext())
            {
                // Arrange 

                // Act
                var result = new SettingsService().Past;

                // Assert
                Assert.AreEqual(60, result);
            }
        }

        [TestMethod]
        public void Past()
        {
            using (GetDefaultShimsContext())
            {
                // Arrange 
                var past = "4";
                _appSettings.Add("qrcode:past", past);

                // Act
                var result = new SettingsService().Past;

                // Assert
                Assert.AreEqual(4, result);
            }
        }

        [TestMethod]
        public void FutureUnavailable()
        {
            using (GetDefaultShimsContext())
            {
                // Arrange 

                // Act
                var result = new SettingsService().Future;

                // Assert
                Assert.AreEqual(60, result);
            }
        }

        [TestMethod]
        public void Future()
        {
            using (GetDefaultShimsContext())
            {
                // Arrange 
                var future = "4";
                _appSettings.Add("qrcode:future", future);

                // Act
                var result = new SettingsService().Future;

                // Assert
                Assert.AreEqual(4, result);
            }
        }

        [TestMethod]
        public void SmsServiceUnavailable()
        {
            using (GetDefaultShimsContext())
            {
                // Arrange 

                // Act
                var result = new SettingsService().SmsService;

                // Assert
                Assert.AreEqual(MultiFactorAuthentication.Models.SmsService.Twilio, result);
            }
        }

        [TestMethod]
        public void SmsService()
        {
            using (GetDefaultShimsContext())
            {
                // Arrange 
                var smsService = "CM";
                _appSettings.Add("sms:service", smsService);

                // Act
                var result = new SettingsService().SmsService;

                // Assert
                Assert.AreEqual(MultiFactorAuthentication.Models.SmsService.CM, result);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationException))]
        public void CM_SmsService_Throws_When_Uri_IsInvalid()
        {
            using (GetDefaultShimsContext())
            {
                // Arrange 
                var url = "google.nl";
                _appSettings.Add("cm:url", url);

                // Act
                var result = new CMSettings().Url;
            }
        }

        [TestMethod]
        public void CM_SmsService_Has_Valid_URI()
        {
            using (GetDefaultShimsContext())
            {
                // Arrange 
                const string url = "https://google.nl";
                _appSettings.Add("cm:url", url);

                // Act
                var result = new CMSettings().Url;
                Assert.AreEqual(result.OriginalString, url);
            }
        }
    }
}
