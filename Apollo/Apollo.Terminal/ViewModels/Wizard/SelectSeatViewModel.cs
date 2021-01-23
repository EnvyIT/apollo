using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Apollo.Core.Dto;
using Apollo.Core.External;
using Apollo.Core.Interfaces;
using Apollo.Core.Validation;
using Apollo.Terminal.Commands;
using Apollo.Terminal.Types.TransferObject;
using Apollo.Terminal.ViewModels.Base;
using Apollo.Util;

namespace Apollo.Terminal.ViewModels.Wizard
{
    public class SelectSeatViewModel : BaseStepViewModel
    {

        #region Member

        private const string TerminalUser = "TerminalUser";

        private readonly string _uuidTerminalUser;
        private readonly IServiceFactory _serviceFactory;
        private int _rows;
        private int _columns;
        private ObservableCollection<SeatDto> _seatLayout;
        private IList<SeatDto> _selectedSeats;
        private ScheduleDto _selectedSchedule;

        #endregion

        #region Properties
        public ObservableCollection<SeatDto> SeatLayout
        {
            get => _seatLayout;
            set => Set(ref _seatLayout, value);
        }

        public IList<SeatDto> SelectedSeats
        {
            get => _selectedSeats;
            set => Set(ref _selectedSeats, value);
        }

        public ScheduleDto SelectedSchedule
        {
            get => _selectedSchedule;
            set => Set(ref _selectedSchedule, value);
        }

        public int Rows
        {
            get => _rows;
            set => Set(ref _rows, value);
        }

        public int Columns
        {
            get => _columns;
            set => Set(ref _columns, value);
        }

        public int SeatCount => _selectedSeats.Count;

        public ReservationDto Reservation { get; private set; }

        public UserDto User { get; private set; }

        #endregion

        #region Commands
        public ICommand SelectSeat { get; set; }
        #endregion

        #region Contructors
        public SelectSeatViewModel(IServiceFactory serviceFactory)
        {
            _uuidTerminalUser = ConfigurationHelper.GetValues(TerminalUser)[0];

            _serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
            _serviceFactory.CreateReservationService().AddRule(new CoronaRule());
            SelectedSeats = new ObservableCollection<SeatDto>();
            SelectSeat = new DelegateCommand<SeatDto>(OnSeatSelect);
        }

        #endregion

        #region Overridden Methods

        public override bool IsCancelAndBackEnabled() => true;

        public override Task<ValidResultTransferObject<object>> IsValidAsync()
        {
            return Task.FromResult(new ValidResultTransferObject<object>(SelectedSeats.Any()));
        }

        public override void ShowValidationErrors(object value)
        {
            // nothing to do.
        }
        
        public override async Task InitializeAsync(object argument)
        {
            if (!(argument is ScheduleDto scheduleDto))
            {
                throw new ArgumentNullException(nameof(scheduleDto));
            }

            SelectedSchedule = scheduleDto;
            User = await _serviceFactory.CreateUserService().GetUserWithAddressByUuidAsync(_uuidTerminalUser);
            await InitLayout(scheduleDto);
        }

        public override async Task<object> NextAsync()
        {
            try
            {
                if (Reservation != null)
                {
                    await _serviceFactory.CreateReservationService().UpdateReservation(Reservation, SelectedSeats);
                }
                else
                {
                    Reservation = await _serviceFactory.CreateReservationService()
                        .AddReservationAsync(SelectedSeats, SelectedSchedule.Id, User.Id);
                    Reservation.Schedule = SelectedSchedule;
                    Reservation.User = User;
                }
                return new WizardSelectSeatToPayment(Reservation, SelectedSeats);
            }
            catch (SeatValidationException exception)
            {
               
                ShowSnackBar("Seat_Validation", exception);
            }
            catch (ArgumentNullException exception)
            {
                ShowSnackBar("Seat_Schedule_Null", exception);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                ShowSnackBar("Seat_Schedule_Begun", exception);
            }
            catch (Exception exception)
            {
                ShowSnackBar("Seat_Unexpected", exception);
            }

            return null;
        }

        public override void NextDone()
        {
            // nothing to do.
        }

        public override async Task BackAsync()
        {
            if (Reservation != null)
            {
                await _serviceFactory.CreateReservationService().DeleteReservationAsync(Reservation);
                SelectedSeats.Clear();
            }
        }

        public override void ResetDone()
        {
            Reservation = null;
            _selectedSchedule = null;

            SeatLayout.Clear();
            SelectedSeats.Clear();
        }

        #endregion

        #region Methods

        private async Task InitLayout(ScheduleDto scheduleDto)
        {
            var seatLayout = await _serviceFactory.CreateInfrastructureService().GetLayout(scheduleDto);
            var flatLayout = seatLayout.Cast<SeatDto>().Where(s => s != null).ToList();
            Rows = seatLayout.GetLength(0);
            Columns = seatLayout.GetLength(1);
            SeatLayout = new ObservableCollection<SeatDto>(flatLayout);
        }


        private void OnSeatSelect(SeatDto seatDto)
        {

            if ((seatDto.State == SeatState.Occupied && !SelectedSeats.Select(s => s.Id).Contains(seatDto.Id) || seatDto.State == SeatState.Locked))
            {
                ShowSnackBar("Seat_Occupied");
                return;
            }

            var layoutSeat = SeatLayout.SingleOrDefault(s => s.Id == seatDto.Id);
            if (SelectedSeats.Contains(seatDto))
            {
                RemoveSeatReservation(seatDto, layoutSeat);
            }
            else
            {
                if (SeatCount + 1 > User.Role.MaxReservations)
                {
                    ShowSnackBar("Seat_Max_Reservations");
                    return;
                }
                AddSeatReservation(seatDto, layoutSeat);
            }

            UpdateSeatLayout(seatDto);
        }

        private void UpdateSeatLayout(SeatDto seatDto)
        {
            SeatLayout.Remove(seatDto);
            SeatLayout.Add(seatDto);
        }

        private void AddSeatReservation(SeatDto seatDto, SeatDto layoutSeat)
        {
            seatDto.State = SeatState.Occupied;
            layoutSeat.State = SeatState.Selected;
            AddSelectedSeat(seatDto);
        }

        private void RemoveSeatReservation(SeatDto seatDto, SeatDto layoutSeat)
        {
            seatDto.State = SeatState.Free;
            layoutSeat.State = SeatState.Free;
            RemoveSelectedSeat(seatDto);
        }

        public void AddSelectedSeat(SeatDto seatDto)
        {
            _selectedSeats.Add(seatDto);
            TriggerPropertyChanged(this, nameof(_selectedSeats));
            TriggerPropertyChanged(this, nameof(SeatCount));
        }

        private void RemoveSelectedSeat(SeatDto seatDto)
        {
            _selectedSeats.Remove(seatDto);
            TriggerPropertyChanged(this, nameof(_selectedSeats));
            TriggerPropertyChanged(this, nameof(SeatCount));
        }

        #endregion
    }
}
