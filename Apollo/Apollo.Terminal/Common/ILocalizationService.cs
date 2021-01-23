using System;
using System.Collections.Generic;
using System.Globalization;

namespace Apollo.Terminal.Common
{
    public interface ILocalizationService
    {
        CultureInfo CurrentCulture { get; set; }

        ISet<string> SupportedLanguages { get; }

        T GetLocalizedValue<T>(string value);
        string this[string index] { get; }

        event Action<CultureInfo> OnCultureChanged;

        void SetSystemCulture();

    }
}
