using Apollo.Core.Dto;
using Apollo.Core.Types;
using Apollo.Payment.Domain;
using System.Threading.Tasks;

namespace Apollo.Core.Interfaces
{
    public interface ICheckoutService
    {
        Task<TicketDto> PayTicketAsync(long reservationId, PaymentType paymentType, IPaymentMethod paymentMethod);
    }
}
