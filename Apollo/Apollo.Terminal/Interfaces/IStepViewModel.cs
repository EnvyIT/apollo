using System.Threading.Tasks;
using Apollo.Terminal.Types.TransferObject;

namespace Apollo.Terminal.Interfaces
{
    public interface IStepViewModel : IViewModel
    {
        Task<ValidResultTransferObject<object>> IsValidAsync();

        void ShowValidationErrors(object value);

        Task<object> NextAsync();

        void NextDone();

        Task BackAsync();

        bool IsCancelAndBackEnabled();
    }
}