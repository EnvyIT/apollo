using System.Windows;
using System.Windows.Controls;

namespace Apollo.Terminal.Common
{
    public class GridHelper
    {

        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.RegisterAttached(
                "RowCount", typeof(int), typeof(GridHelper),
                new PropertyMetadata(-1, RowCountChanged));


        public static int GetRowCount(DependencyObject dependencyObject)
        {
            return (int)dependencyObject.GetValue(RowCountProperty);
        }


        public static void SetRowCount(DependencyObject dependencyObject, int value)
        {
            dependencyObject.SetValue(RowCountProperty, value);
        }

        public static void RowCountChanged(
            DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEvent)
        {
            if (!(dependencyObject is Grid) || (int)dependencyPropertyChangedEvent.NewValue < 0)
                return;

            var grid = (Grid)dependencyObject;
            grid.RowDefinitions.Clear();

            for (var i = 0; i < (int)dependencyPropertyChangedEvent.NewValue; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }
        }

        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.RegisterAttached(
                "ColumnCount", typeof(int), typeof(GridHelper),
                new PropertyMetadata(-1, ColumnCountChanged));

        public static int GetColumnCount(DependencyObject dependencyObject)
        {
            return (int)dependencyObject.GetValue(ColumnCountProperty);
        }
        public static void SetColumnCount(DependencyObject dependencyObject, int value)
        {
            dependencyObject.SetValue(ColumnCountProperty, value);
        }

        public static void ColumnCountChanged(
            DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (!(dependencyObject is Grid) || (int)dependencyPropertyChangedEventArgs.NewValue < 0)
            {
                return;
            }

            var grid = (Grid)dependencyObject;
            grid.ColumnDefinitions.Clear();

            for (var i = 0; i < (int)dependencyPropertyChangedEventArgs.NewValue; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }
   
        }

    }
}
