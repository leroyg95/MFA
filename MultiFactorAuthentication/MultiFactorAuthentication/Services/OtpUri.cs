namespace MultiFactorAuthentication.Services
{
    /// <summary>
    ///     A model used for creating QR Codes
    /// </summary>
    public class OtpUri
    {
        /// <summary>
        ///     The uri for the QRcode
        /// </summary>
        public string Uri { get; set; }
        /// <summary>
        ///     The secret that is used in the QR Code or that is used for manual use
        /// </summary>
        public string Secret { get; set; }
    }
}