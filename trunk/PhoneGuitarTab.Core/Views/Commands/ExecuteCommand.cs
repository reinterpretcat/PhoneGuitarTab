namespace PhoneGuitarTab.Core.Views.Commands
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Interactivity command
    /// </summary>
    public class ExecuteCommand: ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        public ExecuteCommand(Action execute)
            : this(execute, null)
        {
        }

        public ExecuteCommand(Action execute , Func<bool> canExecute)
        {
            this._execute = execute;
            this._canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (this._canExecute != null)
                return this._canExecute();
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            this._execute();
        }
    }
}
