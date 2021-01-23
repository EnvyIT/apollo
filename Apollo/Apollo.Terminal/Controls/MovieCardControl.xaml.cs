using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Apollo.Terminal.Controls
{
    /// <summary>
    /// Interaction logic for MovieCardControl.xaml
    /// </summary>
    public partial class MovieCardControl : UserControl
    {
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register(nameof(Image), typeof(IList<byte>), typeof(MovieCardControl));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(MovieCardControl));

        public static readonly DependencyProperty GenreProperty =
            DependencyProperty.Register(nameof(Genre), typeof(string), typeof(MovieCardControl));

        public static readonly DependencyProperty RatingProperty =
            DependencyProperty.Register(nameof(Rating), typeof(int), typeof(MovieCardControl));

        public IList<byte> Image
        {
            get => (IList<byte>) GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Genre
        {
            get => (string)GetValue(GenreProperty);
            set => SetValue(GenreProperty, value);
        }


        public int Rating
        {
            get => (int)GetValue(RatingProperty);
            set => SetValue(RatingProperty, value);
        }

        public MovieCardControl()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }
    }
}