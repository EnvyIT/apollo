using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Apollo.Core.Dto;
using Apollo.Core.Interfaces;
using Apollo.Terminal.Commands;
using Apollo.Terminal.Interfaces;
using Apollo.Terminal.Types;
using Apollo.Terminal.Types.TransferObject;
using Apollo.Terminal.ViewModels.Base;
using Apollo.Terminal.ViewModels.Wizard;
using Apollo.Util;

namespace Apollo.Terminal.ViewModels
{
    public class WizardViewModel : BasePageViewModel
    {
        #region Constants

        private const StepViewType FirstStep = StepViewType.SelectSeat;

        #endregion

        #region Fields

        private ScheduleDto _currentSchedule;
        private IList<ScheduleDto> _schedules;

        private readonly IDictionary<StepViewType, IStepViewModel> _viewModels;
        private StepViewType _currentViewType;

        private bool _isLoading;
        private bool _backInProgress;

        #endregion

        #region Properties

        private StepViewType CurrentStepViewType
        {
            set
            {
                Set(ref _currentViewType, value);
                TriggerPropertyChanged(this, nameof(CurrentViewModel));
                TriggerPropertyChanged(this, nameof(CurrentStep));
            }
        }

        public IStepViewModel CurrentViewModel => _viewModels[_currentViewType];

        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                Set(ref _isLoading, value);
                TriggerPropertyChanged(this, nameof(IsBackEnabled));
            }
        }

        public int CurrentStep => ConvertStepViewStateToIndex(_currentViewType);

        public bool IsCancelEnabled => CurrentViewModel.IsCancelAndBackEnabled();

        public bool IsBackEnabled => CurrentViewModel.IsCancelAndBackEnabled() && !IsLoading;

        #endregion

        #region Commands

        public ICommand StepNextCommand { get; }
        public ICommand StepCancelCommand { get; }
        public ICommand StepBackCommand { get; }

        #endregion

        #region Constructor

        public WizardViewModel(IServiceFactory serviceFactory)
        {
            _viewModels = new Dictionary<StepViewType, IStepViewModel>
            {
                {StepViewType.SelectSeat, new SelectSeatViewModel(serviceFactory)},
                {StepViewType.Payment, new PaymentViewModel(serviceFactory)},
                {StepViewType.Checkout, new CheckoutViewModel(serviceFactory)}
            };
            _currentViewType = FirstStep;

            StepNextCommand = new AsyncDelegateCommand<object>(NextValidationExecution);
            StepBackCommand = new AsyncDelegateCommand<object>(BackExecution);
            StepCancelCommand = new AsyncDelegateCommand<object>(CancelExecution);
        }

        #endregion

        #region Overridden Methods

        public override async Task InitializeAsync(object argument)
        {
            if (!(argument is MovieDetailToWizard transferObject))
            {
                throw new ArgumentException("Invalid data given!", nameof(argument));
            }

            _currentSchedule = transferObject.SelectedSchedule;
            _schedules = transferObject.Schedules;
            await CurrentViewModel.InitializeAsync(_currentSchedule);
        }

        public override void InitializeDone()
        {
            CurrentViewModel.InitializeDone();
            IsLoading = false;
            _backInProgress = false;
        }

        public override async Task ResetAsync()
        {
            if (!_backInProgress)
            {
                await CancelExecution(null);
            }
        }

        public override void ResetDone()
        {
            foreach (var viewModel in _viewModels)
            {
                viewModel.Value.ResetDone();
            }

            _currentViewType = FirstStep;
        }

        #endregion

        #region Command Execution Methods

        private async Task NextValidationExecution(object argument)
        {
            var taskHelper = new TaskHelper<ValidResultTransferObject<object>>();
            taskHelper.OnSuccess += result =>
            {
                if (result.IsValid)
                {
                    NextExecution();
                }
                else
                {
                    IsLoading = false;
                    CurrentViewModel.ShowValidationErrors(result.Value);
                }
            };
            taskHelper.OnFail += StopLoadingAndShowError;
            var task = taskHelper.Run(async () => await CurrentViewModel.IsValidAsync());

            IsLoading = true;
            await task;
        }

        private async void NextExecution()
        {
            if (!HasNextStep(out var nextStep))
            {
                EndWizard(() => RequestNewPage(PageViewType.MovieList));
                return;
            }

            var taskHelper = new TaskHelper<bool>();
            taskHelper.OnSuccess += result =>
            {
                if (!result)
                {
                    return;
                }
                CurrentViewModel.NextDone();
                CurrentStepViewType = nextStep;
                CurrentViewModel.InitializeDone();

                TriggerPropertyChanged(this, nameof(IsCancelEnabled));
                TriggerPropertyChanged(this, nameof(IsBackEnabled));
            };
            taskHelper.OnComplete += () => IsLoading = false;
            taskHelper.OnFail += StopLoadingAndShowError;

            await taskHelper.Run(async () =>
            { 
                var result = await CurrentViewModel.NextAsync();
                if (result == null)
                {
                    return false;
                }
                await _viewModels[nextStep].InitializeAsync(result);
                return true;
            });
        }

        private async Task BackExecution(object argument)
        {
            if (_currentViewType == StepViewType.Checkout)
            {
                return;
            }

            _backInProgress = true;

            var taskHelper = new TaskHelper();
            taskHelper.OnSuccess += () =>
            {
                if (HasPreviousStep(out var previousStep))
                {
                    CurrentStepViewType = previousStep;
                }
                else
                {
                    BackToMovieDetails();
                }
            };
            taskHelper.OnFail += ShowSnackBar;
            taskHelper.OnComplete += () =>
            {
                _backInProgress = false;
                IsLoading = false;
            };
            var task = taskHelper.Run(async () => await CurrentViewModel.BackAsync());

            IsLoading = true;
            await task;
        }

        private async Task CancelExecution(object argument)
        {
            if (_currentViewType == StepViewType.Checkout)
            {
                return;
            }

            var taskHelper = new TaskHelper();
            taskHelper.OnSuccess += BackToMovieDetails;
            taskHelper.OnFail += ShowSnackBar;
            taskHelper.OnComplete += () => IsLoading = false;
            var task = taskHelper.Run(async () =>
            {
                var currentIndex = CurrentStep;
                foreach (var (key, viewModel) in _viewModels.Reverse())
                {
                    if (ConvertStepViewStateToIndex(key) <= currentIndex)
                    {
                        await viewModel.BackAsync();
                    }
                }
            });

            IsLoading = true;
            await task;
        }

        #endregion

        #region Methods

        private void BackToMovieDetails()
        {
            EndWizard(() => RequestNewPage(PageViewType.MovieDetail, _schedules));
        }

        private void EndWizard(Action pageRequest)
        {
            foreach (var (_, viewModel) in _viewModels)
            {
                viewModel.ResetDone();
            }

            pageRequest?.Invoke();
        }
        
        private void StopLoadingAndShowError(Exception exception)
        {
            IsLoading = false;
            ShowSnackBar(exception);
        }

        private bool HasNextStep(out StepViewType nextStep)
        {
            switch (_currentViewType)
            {
                case StepViewType.SelectSeat:
                    nextStep = StepViewType.Payment;
                    return true;
                case StepViewType.Payment:
                    nextStep = StepViewType.Checkout;
                    return true;
                case StepViewType.Checkout:
                    nextStep = 0;
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool HasPreviousStep(out StepViewType previousStep)
        {
            switch (_currentViewType)
            {
                case StepViewType.SelectSeat:
                    previousStep = 0;
                    return false;
                case StepViewType.Payment:
                    previousStep = StepViewType.SelectSeat;
                    return true;
                case StepViewType.Checkout:
                    previousStep = StepViewType.Payment;
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static int ConvertStepViewStateToIndex(StepViewType state)
        {
            return state switch
            {
                StepViewType.SelectSeat => 1,
                StepViewType.Payment => 2,
                StepViewType.Checkout => 3,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        #endregion
    }
}