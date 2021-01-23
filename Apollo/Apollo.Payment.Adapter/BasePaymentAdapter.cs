using System;
using System.Threading.Tasks;
using Apollo.Payment.Adapter.Types;
using Apollo.Payment.Domain;
using Apollo.Util.Logger;

namespace Apollo.Payment.Adapter
{
    public abstract class BasePaymentAdapter<TOriginalPayment, TPayment> : IPaymentApi<TPayment>
        where TPayment : IPaymentMethod
    {
        private static readonly IApolloLogger<BasePaymentAdapter<TOriginalPayment, TPayment>> Logger = LoggerFactory.CreateLogger<BasePaymentAdapter<TOriginalPayment, TPayment>>();

        public async Task TransactionAsync(decimal amount, string description, TPayment paymentData)
        {
            try
            {
                Logger.Here().Info("Start payment transaction");
                await CallTransaction(amount, description, ConvertPayment(paymentData));
            }
            catch (PaymentException exception)
            {
                Logger.Here().Error(exception, "{Error} during payment transaction", nameof(exception.Error));
                throw;
            }
            catch (Exception exception)
            {
                Logger.Here().Error(exception, "Unknown error during payment transaction");
                throw new PaymentException(PaymentError.Unknown, "Unknown error", exception);
            }
            Logger.Here().Info("Finished payment transaction");
        }

        protected abstract Task CallTransaction(decimal amount, string description, TOriginalPayment data);

        protected abstract TOriginalPayment ConvertPayment(TPayment creditCard);
    }
}