using System.Windows;
using Apollo.Util;
using Apollo.Util.Logger;
using WPFLocalizeExtension.Engine;

namespace Apollo.Terminal
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly IApolloLogger<App> Logger = LoggerFactory.CreateLogger<App>();

        public App()
        {
            LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
            TaskHelper.OnGlobalFail += exception =>
            {
                Logger.Error(exception, exception.Message);
            };
        }
    }
}
