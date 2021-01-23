using System.Windows.Input;
using Apollo.Terminal.Commands;
using Apollo.Terminal.Types;
using Apollo.Terminal.ViewModels.Base;

namespace Apollo.Terminal.ViewModels
{
    public class OverlayViewModel : BasePageViewModel
    {
        public ICommand NextPageCommand { get; }

        public OverlayViewModel()
        {
            NextPageCommand = new DelegateCommand<object>(_ => RequestNewPage(PageViewType.MovieList));
        }
    }
}