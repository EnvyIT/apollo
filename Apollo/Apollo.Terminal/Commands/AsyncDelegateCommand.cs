using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Apollo.Terminal.Commands
{
    public class AsyncDelegateCommand<T> : ICommand
    {
        private readonly Func<T, Task> _executeAsync;
        private readonly Predicate<T> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public AsyncDelegateCommand(Func<T, Task> executeAsync, Predicate<T> canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public async Task ExecuteAsync(object parameter)
        {
            await _executeAsync((T)parameter);
        }

        async void ICommand.Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }
    }
}
