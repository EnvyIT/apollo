using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Apollo.Core.Interfaces;
using Apollo.Terminal.Commands;
using Apollo.Terminal.Interfaces;
using Apollo.Terminal.Types;
using Apollo.Terminal.ViewModels.Base;
using Apollo.Util;

namespace Apollo.Terminal.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Constants

        private const PageViewType Default = PageViewType.Overlay;
        private const string ConfigInactiveTimeout = "Inactive_Timeout";
        private const string ConfigVirtualOnScreenKeyboardEnabled = "VirtualOnScreen_Keyboard_Enabled";

        #endregion

        #region Fields

        private readonly long _inactiveTimeout;

        private readonly IDictionary<PageViewType, IPageViewModel> _viewModels;
        private readonly DispatcherTimer _inactiveTimer;

        private readonly Subject<bool> _autoResetSubject;

        private bool _isLoading;
        private TextBox _focusedTextBox;

        private PageViewType _currentViewType;

        #endregion

        #region Properties

        public IViewModel CurrentPageViewModel => _viewModels[_currentViewType];

        public bool HeaderVisible => CurrentViewType != PageViewType.Overlay;

        public bool IsOnScreenKeyBoardVisible => _focusedTextBox != null;


        public bool IsLoading
        {
            get => _isLoading;
            private set => Set(ref _isLoading, value);
        }

        public PageViewType CurrentViewType
        {
            get => _currentViewType;
            set
            {
                Set(ref _currentViewType, value);
                TriggerPropertyChanged(this, nameof(CurrentPageViewModel));
                TriggerPropertyChanged(this, nameof(HeaderVisible));
            }
        }

        #endregion

        #region Commands

        public ICommand UpdateAutoResetCommand { get; }

        public ICommand KeyboardKeyChangedCommand { get; }

        public ICommand KeyboardSpaceCommand { get; }

        public ICommand KeyboardDeleteCommand { get; }

        public ICommand KeyboardTabulator { get; }

        #endregion

        #region Constructor

        public MainViewModel(IServiceFactory serviceFactory)
        {
            var config = ConfigurationHelper.GetValues(ConfigInactiveTimeout, ConfigVirtualOnScreenKeyboardEnabled);
            _inactiveTimeout = long.Parse(config[0]);

            _viewModels = new Dictionary<PageViewType, IPageViewModel>
            {
                {PageViewType.Overlay, new OverlayViewModel()},
                {PageViewType.MovieList, new MovieListViewModel(serviceFactory)},
                {PageViewType.MovieDetail, new MovieDetailViewModel(serviceFactory)},
                {PageViewType.Wizard, new WizardViewModel(serviceFactory)},
            };
            SetupPageRequestHandler();

            _autoResetSubject = new Subject<bool>();
            RegisterAutoReset();
            UpdateAutoResetCommand = new DelegateCommand<object>(UpdateAutoResetCommandExecution);

            _inactiveTimer = new DispatcherTimer();
            SetupInactiveTimer();

            CurrentViewType = Default;
            IsLoading = false;

            KeyboardKeyChangedCommand = new DelegateCommand<string>(OnKeyboardKeyChanged);
            KeyboardDeleteCommand = new DelegateCommand<object>(OnKeyboardDelete);
            KeyboardSpaceCommand = new DelegateCommand<object>(OnKeyboardSpace);
            KeyboardTabulator = new DelegateCommand<object>(OnKeyboardTabulator);

            if (bool.Parse(config[1]))
            {
                InitializeOnScreenKeyboard();
            }
        }

        #endregion

        #region Methods

        private void UpdateTextBoxCursor()
        {
            _focusedTextBox.CaretIndex = _focusedTextBox.Text.Length;
        }

        private void SetupPageRequestHandler()
        {
            foreach (var viewModel in _viewModels)
            {
                viewModel.Value.OnPageRequest += ChangePage;
            }
        }

        private void SetupInactiveTimer()
        {
            _inactiveTimer.Interval = TimeSpan.FromMilliseconds(_inactiveTimeout);
            _inactiveTimer.Tick += (sender, e) => ChangePage(PageViewType.Overlay, null);

            _inactiveTimer.IsEnabled = false;
        }

        private async void ChangePage(PageViewType viewType, object argument)
        {
            var taskHelper = new TaskHelper();
            taskHelper.OnSuccess += () => PageResetDone(viewType, argument);
            taskHelper.OnFail += exception =>
            {
                IsLoading = false;
                ShowSnackBar(exception);
            };
            var task = taskHelper.Run(async () => { await CurrentPageViewModel.ResetAsync(); });

            IsLoading = true;
            await task;
        }

        private async void PageResetDone(PageViewType viewType, object argument)
        {
            CurrentPageViewModel.ResetDone();

            _inactiveTimer.IsEnabled = viewType != PageViewType.Overlay;
            CurrentViewType = viewType;

            var taskHelper = new TaskHelper();
            taskHelper.OnSuccess += () => CurrentPageViewModel.InitializeDone();
            taskHelper.OnFail += ShowSnackBar;
            taskHelper.OnComplete += () => IsLoading = false;

            await taskHelper.Run(async () => { await CurrentPageViewModel.InitializeAsync(argument); });
        }

        private void RegisterAutoReset()
        {
            _autoResetSubject
                .Throttle(TimeSpan.FromMilliseconds(100))
                .Where(value => value)
                .Subscribe(_ => RefreshAutoReset());
        }

        private void UpdateAutoResetCommandExecution(object o)
        {
            _autoResetSubject.OnNext(true);
        }

        private void RefreshAutoReset()
        {
            _inactiveTimer.IsEnabled = false;
            _inactiveTimer.IsEnabled = true;
        }

        #endregion

        #region On Screen Keyboard

        private void InitializeOnScreenKeyboard()
        {
            EventManager.RegisterClassHandler(
                typeof(TextBox),
                UIElement.GotFocusEvent,
                new RoutedEventHandler((s, e) =>
                {
                    _focusedTextBox = (TextBox) s;
                    TriggerPropertyChanged(this, nameof(IsOnScreenKeyBoardVisible));
                }),
                true);

            EventManager.RegisterClassHandler(
                typeof(TextBox),
                UIElement.LostFocusEvent,
                new RoutedEventHandler((s, e) =>
                {
                    _focusedTextBox = null;
                    TriggerPropertyChanged(this, nameof(IsOnScreenKeyBoardVisible));
                }),
                true);
        }

        private void OnKeyboardKeyChanged(string key)
        {
            if (_focusedTextBox == null)
            {
                return;
            }

            _focusedTextBox.Text += key;
            UpdateTextBoxCursor();
        }

        private void OnKeyboardDelete(object obj)
        {
            if (_focusedTextBox?.Text == null || _focusedTextBox.Text.Length <= 0)
            {
                return;
            }

            _focusedTextBox.Text = _focusedTextBox.Text.Remove(_focusedTextBox.Text.Length - 1);
            UpdateTextBoxCursor();
        }

        private void OnKeyboardSpace(object obj)
        {
            if (_focusedTextBox == null)
            {
                return;
            }

            _focusedTextBox.Text += " ";
            UpdateTextBoxCursor();
        }

        private void OnKeyboardTabulator(object obj)
        {
            _focusedTextBox?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        #endregion
    }
}