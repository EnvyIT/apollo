using System.Windows.Controls;
using Apollo.Core.Implementation;
using Apollo.Core.Interfaces;
using Apollo.Terminal.Common;
using Apollo.Terminal.ViewModels;
using Apollo.Util;
using MaterialDesignExtensions.Controls;
using Microsoft.Extensions.Configuration;

namespace Apollo.Terminal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MaterialWindow
    {
        public MainWindow()
        {
            ConfigurationHelper.ConfigurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.terminal.json")
                .Build();
            IServiceFactory serviceFactory = new ServiceFactory("Apollo_Database");
            LocalizationService.GetInstance().SetSystemCulture();
            
            RegisterOnScreenKeyboard();

            DataContext = new MainViewModel(serviceFactory);
            InitializeComponent();
        }

        private void RegisterOnScreenKeyboard()
        {
            var isEnabled = bool.Parse(ConfigurationHelper.GetValues("OnScreen_Keyboard_Enabled")[0]);
            if (isEnabled)
            {
                OnScreenHelper.BindTo<TextBox>();
            }
        }
    }
}
