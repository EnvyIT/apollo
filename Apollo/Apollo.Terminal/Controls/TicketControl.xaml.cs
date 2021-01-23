using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Apollo.Terminal.Controls
{
    /// <summary>
    /// Interaction logic for TicketControl.xaml
    /// </summary>
    public partial class TicketControl : UserControl
    {
        public static readonly DependencyProperty MovieImageProperty =
            DependencyProperty.Register(nameof(MovieImage), typeof(IList<byte>), typeof(TicketControl));

        public static readonly DependencyProperty MovieTitleProperty =
            DependencyProperty.Register(nameof(MovieTitle), typeof(string), typeof(TicketControl));

        public static readonly DependencyProperty SeatsTextProperty =
            DependencyProperty.Register(nameof(SeatsText), typeof(string), typeof(TicketControl));

        public static readonly DependencyProperty PriceProperty =
            DependencyProperty.Register(nameof(Price), typeof(decimal), typeof(TicketControl));

        public static readonly DependencyProperty QrCodeImageProperty =
            DependencyProperty.Register(nameof(QrCodeImage), typeof(IList<byte>), typeof(TicketControl));

        public IList<byte> MovieImage
        {
            get => (IList<byte>) GetValue(MovieImageProperty);
            set => SetValue(MovieImageProperty, value);
        }

        public string MovieTitle
        {
            get => (string) GetValue(MovieTitleProperty);
            set => SetValue(MovieTitleProperty, value);
        }

        public string SeatsText
        {
            get => (string) GetValue(SeatsTextProperty);
            set => SetValue(SeatsTextProperty, value);
        }

        public decimal Price
        {
            get => (decimal) GetValue(PriceProperty);
            set => SetValue(PriceProperty, value);
        }

        public IList<byte> QrCodeImage
        {
            get => (IList<byte>) GetValue(QrCodeImageProperty);
            set => SetValue(QrCodeImageProperty, value);
        }

        public TicketControl()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }
    }
}