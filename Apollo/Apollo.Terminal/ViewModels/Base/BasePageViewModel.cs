using Apollo.Terminal.Interfaces;
using Apollo.Terminal.Types;

namespace Apollo.Terminal.ViewModels.Base
{
    public class BasePageViewModel: BaseViewModel, IPageViewModel
    {
        public event PageRequest OnPageRequest;

        protected void RequestNewPage(PageViewType nextPage, object argument = null)
        {
            OnPageRequest?.Invoke(nextPage, argument);
        }
    }
}
