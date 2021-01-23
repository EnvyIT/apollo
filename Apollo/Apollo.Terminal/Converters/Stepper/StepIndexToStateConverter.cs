using System;
using System.Globalization;
using System.Windows.Data;
using Apollo.Terminal.Types.Stepper;

namespace Apollo.Terminal.Converters.Stepper
{
    public class StepIndexToStateConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int currentIndex))
            {
                throw new ArgumentException("Invalid index given!", nameof(value));
            }

            if (parameter == null || !int.TryParse(parameter.ToString(), out var stepIndex))
            {
                throw new ArgumentException("Invalid step index given!", nameof(parameter));
            }

            if (currentIndex > stepIndex)
            {
                return StepState.Done;
            }

            if (currentIndex < stepIndex)
            {
                return StepState.Open;
            }

            return StepState.Active;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
