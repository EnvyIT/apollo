using System.Threading.Tasks;
using Apollo.Terminal.Interfaces;
using Apollo.Terminal.Types.TransferObject;

namespace Apollo.Terminal.ViewModels.Base
{
    public abstract class BaseStepViewModel : BaseViewModel, IStepViewModel
    {
        public abstract bool IsCancelAndBackEnabled();

        public abstract Task<ValidResultTransferObject<object>> IsValidAsync();
        public abstract void ShowValidationErrors(object value);

        public abstract Task<object> NextAsync();
        public abstract void NextDone();

        public abstract Task BackAsync();
    }
}