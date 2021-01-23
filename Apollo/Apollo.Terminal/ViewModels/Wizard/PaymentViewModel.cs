using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Core.Dto;
using Apollo.Core.Interfaces;
using Apollo.Core.Types;
using Apollo.Payment.Domain;
using Apollo.Terminal.Common;
using Apollo.Terminal.Types.TransferObject;
using Apollo.Terminal.ViewModels.Base;
using Apollo.Util;

namespace Apollo.Terminal.ViewModels.Wizard
{
    public delegate bool ValidationCallback(out string errorKey);

    public class PaymentViewModel : BaseStepViewModel, INotifyDataErrorInfo
    {
        private readonly IServiceFactory _serviceFactory;

        #region Fields

        private string _creditCardNumber;
        private string _creditCardCvc;
        private string _creditCardName;
        private DateTime _creditCardExpirationDate;

        private readonly IDictionary<string, string> _errors = new Dictionary<string, string>();
        private readonly IDictionary<string, string> _errorKeys = new Dictionary<string, string>();
        private int _countSeats;
        private string _movieTitle;
        private decimal _totalPrice;

        private ReservationDto _reservation;
        private IEnumerable<SeatDto> _seats;
        private TicketDto _ticket;

        #endregion

        #region Properties

        public string CreditCardNumber
        {
            get => _creditCardNumber;
            set
            {
                ValidateCreditCardNumber(value);
                Set(ref _creditCardNumber, value);
            }
        }

        public string CreditCardCvc
        {
            get => _creditCardCvc;
            set
            {
                ValidateCreditCardCvc(value);
                Set(ref _creditCardCvc, value);
            }
        }

        public string CreditCardName
        {
            get => _creditCardName;
            set
            {
                ValidateCreditCardName(value);
                Set(ref _creditCardName, value);
            }
        }

        public DateTime CreditCardExpirationDate
        {
            get => _creditCardExpirationDate;
            set
            {
                ValidateCreditCardExpirationDate(value);
                Set(ref _creditCardExpirationDate, value);
            }
        }

        public int CountSeats
        {
            get => _countSeats;
            private set => Set(ref _countSeats, value);
        }

        public string MovieTitle
        {
            get => _movieTitle;
            private set => Set(ref _movieTitle, value);
        }

        public decimal TotalPrice
        {
            get => _totalPrice;
            private set => Set(ref _totalPrice, value);
        }

        #endregion

        #region Constructor

        public PaymentViewModel(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        #endregion

        #region Overridden Methods

        public override bool IsCancelAndBackEnabled() => true;

        public override async Task<ValidResultTransferObject<object>> IsValidAsync()
        {
            TriggerValidation();
            return await Task.FromResult(new ValidResultTransferObject<object>(!HasErrors));
        }

        public override void ShowValidationErrors(object value)
        {
            // nothing to do.
        }

        public override Task InitializeAsync(object argument)
        {
            if (!(argument is WizardSelectSeatToPayment transferObject))
            {
                throw new ArgumentException(nameof(argument));
            }

            _reservation = transferObject.Reservation;
            _seats = transferObject.Seats;

            return Task.CompletedTask;
        }

        public override void InitializeDone()
        {
            _ticket = null;
            ResetInputData();

            CountSeats = _seats.Count();
            MovieTitle = _reservation.Schedule.Movie.Title;
            TotalPrice = TicketPriceHelper.CalculatePrice(_reservation.Schedule.Price,
                _seats.Select(seat => seat.Row.Category.PriceFactor));

            LocalizationService.GetInstance().OnCultureChanged += ValidationUpdate;
        }

        public override void ResetDone()
        {
            _ticket = null;
            _reservation = null;
            _seats = null;

            LocalizationService.GetInstance().OnCultureChanged -= ValidationUpdate;
        }

        public override async Task<object> NextAsync()
        {
            if (_ticket == null)
            {
                try
                {
                    _ticket = await _serviceFactory.CreateCheckoutService().PayTicketAsync(_reservation.Id,
                        PaymentType.FhPay,
                        new ApolloCreditCard
                        {
                            OwnerName = CreditCardName,
                            CardNumber = CreditCardNumber,
                            Cvc = CreditCardCvc,
                            Expiration = CreditCardExpirationDate
                        }
                    );
                    return new WizardPaymentToCheckout(_reservation, _ticket, _seats, _totalPrice);
                }
                catch (Exception e)
                {
                    var message = e.Message.Replace(" ", "_");
                    ShowSnackBar(message);
                }
               
            }

            return null;
        }

        public override void NextDone()
        {
            ResetInputData();
            _reservation = null;
        }

        public override Task BackAsync()
        {
            _reservation = null;
            ResetInputData();
            return Task.CompletedTask;
        }

        #endregion

        #region Methods

        private void ResetInputData()
        {
            CreditCardName = "";
            CreditCardCvc = "";
            CreditCardNumber = "";
            CreditCardExpirationDate = DateTime.Now.AddDays(-1);
            ClearErrors();
        }

        private void ClearErrors()
        {
            var errorKeys = _errorKeys.Keys.ToList();
            _errorKeys.Clear();
            _errors.Clear();

            foreach (var key in errorKeys)
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(key));
            }

            TriggerPropertyChanged(HasErrors);
        }

        #endregion

        #region Validation

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => _errors.Any();

        public IEnumerable GetErrors(string propertyName)
        {
            return _errors.TryGetValue(propertyName, out string error) ? new List<string> {error} : null;
        }

        private void TriggerValidation()
        {
            ValidateCreditCardNumber(_creditCardNumber);
            ValidateCreditCardCvc(_creditCardCvc);
            ValidateCreditCardName(_creditCardName);
            ValidateCreditCardExpirationDate(_creditCardExpirationDate);
        }

        private void ValidationUpdate(CultureInfo culture)
        {
            _errors.Clear();
            foreach (var (key, _) in _errorKeys)
            {
                _errors[key] = LocalizationService.GetInstance()[_errorKeys[key]];
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(key));
            }
        }

        private void ValidateCreditCardNumber(string value)
        {
            GenericValidation(nameof(CreditCardNumber), (out string key) =>
            {
                var isValid = CreditCardValidatorHelper.IsValidCardNumber(value);
                key = isValid ? "" : "Invalid_CreditCardNumber";
                return isValid;
            });
        }

        private void ValidateCreditCardCvc(string value)
        {
            GenericValidation(nameof(CreditCardCvc), (out string key) =>
            {
                var isValid = CreditCardValidatorHelper.IsValidCvcNumber(value);
                key = isValid ? "" : "Invalid_CreditCardCvc";
                return isValid;
            });
        }

        private void ValidateCreditCardName(string value)
        {
            GenericValidation(nameof(CreditCardName), (out string key) =>
            {
                var isValid = CreditCardValidatorHelper.IsValidName(value);
                key = isValid ? "" : "Invalid_CreditCardName";
                return isValid;
            });
        }

        private void ValidateCreditCardExpirationDate(DateTime value)
        {
            GenericValidation(nameof(CreditCardExpirationDate), (out string key) =>
            {
                var isValid = CreditCardValidatorHelper.IsValidExpirationDate(value);
                key = isValid ? "" : "Invalid_CreditCardDate";
                return isValid;
            });
        }

        private void GenericValidation(string propertyName, ValidationCallback validationCallback)
        {
            _errors.TryGetValue(propertyName, out var oldValue);

            if (!validationCallback(out var errorKey))
            {
                _errorKeys[propertyName] = errorKey;
                _errors[propertyName] = LocalizationService.GetInstance()[_errorKeys[propertyName]];
            }
            else
            {
                _errorKeys.Remove(propertyName);
                _errors.Remove(propertyName);
            }

            _errors.TryGetValue(propertyName, out var newValue);
            if (!string.Equals(oldValue, newValue))
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}