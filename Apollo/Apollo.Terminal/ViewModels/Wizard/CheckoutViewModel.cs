using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Apollo.Core.Dto;
using Apollo.Core.Interfaces;
using Apollo.Terminal.Commands;
using Apollo.Terminal.Common;
using Apollo.Terminal.Types;
using Apollo.Terminal.Types.TransferObject;
using Apollo.Terminal.ViewModels.Base;
using Apollo.Util;
using static System.String;

namespace Apollo.Terminal.ViewModels.Wizard
{
    public class CheckoutViewModel : BaseStepViewModel
    {
        #region Fields

        private ReservationDto _reservation;
        private TicketDto _ticket;

        private IList<byte> _movieImage;
        private IList<byte> _qrCodeImage;
        private IEnumerable<SeatDto> _seats;
        private string _movieTitle;
        private string _seatsText;
        private decimal _totalPrice;

        #endregion

        #region Properties

        public IEnumerable<SeatDto> Seats
        {
            get => _seats;
            private set
            {
                Set(ref _seats, value);
                TriggerPropertyChanged(this, nameof(CountSeats));
            }
        }

        public IList<byte> MovieImage
        {
            get => _movieImage;
            private set => Set(ref _movieImage, value);
        }

        public IList<byte> QrCodeImage
        {
            get => _qrCodeImage;
            private set => Set(ref _qrCodeImage, value);
        }

        public string MovieTitle
        {
            get => _movieTitle;
            private set => Set(ref _movieTitle, value);
        }

        public string SeatsText
        {
            get => _seatsText;
            private set => Set(ref _seatsText, value);
        }

        public decimal TotalPrice
        {
            get => _totalPrice;
            private set => Set(ref _totalPrice, value);
        }

        public int CountSeats => Seats.Count();

        #endregion

        #region Commands

        public ICommand PrintCommand { get; }

        #endregion

        #region Constructor

        public CheckoutViewModel(IServiceFactory serviceFactory)
        {
            PrintCommand = new DelegateCommand<object>(PrintTicket);
        }

        #endregion

        #region Overridden Methods

        public override bool IsCancelAndBackEnabled() => false;

        public override Task<ValidResultTransferObject<object>> IsValidAsync()
        {
            return Task.FromResult(new ValidResultTransferObject<object>(true));
        }

        public override void ShowValidationErrors(object value)
        {
            // nothing to do.
        }

        public override Task<object> NextAsync()
        {
            return Task.FromResult((object) true);
        }

        public override Task InitializeAsync(object argument)
        {
            if (!(argument is WizardPaymentToCheckout transferObject))
            {
                throw new ArgumentException(nameof(argument));
            }

            _reservation = transferObject.Reservation;
            _ticket = transferObject.Ticket;
            _seats = transferObject.Seats;
            _totalPrice = transferObject.TotalPrice;

            return Task.CompletedTask;
        }

        public override void InitializeDone()
        {
            MovieTitle = _reservation.Schedule.Movie.Title;
            MovieImage = _reservation.Schedule.Movie.Image.ToList();
            QrCodeImage = QrCodeHelper.GetQrCode(new TicketData(_ticket.Id, _reservation.Id,
                _reservation.Schedule.CinemaHallId, _reservation.Schedule.MovieId, _reservation.Schedule.StartTime));
            SeatsText = GetSeatText();

            TriggerPropertyChanged(this, nameof(Seats));
            TriggerPropertyChanged(this, nameof(TotalPrice));

            LocalizationService.GetInstance().OnCultureChanged += OnLocalizationChanged;
        }

        public override void ResetDone()
        {
            _ticket = null;
            _reservation = null;

            LocalizationService.GetInstance().OnCultureChanged -= OnLocalizationChanged;
        }

        public override void NextDone()
        {
            TotalPrice = 0;
            SeatsText = "";
            MovieTitle = "";
        }

        public override Task BackAsync()
        {
            throw new InvalidOperationException("Back is not possible");
        }

        #endregion

        #region Methods

        private void PrintTicket(object element)
        {
            try
            {
                PrintHelper.PrintFrameworkElement((FrameworkElement) element, $"Apollo - Ticket {_ticket.Id}");
            }
            catch (Exception exception)
            {
                ShowSnackBar("Print_Error", exception);
            }
        }

        private void OnLocalizationChanged(CultureInfo cultureInfo)
        {
            SeatsText = GetSeatText();
        }

        private string GetSeatText()
        {
            var rowTranslation = LocalizationService.GetInstance()["Ticket_Row"];
            var seatTranslation = LocalizationService.GetInstance()["Ticket_Seat"];

            return Join("; ", _seats.GroupBy(seat => seat.RowId)
                .Select(group =>
                    {
                        var seats = group.OrderBy(seat => seat.Number).Select(seat => seat.Number.ToString());
                        return $"{rowTranslation} {group.Key}: {seatTranslation} {Join(",", seats)}";
                    }
                )
            );
        }

        #endregion
    }
}