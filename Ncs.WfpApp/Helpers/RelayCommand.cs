using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ncs.WpfApp.Helpers
{
    /// <summary>
    /// A command that executes a synchronous action.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? (() => true);
        }

        public bool CanExecute(object? parameter) => _canExecute();

        public void Execute(object? parameter) => _execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// A command that executes an asynchronous action.
    /// </summary>
    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _executeAsync;
        private readonly Func<bool> _canExecute;

        public event EventHandler? CanExecuteChanged;

        public AsyncRelayCommand(Func<Task> executeAsync, Func<bool> canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute ?? (() => true);
        }

        public bool CanExecute(object? parameter) => _canExecute();

        public async void Execute(object? parameter) => await _executeAsync();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// A command that executes a synchronous action with a parameter.
    /// </summary>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? (_ => true);
        }

        public bool CanExecute(object? parameter) => parameter is T param && _canExecute(param);

        public void Execute(object? parameter)
        {
            if (parameter is T param)
            {
                _execute(param);
            }
        }

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// A command that executes an asynchronous action with a parameter.
    /// </summary>
    public class AsyncRelayCommand<T> : ICommand
    {
        private readonly Func<T, Task> _executeAsync;
        private readonly Func<T, bool> _canExecute;

        public event EventHandler? CanExecuteChanged;

        public AsyncRelayCommand(Func<T, Task> executeAsync, Func<T, bool> canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute ?? (_ => true);
        }

        public bool CanExecute(object? parameter) => parameter is T param && _canExecute(param);

        public async void Execute(object? parameter)
        {
            if (parameter is T param)
            {
                await _executeAsync(param);
            }
        }

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
