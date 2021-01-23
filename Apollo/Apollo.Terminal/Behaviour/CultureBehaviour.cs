using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Apollo.Terminal.Common;
using Microsoft.Xaml.Behaviors;

namespace Apollo.Terminal.Behaviour
{
    public class CultureBehaviour : Behavior<FrameworkElement>
    {
        private FrameworkElement _control;
        private DependencyProperty _controlProperty;

        protected override void OnAttached()
        {
            base.OnAttached();
            _control = AssociatedObject;

            InitHostingControl();
            LocalizationService.GetInstance().OnCultureChanged += Translator_LanguageChanged;
        }

        protected override void OnDetaching()
        {
            LocalizationService.GetInstance().OnCultureChanged -= Translator_LanguageChanged;
            base.OnDetaching();
        }

        private void Translator_LanguageChanged(CultureInfo culture)
        {
            _control?.GetBindingExpression(_controlProperty)?.UpdateTarget();
        }

        private void InitHostingControl()
        {
            if (_control is TextBlock)
            {
                _controlProperty = TextBlock.TextProperty;
            }
            else if (typeof(TextBox) == _control.GetType())
            {
                _controlProperty = TextBox.TextProperty;

            }
            // extend further properties
        }
    }
}
