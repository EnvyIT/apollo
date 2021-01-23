using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using Apollo.Terminal.I18N;
using WPFLocalizeExtension.Engine;
using WPFLocalizeExtension.Extensions;

namespace Apollo.Terminal.Common
{
    public class LocalizationService : ILocalizationService
    {
        private const string ResourceKey = "Resources";
        private static readonly object Lock = new object();

        private static ILocalizationService _instance;

        private readonly ResourceManager _resourceManager = Resources.ResourceManager;
        private readonly HashSet<string> _supportedLanguages;

        private CultureInfo _currentCulture;

        public event Action<CultureInfo> OnCultureChanged;

        public CultureInfo CurrentCulture
        {
            get => _currentCulture;
            set
            {
                if (Equals(_currentCulture, value))
                {
                    return;
                }
                UpdateCulture(value);
            }
        }

        public ISet<string> SupportedLanguages => _supportedLanguages;

        private LocalizationService()
        {
            _supportedLanguages = LoadAvailableLanguages();
        }
        
        public string this[string key] => GetLocalizedValue<string>(key);

        public T GetLocalizedValue<T>(string key)
        {
            return LocExtension.GetLocalizedValue<T>($"{Assembly.GetCallingAssembly().GetName().Name}:{ResourceKey}:{key}");
        }

        public static ILocalizationService GetInstance()
        {
            lock (Lock)
            {
                _instance ??= new LocalizationService();
            }

            return _instance;
        }

        public void SetSystemCulture()
        {
            CurrentCulture = Thread.CurrentThread.CurrentUICulture;
        }

        private void UpdateCulture(CultureInfo culture)
        {
            _currentCulture = culture;
            LocalizeDictionary.Instance.Culture = culture;
            OnCultureChanged?.Invoke(culture);
        }

        private HashSet<string> LoadAvailableLanguages()
        {
            var result = new HashSet<string>();

            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (var culture in cultures)
            {
                try
                {
                    if (!culture.Equals(CultureInfo.InvariantCulture) &&
                        _resourceManager.GetResourceSet(culture, true, false) != null)
                    {
                        result.Add(culture.TwoLetterISOLanguageName);
                    }
                }
                catch (CultureNotFoundException)
                {
                    // ignore
                }
            }

            return result;
        }
    }
}