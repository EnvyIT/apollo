using System;
using QRCoder;

namespace Apollo.Util
{
    public static class QrCodeHelper
    {
        public static byte[] GetQrCode<T>(T content) where T : IEquatable<T>
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (Equals(content, default(T)) || typeof(T) == typeof(string) && string.IsNullOrEmpty(content as string))
            {
                throw new ArgumentException("Given content is empty", nameof(content));
            }

            var qrGenerator = new QRCodeGenerator();
            var qrContent = JsonMapper.Map(content);

            var qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new BitmapByteQRCode(qrCodeData);

            return qrCode.GetGraphic(20);
        }
    }
}