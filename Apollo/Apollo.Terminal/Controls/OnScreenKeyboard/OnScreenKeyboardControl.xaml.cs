using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Apollo.Terminal.Commands;
using Apollo.Terminal.Converters.OnScreenKeyboard;

namespace Apollo.Terminal.Controls.OnScreenKeyboard
{
    /// <summary>
    /// Interaction logic for OnScreenKeyboardControl.xaml
    /// </summary>
    public partial class OnScreenKeyboardControl : UserControl, INotifyPropertyChanged
    {
        private bool _isCapsActive;
        private bool _capsLocked;
        private readonly Subject<string> _keyChangedSubject;

        public static readonly DependencyProperty KeyChangedCommandProperty = DependencyProperty.Register(
            nameof(KeyChangedCommand), typeof(ICommand), typeof(OnScreenKeyboardControl));

        public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register(
            nameof(DeleteCommand), typeof(ICommand), typeof(OnScreenKeyboardControl));

        public static readonly DependencyProperty SpaceCommandProperty = DependencyProperty.Register(
            nameof(SpaceCommand), typeof(ICommand), typeof(OnScreenKeyboardControl));

        public static readonly DependencyProperty TabulatorCommandProperty = DependencyProperty.Register(
            nameof(TabulatorCommand), typeof(ICommand), typeof(OnScreenKeyboardControl));

        public static readonly DependencyProperty EnterCommandProperty = DependencyProperty.Register(
            nameof(EnterCommand), typeof(ICommand), typeof(OnScreenKeyboardControl));

        public ICommand KeyChangedCommand
        {
            get => (ICommand) GetValue(KeyChangedCommandProperty);
            set => SetValue(KeyChangedCommandProperty, value);
        }

        public ICommand DeleteCommand
        {
            get => (ICommand) GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }

        public ICommand SpaceCommand
        {
            get => (ICommand) GetValue(SpaceCommandProperty);
            set => SetValue(SpaceCommandProperty, value);
        }

        public ICommand TabulatorCommand
        {
            get => (ICommand) GetValue(TabulatorCommandProperty);
            set => SetValue(TabulatorCommandProperty, value);
        }

        public ICommand EnterCommand
        {
            get => (ICommand) GetValue(EnterCommandProperty);
            set => SetValue(EnterCommandProperty, value);
        }

        public bool IsCapsActive
        {
            get => _isCapsActive;
            set => Set(ref _isCapsActive, value);
        }

        public ICommand CapsCommand { get; }
        public ICommand CapsLockCommand { get; }

        public ICommand InternalKeyChangedCommand { get; }

        public OnScreenKeyboardControl()
        {
            _keyChangedSubject = new Subject<string>();
            InitializeKeyListener();

            CapsCommand = new DelegateCommand<object>(OnCapsPressed);
            CapsLockCommand = new DelegateCommand<object>(OnCapsLockPressed);
            InternalKeyChangedCommand = new DelegateCommand<string>(key => _keyChangedSubject.OnNext(key));

            IsCapsActive = false;
            _capsLocked = false;

            InitializeComponent();
        }

        private void InitializeKeyListener()
        {
            _keyChangedSubject
                .Throttle(TimeSpan.FromMilliseconds(100))
                .Do(key => Application.Current.Dispatcher.Invoke(() => OnKeyChanged(key)))
                .Subscribe(key => {});
            Loaded += (s, e) =>
            {
                var window = Window.GetWindow(this);
                if (window != null)
                {
                    window.Closing += (s1, e1) => { _keyChangedSubject.Dispose(); };
                }
            };
        }

        private void OnKeyChanged(string key)
        {
            KeyChangedCommand?.Execute(_isCapsActive ? key.ToUpper() : key.ToLower());
            if (_isCapsActive && !_capsLocked)
            {
                IsCapsActive = false;
            }
        }

        private void OnCapsPressed(object obj)
        {
            _capsLocked = false;
            IsCapsActive = !IsCapsActive;
        }

        private void OnCapsLockPressed(object obj)
        {
            _capsLocked = !_capsLocked;
            IsCapsActive = _capsLocked;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Set<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(value, field))
            {
                return;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}