using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PhoneGuitarTab.Controls
{
    public static class KeyUp
    {
        private static readonly DependencyProperty KeyUpCommandBehaviorProperty = DependencyProperty.RegisterAttached(
            "KeyUpCommandBehavior",
            typeof(TextBoxCommandBehavior),
            typeof(KeyUp),
            null);


        /// <summary>
        /// Command to execute on KeyUp event.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(KeyUp),
            new PropertyMetadata(OnSetCommandCallback));

        /// <summary>
        /// Command parameter to supply on command execution.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(KeyUp),
            new PropertyMetadata(OnSetCommandParameterCallback));


        /// <summary>
        /// Sets the <see cref="ICommand"/> to execute on the KeyUp event.
        /// </summary>
        /// <param name="textBox">TextBox dependency object to attach command</param>
        /// <param name="command">Command to attach</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only works for buttonbase")]
        public static void SetCommand(TextBox textBox, ICommand command)
        {
            textBox.SetValue(CommandProperty, command);
        }

        /// <summary>
        /// Retrieves the <see cref="ICommand"/> attached to the <see cref="TextBox"/>.
        /// </summary>
        /// <param name="textBox">TextBox containing the Command dependency property</param>
        /// <returns>The value of the command attached</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only works for buttonbase")]
        public static ICommand GetCommand(TextBox textBox)
        {
            return textBox.GetValue(CommandProperty) as ICommand;
        }

        /// <summary>
        /// Sets the value for the CommandParameter attached property on the provided <see cref="TextBox"/>.
        /// </summary>
        /// <param name="textBox">TextBox to attach CommandParameter</param>
        /// <param name="parameter">Parameter value to attach</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only works for buttonbase")]
        public static void SetCommandParameter(TextBox textBox, object parameter)
        {
            textBox.SetValue(CommandParameterProperty, parameter);
        }

        /// <summary>
        /// Gets the value in CommandParameter attached property on the provided <see cref="TextBox"/>
        /// </summary>
        /// <param name="textBox">TextBox that has the CommandParameter</param>
        /// <returns>The value of the property</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only works for buttonbase")]
        public static object GetCommandParameter(TextBox textBox)
        {
            return textBox.GetValue(CommandParameterProperty);
        }

        private static void OnSetCommandCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = dependencyObject as TextBox;
            if (textBox != null)
            {
                TextBoxCommandBehavior behavior = GetOrCreateBehavior(textBox);
                behavior.Command = e.NewValue as ICommand;
            }
        }

        private static void OnSetCommandParameterCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = dependencyObject as TextBox;
            if (textBox != null)
            {
                TextBoxCommandBehavior behavior = GetOrCreateBehavior(textBox);
                behavior.CommandParameter = e.NewValue;
            }
        }

        private static TextBoxCommandBehavior GetOrCreateBehavior(TextBox textBox)
        {
            TextBoxCommandBehavior behavior = textBox.GetValue(KeyUpCommandBehaviorProperty) as TextBoxCommandBehavior;
            if (behavior == null)
            {
                behavior = new TextBoxCommandBehavior(textBox);
                textBox.SetValue(KeyUpCommandBehaviorProperty, behavior);
            }

            return behavior;
        }
    }
}
