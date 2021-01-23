using System;
using System.Threading.Tasks;
using Apollo.Payment.Adapter.Types;
using Apollo.Payment.Test.Mocks;
using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Payment.Test
{
    public class PaymentTest
    {
        private IPaymentApi<SimplePaymentMethod> _paymentApi;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _paymentApi = new SimplePaymentMock();
        }

        [Test]
        public async Task Test_Transaction_Success()
        {
            Func<Task> execution = async () =>
                await _paymentApi.TransactionAsync(1, "ticket", new SimplePaymentMethod {Id = "123456789"});
            await execution.Should().NotThrowAsync();
        }

        [Test]
        public async Task Test_Transaction_Failure()
        {
            Func<Task> execution = async () =>
                await _paymentApi.TransactionAsync(1, "ticket", new SimplePaymentMethod {Id = ""});
            await execution.Should().ThrowAsync<PaymentException>();
        }
    }
}