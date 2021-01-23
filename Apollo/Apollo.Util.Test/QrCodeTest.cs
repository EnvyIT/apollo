using System;
using System.Drawing;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace Apollo.Util.Test
{
    public class QrCodeTest
    {
        class QrCodeMockData : IEquatable<QrCodeMockData>
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime Date { get; set; }
            public decimal Price { get; set; }

            public bool Equals(QrCodeMockData other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Id == other.Id && Name == other.Name && Date.Equals(other.Date) && Price == other.Price;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((QrCodeMockData) obj);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Id, Name, Date, Price);
            }
        }

        [Test]
        public void Test_GetQrCode_ContentNull_ShouldThrow_Exception()
        {
            Action callback = () => QrCodeHelper.GetQrCode<QrCodeMockData>(null);
            callback.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Test_GetQrCode_ContentEmpty_ShouldThrow_Exception()
        {
            Action callback = () => QrCodeHelper.GetQrCode("");
            callback.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Test_GetQrCode_ContentSingleText_Should_GenerateQrCode()
        {
            const string content = "apollo";

            var result = QrCodeHelper.GetQrCode(content);
            var deserializedResult = GetQrCodeContent<string>(result);

            deserializedResult.Should().Be(content);
        }

        [Test]
        public void Test_GetQrCode_ContentMultipleText_Should_GenerateQrCode()
        {
            var content = new QrCodeMockData
            {
                Id = 123456,
                Name = "Apollo",
                Date = DateTime.UtcNow.AddDays(15),
                Price = new decimal(25.65)
            };

            var result = QrCodeHelper.GetQrCode(content);
            var deserializedResult = GetQrCodeContent<QrCodeMockData>(result);

            deserializedResult.Id.Should().Be(content.Id);
            deserializedResult.Name.Should().Be(content.Name);
            deserializedResult.Date.Should().Be(content.Date);
            deserializedResult.Price.Should().Be(content.Price);
        }

        private static T GetQrCodeContent<T>(byte[] data)
        {
            var reader = new QRCodeReader();

            using var bitmap = new Bitmap(new MemoryStream(data));
            var binary = new BinaryBitmap(
                new HybridBinarizer(new RGBLuminanceSource(data, bitmap.Width, bitmap.Height))
            );

            return JsonMapper.Map<T>(reader.decode(binary).Text);
        }
    }
}