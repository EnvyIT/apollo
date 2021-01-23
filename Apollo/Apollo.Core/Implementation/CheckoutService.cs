using System.Linq;
using System.Threading.Tasks;
using Apollo.Core.Dto;
using Apollo.Core.Interfaces;
using Apollo.Core.Types;
using Apollo.Payment.Domain;
using Apollo.UnitOfWork.Interfaces;
using Apollo.Util;
using Apollo.Util.Logger;

namespace Apollo.Core.Implementation
{
    public class CheckoutService : ICheckoutService
    {
        private static readonly IApolloLogger<CheckoutService> _logger = LoggerFactory.CreateLogger<CheckoutService>();

        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentFactory _paymentFactory;

        public CheckoutService(IUnitOfWork unitOfWork, IPaymentFactory paymentFactory)
        {
            _unitOfWork = unitOfWork;
            _paymentFactory = paymentFactory;
        }

        public async Task<TicketDto> PayTicketAsync(long reservationId, PaymentType paymentType,
            IPaymentMethod paymentMethod)
        {
            _logger.Debug("Perform ticket payment for reservation {ReservationId}", reservationId);

            var api = _paymentFactory.CreatePayment(paymentType);

            var reservation = await _unitOfWork.RepositoryTicket.GetReservationByIdAsync(reservationId);
            var schedule = await _unitOfWork.RepositorySchedule.GetScheduleByIdAsync(reservation.ScheduleId);
            var seats = await _unitOfWork.RepositoryTicket.GetSeatsWithRowAndCategoryByIdAsync(reservationId);

            var totalPrice = TicketPriceHelper.CalculatePrice(schedule.Price, 
                seats.Select(seat => seat.Row.Category.PriceFactor));
            _logger.Debug("Book {Price}€ for reservation {ReservationId}", totalPrice, reservationId);

            try
            {
                long ticketId = 0;
                await _unitOfWork.Transaction().PerformBlock(async () =>
                {
                    ticketId = await _unitOfWork.RepositoryTicket.AddTicketAsync(reservation);
                    await api.TransactionAsync(totalPrice, $"Apollo thanks for your purchase ({ticketId}).",
                        paymentMethod);
                }).Commit();

                return Mapper.Map(await _unitOfWork.RepositoryTicket.GetTicketByIdAsync(ticketId));
            }
            catch (System.Exception exception)
            {
                _logger.Error(exception, "Book {Price}€ for reservation {ReservationId} failed!",
                    totalPrice, reservationId);
                throw;
            }
        }
    }
}