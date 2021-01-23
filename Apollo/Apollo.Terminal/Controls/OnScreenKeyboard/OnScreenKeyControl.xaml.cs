using System.Windows;
using System.Windows.Controls;

namespace Apollo.Terminal.Controls.OnScreenKeyboard
{
    /// <summary>
    /// Interaction logic for OnScreenKeyControl.xaml
    /// </summary>
    public partial class OnScreenKeyControl : UserControl
    {
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register(
            nameof(Key), typeof(string), typeof(OnScreenKeyControl));

        public string Key
        {
            get => (string) GetValue(KeyProperty);
            set => SetValue(KeyProperty, value);
        }

        public OnScreenKeyControl()
        {
            InitializeComponent();
        }
    }
}