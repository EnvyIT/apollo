using System;
using System.Configuration;
using Apollo.Core.Implementation;
using Apollo.Core.Interfaces;
using Apollo.Core.Types;
using Apollo.Payment;
using Apollo.Payment.Domain;
using Apollo.Util;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Apollo.Core.Test
{
    public class PaymentFactoryTest
    {
        private IPaymentFactory _paymentFactory;

        [SetUp]
        public void Setup()
        {
            _paymentFactory = new PaymentFactory();
        }
        
        [Test]
        public void Test_CreateInstance_SingleCall_Should_Return_NewInstance()
        {
            var result = _paymentFactory.CreatePayment(PaymentType.FhPay);
            result.Should().NotBeNull();
        }

        [Test]
        public void Test_CreateInstance_MultipleCalls_Should_Return_SameInstance()
        {
            var result1 = _paymentFactory.CreatePayment(PaymentType.FhPay);
            var result2 = _paymentFactory.CreatePayment(PaymentType.FhPay);
            result1.Should().Be(result2);
        }

        [Test]
        public void Test_CreateInstanceWithWrongSettings_ShouldThrowConfigurationException()
        {
            _paymentFactory.ConfigurationRoot = new ConfigurationBuilder()
                .AddJsonFile("wrong.settings.json")
                .Build();
            Func<IPaymentApi<IPaymentMethod>> createPayment = () =>  _paymentFactory.CreatePayment(PaymentType.FhPay);
            createPayment.Should().ThrowExactly<ConfigurationErrorsException>();
        }

        [Test]
        public void Test_CreatePaymentWithWrongEnum_ShouldThrowConfigurationException()
        {
            const  PaymentType paymentType = (PaymentType) 99;
            Func<IPaymentApi<IPaymentMethod>> createPayment = () => _paymentFactory.CreatePayment(paymentType);
            createPayment.Should().ThrowExactly<ConfigurationErrorsException>();
        }
    }
}