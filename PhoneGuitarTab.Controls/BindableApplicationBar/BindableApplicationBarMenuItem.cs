using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Shell;

namespace PhoneGuitarTab.Controls
{
    /// <summary>
    ///     A wrapper for an <see cref="ApplicationBarMenuItem" /> object
    ///     that adds support for data binding.
    /// </summary>
    /// <remarks>
    ///     To be used in <see cref="BindableApplicationBar.MenuItems" /> or
    ///     <see cref="BindableApplicationBar.MenuItemTemplate" />
    ///     <para />
    ///     The class derives from <see cref="FrameworkElement" /> to support
    ///     DataContext and bindings.
    ///     <para />
    ///     Note that <see cref="ApplicationBarMenuItem.Click" /> event is not
    ///     wrapped since /// the purpose of this class is to bind to view models
    ///     and use Commands instead.
    /// </remarks>
    /// <example>
    ///     See the below example for usage of the BindableApplicationBar:<br />
    ///     <code>
    /// <![CDATA[
    /// <bar:BindableApplicationBar
    ///     xmlns:bar="clr-namespace:BindableApplicationBar;assembly=BindableApplicationBar"
    ///     MenuItemsSource="{Binding MenuItems}">
    ///     <bar:BindableApplicationBar.MenuItemTemplate>
    ///         <DataTemplate>
    ///             <bar:BindableApplicationBarMenuItem
    ///                 Command="{Binding Command}"
    ///                 CommandParameter="{Binding CommandParameter}"
    ///                 IsEnabled="{Binding IsEnabled}"
    ///                 Text="{Binding Text}" />
    ///         </DataTemplate>
    ///     </bar:BindableApplicationBar.MenuItemTemplate>
    ///     <bar:BindableApplicationBar.MenuItems>
    ///         <bar:BindableApplicationBarMenuItem
    ///             Text="{Binding MenuItemText}"
    ///             IsEnabled="{Binding MenuItemIsEnabled}"/>
    ///         <bar:BindableApplicationBarMenuItem
    ///             Text="Menu item 2"
    ///             Command="{Binding TestCommand2}"
    ///             CommandParameter="{Binding TestCommand2Parameter}" />
    ///     </bar:BindableApplicationBar.MenuItems>
    /// </bar:BindableApplicationBar>
    /// ]]>
    /// </code>
    /// </example>
    /// <seealso cref="BindableApplicationBar" />
    /// <seealso cref="BindableApplicationBarButton" />
    public class BindableApplicationBarMenuItem : FrameworkElement
    {
        private ApplicationBar applicationBar;
        private ApplicationBarMenuItem applicationBarMenuItem;

        #region Text

        /// <summary>
        ///     Text Dependency Property
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof (string),
                typeof (BindableApplicationBarMenuItem),
                new PropertyMetadata(null, OnTextChanged));

        /// <summary>
        ///     Gets or sets the Text property. This dependency property
        ///     indicates the Text bound to the associated
        ///     ApplicationBarMenuItem's Text property.
        /// </summary>
        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        ///     Handles changes to the Text property.
        /// </summary>
        /// <param name="d">
        ///     The <see cref="DependencyObject" /> on which
        ///     the property has changed value.
        /// </param>
        /// <param name="e">
        ///     Event data that is issued by any event that
        ///     tracks changes to the effective value of this property.
        /// </param>
        private static void OnTextChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (BindableApplicationBarMenuItem) d;
            string oldText = (string) e.OldValue;
            string newText = target.Text;
            target.OnTextChanged(oldText, newText);
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to the
        ///     Text property.
        /// </summary>
        /// <param name="oldText">The old text.</param>
        /// <param name="newText">The new text.</param>
        protected virtual void OnTextChanged(string oldText, string newText)
        {
            if (applicationBarMenuItem != null)
            {
                applicationBarMenuItem.Text = newText;
            }
        }

        #endregion

        #region IsEnabled

        /// <summary>
        ///     IsEnabled Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(
                "IsEnabled",
                typeof (bool),
                typeof (BindableApplicationBarMenuItem),
                new PropertyMetadata(true, OnIsEnabledChanged));

        /// <summary>
        ///     Gets or sets a value indicating whether the associated menu item
        ///     is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get { return (bool) GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        /// <summary>
        ///     Handles changes to the IsEnabled property.
        /// </summary>
        /// <param name="d">
        ///     The <see cref="DependencyObject" /> on which
        ///     the property has changed value.
        /// </param>
        /// <param name="e">
        ///     Event data that is issued by any event that
        ///     tracks changes to the effective value of this property.
        /// </param>
        private static void OnIsEnabledChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (BindableApplicationBarMenuItem) d;
            bool oldIsEnabled = (bool) e.OldValue;
            bool newIsEnabled = target.IsEnabled;
            target.OnIsEnabledChanged(oldIsEnabled, newIsEnabled);
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to the
        ///     IsEnabled property.
        /// </summary>
        /// <param name="oldIsEnabled">The old IsEnabled value.</param>
        /// <param name="newIsEnabled">The new IsEnabled value.</param>
        protected virtual void OnIsEnabledChanged(
            bool oldIsEnabled, bool newIsEnabled)
        {
            if (applicationBarMenuItem != null)
            {
                applicationBarMenuItem.IsEnabled = IsEnabled;
            }
        }

        #endregion

        #region Command

        /// <summary>
        ///     Command Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                "Command",
                typeof (ICommand),
                typeof (BindableApplicationBarMenuItem),
                new PropertyMetadata(null, OnCommandChanged));

        /// <summary>
        ///     Gets or sets the Command property. This dependency property
        ///     indicates the command to execute when the menuItem gets clicked.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        ///     Handles changes to the Command property.
        /// </summary>
        /// <param name="d">
        ///     The <see cref="DependencyObject" /> on which
        ///     the property has changed value.
        /// </param>
        /// <param name="e">
        ///     Event data that is issued by any event that
        ///     tracks changes to the effective value of this property.
        /// </param>
        private static void OnCommandChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (BindableApplicationBarMenuItem) d;
            ICommand oldCommand = (ICommand) e.OldValue;
            ICommand newCommand = target.Command;
            target.OnCommandChanged(oldCommand, newCommand);
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to
        ///     the Command property.
        /// </summary>
        /// <param name="oldCommand">The old command.</param>
        /// <param name="newCommand">The new command.</param>
        protected virtual void OnCommandChanged(
            ICommand oldCommand, ICommand newCommand)
        {
            if (oldCommand != null)
            {
                oldCommand.CanExecuteChanged -=
                    CommandCanExecuteChanged;
            }

            if (newCommand != null)
            {
                IsEnabled =
                    newCommand.CanExecute(CommandParameter);
                newCommand.CanExecuteChanged +=
                    CommandCanExecuteChanged;
            }
        }

        private void CommandCanExecuteChanged(object sender, EventArgs e)
        {
            if (Command != null)
            {
                IsEnabled =
                    Command.CanExecute(CommandParameter);
            }
        }

        #endregion

        #region CommandParameter

        /// <summary>
        ///     CommandParameter Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                "CommandParameter",
                typeof (object),
                typeof (BindableApplicationBarMenuItem),
                new PropertyMetadata(null, OnCommandParameterChanged));

        /// <summary>
        ///     Gets or sets the CommandParameter property.
        ///     This dependency property indicates the parameter to be passed
        ///     to the Command when the menu item gets pressed.
        /// </summary>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        ///     Handles changes to the CommandParameter property.
        /// </summary>
        /// <param name="d">
        ///     The <see cref="DependencyObject" /> on which
        ///     the property has changed value.
        /// </param>
        /// <param name="e">
        ///     Event data that is issued by any event that
        ///     tracks changes to the effective value of this property.
        /// </param>
        private static void OnCommandParameterChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (BindableApplicationBarMenuItem) d;
            object oldCommandParameter = e.OldValue;
            object newCommandParameter = target.CommandParameter;
            target.OnCommandParameterChanged(
                oldCommandParameter, newCommandParameter);
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to
        ///     the CommandParameter property.
        /// </summary>
        /// <param name="oldCommandParameter">
        ///     The old CommandParameter value.
        /// </param>
        /// <param name="newCommandParameter">
        ///     The new CommandParameter value.
        /// </param>
        protected virtual void OnCommandParameterChanged(
            object oldCommandParameter, object newCommandParameter)
        {
            if (Command != null)
            {
                IsEnabled =
                    Command.CanExecute(CommandParameter);
            }
        }

        #endregion

        /// <summary>
        ///     Creates an associated <see cref="ApplicationBarMenuItem" /> and
        ///     attaches it to the specified application bar.
        /// </summary>
        /// <param name="parentApplicationBar">
        ///     The application bar to attach to.
        /// </param>
        /// <param name="i">
        ///     The index at which the associated
        ///     <see cref="ApplicationBarMenuItem" /> will be inserted.
        /// </param>
        public void Attach(ApplicationBar parentApplicationBar, int i)
        {
            if (applicationBarMenuItem != null)
            {
                return;
            }

            applicationBar = parentApplicationBar;
            applicationBarMenuItem =
                new ApplicationBarMenuItem
                {
                    Text = string.IsNullOrEmpty(Text) ? "." : Text,
                    IsEnabled = IsEnabled
                };

            applicationBarMenuItem.Click +=
                ApplicationBarMenuItemClick;

            try
            {
                applicationBar.MenuItems.Insert(
                    i, applicationBarMenuItem);
            }
            catch (InvalidOperationException ex)
            {
                // Up to 50 menu items supported in ApplicationBar.MenuItems
                // at the time of this writing.
                if (ex.Message == "Too many items in list" &&
                    Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }

        /// <summary>
        ///     Detaches the associated <see cref="ApplicationBarMenuItem" />
        ///     from the <see cref="ApplicationBar" /> and from this instance.
        /// </summary>
        public void Detach()
        {
            applicationBarMenuItem.Click -=
                ApplicationBarMenuItemClick;
            applicationBar.MenuItems.Remove(
                applicationBarMenuItem);
            applicationBar = null;
            applicationBarMenuItem = null;
        }

        private void ApplicationBarMenuItemClick(object sender, EventArgs e)
        {
            if (Command != null &&
                Command.CanExecute(CommandParameter))
            {
                Command.Execute(CommandParameter);
            }
        }
    }
}