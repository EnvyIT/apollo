using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Apollo.Core.Interfaces;
using Apollo.Core.Types;
using Apollo.Payment;
using Apollo.Payment.Adapter;
using Apollo.Payment.Adapter.Wrapper;
using Apollo.Payment.Domain;
using Apollo.Util;
using FHPay;
using Microsoft.Extensions.Configuration;

namespace Apollo.Core.Implementation
{
    public class PaymentFactory : IPaymentFactory
    {
        private static readonly object LockObject = new object();

        private readonly Dictionary<PaymentType, IPaymentApi<IPaymentMethod>> _paymentApis =
            new Dictionary<PaymentType, IPaymentApi<IPaymentMethod>>();

        public IConfigurationRoot ConfigurationRoot { get; set; } = new ConfigurationBuilder()
            .AddJsonFile("appsettings.core.json")
            .Build();

        public IPaymentApi<IPaymentMethod> CreatePayment(PaymentType paymentType)
        {
            return GetSingleton(paymentType);
        }

        private IPaymentApi<IPaymentMethod> GetSingleton(PaymentType paymentType)
        {
            lock (LockObject)
            {
                if (_paymentApis.ContainsKey(paymentType))
                {
                    return _paymentApis[paymentType];
                }

                IPaymentApi<IPaymentMethod> api;
                switch (paymentType)
                {
                    case PaymentType.FhPay:
                        api = GetFhPayment();
                        break;
                    default:
                        throw new ConfigurationErrorsException("Invalid payment method configured!");
                }

                _paymentApis.Add(paymentType, api);
                return api;
            }
        }

        private IPaymentApi<IPaymentMethod> GetFhPayment()
        {
            ConfigurationHelper.ConfigurationRoot = ConfigurationRoot;
            try
            {
                var appSettings = ConfigurationHelper.GetValues("Fh_Pay_Api_Key");
                return new FhPayAdapter(new FhPayWrapper(new PaymentApi(appSettings[0])));
            }
            catch (KeyNotFoundException ex)
            {
                throw new ConfigurationErrorsException("No FH payment API key configured", ex);
            }
        }
    }
}