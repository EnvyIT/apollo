using System;
using System.Threading.Tasks;
using Apollo.Terminal.Common;
using Apollo.Terminal.Interfaces;
using Apollo.Util;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Configuration;

namespace Apollo.Terminal.ViewModels.Base
{
    public abstract class BaseViewModel : NotifyPropertyChanged, IViewModel
    {
        private ILocalizationService _localization;
        public static ISnackbarMessageQueue MessageQueue { get; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(3.0));

        protected BaseViewModel()
        {
            SetConfigurationPath();
            _localization = LocalizationService.GetInstance();
        }

        public virtual Task InitializeAsync(object argument)
        {
            return Task.CompletedTask;
        }

        public virtual void InitializeDone()
        {
            // nothing to do.
        }

        public virtual Task ResetAsync()
        {
            return Task.CompletedTask;
        }

        public virtual void ResetDone()
        {
            // nothing to do.
        }

        protected void SetConfigurationPath()
        {
            ConfigurationHelper.ConfigurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.terminal.json")
                .Build();
        }

        protected void ShowSnackBar(Exception exception)
        {
            ShowSnackBar(exception.Message, exception);
        }

        protected void ShowSnackBar(string message, Exception exception = null)
        {
            var localizedMessage = _localization.GetLocalizedValue<string>(message);
            if (localizedMessage != null)
            {
                MessageQueue.Enqueue(localizedMessage);
            }
            
        }

    }
}