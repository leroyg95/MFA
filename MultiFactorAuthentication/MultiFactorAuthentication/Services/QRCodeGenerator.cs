#region

using System.Drawing;
using ZXing;
using ZXing.Common;

#endregion

namespace MultiFactorAuthentication.Services
{
    /// <summary>
    ///     A QRCode Generator
    /// </summary>
    public static class QRCodeGenerator
    {
        /// <summary>
        ///     Generate a QRCode, based on an url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="heigth">The height of the resulting QRCode (default: 250)</param>
        /// <param name="width">The widht of the resulting QRCode (default: 250)</param>
        /// <param name="margin">The margin of the resulting QRCode (default: 0)</param>
        /// <returns>A bitmap containing the QRCode.</returns>
        public static Bitmap Generate(string url, int heigth = 250, int width = 250, int margin = 0)
        {
            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = heigth,
                    Width = width,
                    Margin = margin
                }
            };

            return barcodeWriter.Write(url);
        }
    }
}