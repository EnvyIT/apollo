using System.Windows;
using System.Windows.Controls;
using Apollo.Terminal.Types.Stepper;

namespace Apollo.Terminal.Controls.Stepper
{
    /// <summary>
    /// Interaction logic for StepperCircle.xaml
    /// </summary>
    public partial class StepperCircle : UserControl
    {
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register(nameof(Index), typeof(int), typeof(StepperCircle));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(StepperCircle));

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(nameof(State), typeof(StepState), typeof(StepperCircle));

        public int Index
        {
            get => (int) GetValue(IndexProperty);
            set => SetValue(IndexProperty, value);
        }

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public StepState State
        {
            get => (StepState) GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        public StepperCircle()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }
    }
}