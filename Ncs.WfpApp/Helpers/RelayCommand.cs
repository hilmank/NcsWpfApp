using System.Windows.Input;

namespace Ncs.WfpApp.Helpers
{
    public class RelayCommand : ICommand
    {
        private readonly Func<bool> _canExecute;
        private readonly Action _execute;

        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        public bool CanExecute(object? parameter) => _canExecute();

        public void Execute(object? parameter) => _execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
