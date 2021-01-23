using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;
using Apollo.Terminal.Commands;
using Apollo.Terminal.Common;

namespace Apollo.Terminal.Controls
{
    /// <summary>
    /// Interaction logic for TranslationControl.xaml
    /// </summary>
    public partial class TranslationControl : UserControl
    {
        public ISet<string> SupportedLanguages => LocalizationService.GetInstance().SupportedLanguages;

        public int SupportedLanguagesCount => SupportedLanguages.Count;

        public ICommand SelectLanguageCommand { get; set; }

        public TranslationControl()
        {
            SelectLanguageCommand = new DelegateCommand<string>(SelectLanguage, language => LocalizationService.GetInstance().CurrentCulture.TwoLetterISOLanguageName != language);

            InitializeComponent();
            DataContext = this;
        }

        private static void SelectLanguage(object language)
        {
            LocalizationService.GetInstance().CurrentCulture = new CultureInfo((string) language);
        }
    }
}