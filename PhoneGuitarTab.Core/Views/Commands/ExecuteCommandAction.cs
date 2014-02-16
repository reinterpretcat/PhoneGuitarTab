
namespace PhoneCore.Framework.Views.Commands
{
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interactivity;

    /// <summary>
    /// Interactivity command trigger
    /// </summary>
    public class ExecuteCommandAction : TriggerAction<DependencyObject>
    {
        #region Properties

        #region Command
        public ICommand Command
        {
            get { return (ICommand)base.GetValue(CommandProperty); }
            set { base.SetValue(CommandProperty, value); }
        }

        public static ICommand GetCommand(DependencyObject obj)
        {

            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ExecuteCommandAction), null);

        #endregion

        #region Command parameter

        public object CommandParameter
        {
            get { return base.GetValue(CommandParameterProperty); }
            set { base.SetValue(CommandParameterProperty, value); }
        }

        public static object GetParameterCommand(DependencyObject obj)
        {
            return obj.GetValue(CommandParameterProperty);
        }

        public static void SetParameterCommand(DependencyObject obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(ExecuteCommandAction), null);

        #endregion

        #endregion Properties

        
        protected override void Invoke(object parameter)
        {
            ICommand command = Command ?? GetCommand(AssociatedObject);
            if (CommandParameter != null)
                parameter = CommandParameter;
            if (command != null && command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
        }
    }
}
