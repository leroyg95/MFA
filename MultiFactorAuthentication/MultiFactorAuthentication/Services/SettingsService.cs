#region

using System;
using System.Configuration;
using MultiFactorAuthentication.Models;
using MultiFactorAuthentication.Services.Settings;

#endregion

namespace MultiFactorAuthentication.Services
{
    /// <summary>
    ///     A SettingService
    /// </summary>
    public class SettingsService
    {
        private CMSettings _cmSettings;

        private int _future;

        private string _issuer;

        private int _maxResendSmsCode;

        private int _maxFailedHotpAttemptCount;

        private int _maxFailedPhoneAttemptCount;

        private int _maxFailedSmsAttemptCount;

        private int _maxFailedTotpAttemptCount;

        private MessageBirdSettings _messageBirdSettings;

        private string _nameOrConnectionString;

        private int _past;

        private int _period;

        private string _smsMessage;

        private SmsService? _smsService;

        private TimeSpan _smsWindow;

        private SpryngSettings _spryngSettings;

        private TwilioSettings _twilioSettings;

        static SettingsService() => Instance = new SettingsService();

        /// <summary>
        ///     Settings for CM
        /// </summary>
        public CMSettings CM => _cmSettings ?? (_cmSettings = new CMSettings());

        /// <summary>
        ///     A number of seconds in the future that the code will be considered valid (used in case of clock skew). Default is
        ///     60 seconds.
        /// </summary>
        public int Future
        {
            get
            {
                if (_future == default(int))
                    int.TryParse(ConfigurationManager.AppSettings["qrcode:future"], out _future);
                if (_future == default(int))
                    _future = 60;
                return _future;
            }
        }

        /// <summary>
        ///     A standard singleton for the setting service
        ///     Use it for performance.
        /// </summary>
        public static SettingsService Instance { get; }

        /// <summary>
        ///     The issuer of the multifactor application, a name that will be used with TOTP.
        /// </summary>
        public string Issuer
        {
            get
            {
                if (_issuer == null)
                    _issuer = ConfigurationManager.AppSettings["app:issuer"];
                if (string.IsNullOrWhiteSpace(_issuer))
                    throw new ArgumentNullException("Please set the issuer in the AppSettings");
                return _issuer;
            }
        }

        /// <summary>
        ///     The max number of entering the wrong code for Hotp
        /// </summary>
        public int MaxFailedHotpAttemptCount
        {
            get
            {
                if (_maxFailedHotpAttemptCount == default(int))
                    int.TryParse(ConfigurationManager.AppSettings["maxFailedHotpAttemptCount"], out _maxFailedHotpAttemptCount);
                if (_maxFailedHotpAttemptCount == default(int))
                    _maxFailedHotpAttemptCount = 5;
                return _maxFailedHotpAttemptCount;
            }
        }

        /// <summary>
        ///     The max number of entering the wrong phonenumber
        /// </summary>
        public int MaxFailedPhoneAttemptCount
        {
            get
            {
                if (_maxFailedPhoneAttemptCount == default(int))
                    int.TryParse(ConfigurationManager.AppSettings["maxFailedPhoneAttemptCount"], out _maxFailedPhoneAttemptCount);
                if (_maxFailedPhoneAttemptCount == default(int))
                    _maxFailedPhoneAttemptCount = 5;
                return _maxFailedPhoneAttemptCount;
            }
        }

        /// <summary>
        ///     The max number of resending sms codes
        /// </summary>
        public int MaxResendSmsCode
        {
            get
            {
                if (_maxResendSmsCode == default(int))
                    int.TryParse(ConfigurationManager.AppSettings["maxResendSmsCode"], out _maxResendSmsCode);
                if (_maxResendSmsCode == default(int))
                    _maxResendSmsCode = 5;
                return _maxResendSmsCode;
            }
        }

        /// <summary>
        ///     The max number of entering the wrong code for Sms
        /// </summary>
        public int MaxFailedSmsAttemptCount
        {
            get
            {
                if (_maxFailedSmsAttemptCount == default(int))
                    int.TryParse(ConfigurationManager.AppSettings["maxFailedSmsAttemptCount"], out _maxFailedSmsAttemptCount);
                if (_maxFailedSmsAttemptCount == default(int))
                    _maxFailedSmsAttemptCount = 5;
                return _maxFailedSmsAttemptCount;
            }
        }

        /// <summary>
        ///     The max number of entering the wrong code for Totp
        /// </summary>
        public int MaxFailedTotpAttemptCount
        {
            get
            {
                if (_maxFailedTotpAttemptCount == default(int))
                    int.TryParse(ConfigurationManager.AppSettings["maxFailedTotpAttemptCount"], out _maxFailedTotpAttemptCount);
                if (_maxFailedTotpAttemptCount == default(int))
                    _maxFailedTotpAttemptCount = 5;
                return _maxFailedTotpAttemptCount;
            }
        }

        /// <summary>
        ///     Settings for MessageBird
        /// </summary>
        public MessageBirdSettings MessageBird =>
            _messageBirdSettings ?? (_messageBirdSettings = new MessageBirdSettings());

        /// <summary>
        ///     Name or Connection string, to be used for database connections.
        /// </summary>
        public string NameOrConnectionString
        {
            get
            {
                if (_nameOrConnectionString == null)
                    _nameOrConnectionString = ConfigurationManager.AppSettings["app:db"];
                if (string.IsNullOrWhiteSpace(_nameOrConnectionString))
                    throw new ArgumentNullException("Please set the db in de AppSettings");
                return _nameOrConnectionString;
            }
        }

        /// <summary>
        ///     A number of seconds in the past that the code will be considered valid (used in case of clock skew). Default is 30
        ///     seconds.
        /// </summary>
        public int Past
        {
            get
            {
                if (_past == default(int))
                    int.TryParse(ConfigurationManager.AppSettings["qrcode:past"], out _past);
                if (_past == default(int))
                    _past = 60;
                return _past;
            }
        }

        /// <summary>
        ///     A period that a TOTP code will be valid for in seconds, default is 30 seconds.
        /// </summary>
        public int Period
        {
            get
            {
                if (_period == default(int))
                    int.TryParse(ConfigurationManager.AppSettings["qrcode:period"], out _period);
                if (_period == default(int))
                    _period = 30;
                return _period;
            }
        }


        /// <summary>
        ///     Sms message, the message to be sent to an user for requesting a securitycode
        /// </summary>
        public string SmsMessage
        {
            get
            {
                if (_smsMessage == null)
                    _smsMessage = ConfigurationManager.AppSettings["app:smsmessage"];
                if (string.IsNullOrWhiteSpace(_smsMessage))
                    _smsMessage = "Your verification code is: {0}";
                return _smsMessage;
            }
        }

        /// <summary>
        ///     The SmsService to be used. Default is Twilio.
        /// </summary>
        public SmsService SmsService
        {
            get
            {
                if (!_smsService.HasValue)
                    if (Enum.TryParse(ConfigurationManager.AppSettings["sms:service"], out SmsService tmp))
                        _smsService = tmp;
                if (!_smsService.HasValue)
                    _smsService = SmsService.Twilio;
                return _smsService.Value;
            }
        }

        /// <summary>
        ///     Sms window, the window in which a response may be given by the user. In minutes.
        /// </summary>
        public TimeSpan SmsWindow
        {
            get
            {
                if (_smsWindow == default(TimeSpan))
                {
                    int.TryParse(ConfigurationManager.AppSettings["app:smswindow"], out var minutes);
                    _smsWindow = TimeSpan.FromMinutes(minutes);
                }
                if (_smsWindow == default(TimeSpan))
                    _smsWindow = TimeSpan.FromMinutes(15);
                return _smsWindow;
            }
        }

        /// <summary>
        ///     Settings for Spryng
        /// </summary>
        public SpryngSettings Spryng => _spryngSettings ?? (_spryngSettings = new SpryngSettings());

        /// <summary>
        ///     Settings for Twilio
        /// </summary>
        public TwilioSettings Twilio => _twilioSettings ?? (_twilioSettings = new TwilioSettings());
    }
}