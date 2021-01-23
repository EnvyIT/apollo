using System;
using System.Threading;
using System.Threading.Tasks;

namespace Apollo.Util
{
    public class TaskHelper<T> : TaskHelper
    {
        private T _result;

        public new event Action<T> OnSuccess;

        public virtual Task Run(Func<Task<T>> doWork, CancellationTokenSource cancellation = null)
        {
            return base.Run(async () => { _result = await doWork(); }, cancellation);
        }

        protected override void CallOnSuccess()
        {
            OnSuccess?.Invoke(_result);
        }
    }

    public class TaskHelper
    {
        public static event Action<Exception> OnGlobalFail;

        public event Action<Exception> OnFail;
        public event Action OnSuccess;
        public event Action OnComplete;
        public event Action OnCancel;

        public virtual async Task Run(Func<Task> doWork, CancellationTokenSource cancellation = null)
        {
            try
            {
                var token = cancellation?.Token ?? new CancellationToken();
                var task = Task.Factory.StartNew(async () => await doWork(),
                    token,
                    TaskCreationOptions.None,
                    TaskScheduler.Default);
                await await task;

                if (token.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }

                CallOnSuccess();
                OnComplete?.Invoke();
            }
            catch (TaskCanceledException)
            {
                if (OnCancel != null)
                {
                    OnCancel?.Invoke();
                }
                else
                {
                    OnComplete?.Invoke();
                }
            }
            catch (Exception exception)
            {
                OnGlobalFail?.Invoke(exception);
                OnFail?.Invoke(exception);
                OnComplete?.Invoke();
            }
        }

        protected virtual void CallOnSuccess()
        {
            OnSuccess?.Invoke();
        }
    }
}