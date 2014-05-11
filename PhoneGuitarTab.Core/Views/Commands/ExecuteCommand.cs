using System;
using System.Windows.Input;

namespace PhoneGuitarTab.Core.Views.Commands
{
    /// <summary>
    ///     Interactivity command
    /// </summary>
    public class ExecuteCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public ExecuteCommand(Action execute)
            : this(execute, null)
        {
        }

        public ExecuteCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute();
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _execute();
        }
    }
}