using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Apollo.Terminal.Commands;
using MaterialDesignThemes.Wpf;

namespace Apollo.Terminal.Controls.OnScreenKeyboard
{
    /// <summary>
    /// Interaction logic for OnScreenIconKeyControl.xaml
    /// </summary>
    public partial class OnScreenIconKeyControl : UserControl
    {
        private readonly Subject<object> _keyChangedSubject;

        public static readonly DependencyProperty KeyCommandProperty = DependencyProperty.Register(
            nameof(KeyCommand), typeof(ICommand), typeof(OnScreenIconKeyControl));

        public static readonly DependencyProperty KeyIconProperty = DependencyProperty.Register(
            nameof(KeyIcon), typeof(PackIconKind), typeof(OnScreenIconKeyControl));

        public ICommand KeyCommand
        {
            get => (ICommand) GetValue(KeyCommandProperty);
            set => SetValue(KeyCommandProperty, value);
        }

        public PackIconKind KeyIcon
        {
            get => (PackIconKind) GetValue(KeyIconProperty);
            set => SetValue(KeyIconProperty, value);
        }

        public ICommand InternalKeyCommand { get; }

        public OnScreenIconKeyControl()
        {
            _keyChangedSubject = new Subject<object>();
            InitializeKeyListener();

            InternalKeyCommand = new DelegateCommand<object>(obj => _keyChangedSubject.OnNext(obj));

            InitializeComponent();
        }

        private void InitializeKeyListener()
        {
            _keyChangedSubject
                .Throttle(TimeSpan.FromMilliseconds(100))
                .Do(key => Application.Current.Dispatcher.Invoke(() => OnKeyChanged(key)))
                .Subscribe(key => { });
            Loaded += (s, e) =>
            {
                var window = Window.GetWindow(this);
                if (window != null)
                {
                    window.Closing += (s1, e1) => { _keyChangedSubject.Dispose(); };
                }
            };
        }

        private void OnKeyChanged(object obj)
        {
            KeyCommand?.Execute(obj);
        }
    }
}