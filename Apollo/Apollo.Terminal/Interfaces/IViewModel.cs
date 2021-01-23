using System.Threading.Tasks;
using Apollo.Terminal.Types;

namespace Apollo.Terminal.Interfaces
{
    public delegate void PageRequest(PageViewType pageType, object argument);

    public interface IViewModel
    {
        Task InitializeAsync(object argument);

        void InitializeDone();

        Task ResetAsync();

        void ResetDone();
    }
}
