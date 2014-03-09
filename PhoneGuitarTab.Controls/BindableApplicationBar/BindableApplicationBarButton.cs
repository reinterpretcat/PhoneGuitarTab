using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Shell;

namespace PhoneGuitarTab.Controls
{
    /// <summary>
    /// A wrapper for an <see cref="ApplicationBarIconButton"/> object
    /// that adds support for data binding.
    /// </summary>
    /// <remarks>
    /// To be used in <see cref="BindableApplicationBar.Buttons"/> or
    /// <see cref="BindableApplicationBar.ButtonTemplate"/>
    /// The class derives from <see cref="FrameworkElement"/> to support
    /// DataContext and bindings.
    /// </remarks>
    public class BindableApplicationBarButton : FrameworkElement
    {
        private ApplicationBar applicationBar;
        private ApplicationBarIconButton applicationBarIconButton;

        #region Text
        /// <summary>
        /// Text Dependency Property
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(BindableApplicationBarButton),
                new PropertyMetadata(null, OnTextChanged));

        /// <summary>
        /// Gets or sets the Text property. This dependency property 
        /// indicates the Text bound to the associated
        /// ApplicationBarIconButton's Text property.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Text property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnTextChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (BindableApplicationBarButton)d;
            string oldText = (string)e.OldValue;
            string newText = target.Text;
            target.OnTextChanged(oldText, newText);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the
        /// Text property.
        /// </summary>
        /// <param name="oldText">The old Text value.</param>
        /// <param name="newText">The new Text value.</param>
        protected virtual void OnTextChanged(string oldText, string newText)
        {
            if (this.applicationBarIconButton != null)
            {
                this.applicationBarIconButton.Text = newText;
            }
        }
        #endregion

        #region IconUri
        /// <summary>
        /// IconUri Dependency Property
        /// </summary>
        public static readonly DependencyProperty IconUriProperty =
            DependencyProperty.Register(
                "IconUri",
                typeof(Uri),
                typeof(BindableApplicationBarButton),
                new PropertyMetadata(null, OnIconUriChanged));

        /// <summary>
        /// Gets or sets the IconUri property. This dependency property 
        /// indicates the URI to the icon to display for the button.
        /// </summary>
        public Uri IconUri
        {
            get { return (Uri)GetValue(IconUriProperty); }
            set { SetValue(IconUriProperty, value); }
        }

        /// <summary>
        /// Handles changes to the IconUri property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnIconUriChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (BindableApplicationBarButton)d;
            Uri oldIconUri = (Uri)e.OldValue;
            Uri newIconUri = target.IconUri;
            target.OnIconUriChanged(oldIconUri, newIconUri);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the
        /// IconUri property.
        /// </summary>
        /// <param name="oldIconUri">The old IconUri value.</param>
        /// <param name="newIconUri">The new IconUri value.</param>
        protected virtual void OnIconUriChanged(
            Uri oldIconUri, Uri newIconUri)
        {
            if (this.applicationBarIconButton != null)
            {
                this.applicationBarIconButton.IconUri = this.IconUri;
            }
        }
        #endregion

        #region IsEnabled
        /// <summary>
        /// IsEnabled Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(
                "IsEnabled",
                typeof(bool),
                typeof(BindableApplicationBarButton),
                new PropertyMetadata(true, OnIsEnabledChanged));

        /// <summary>
        /// Gets or sets a value indicating whether the associated button is
        /// enabled.
        /// </summary>
        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        /// <summary>
        /// Handles changes to the IsEnabled property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnIsEnabledChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (BindableApplicationBarButton)d;
            bool oldIsEnabled = (bool)e.OldValue;
            bool newIsEnabled = target.IsEnabled;
            target.OnIsEnabledChanged(oldIsEnabled, newIsEnabled);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to
        /// the IsEnabled property.
        /// </summary>
        /// <param name="oldIsEnabled">The old IsEnabled value.</param>
        /// <param name="newIsEnabled">The new IsEnabled value.</param>
        protected virtual void OnIsEnabledChanged(
            bool oldIsEnabled, bool newIsEnabled)
        {
            if (this.applicationBarIconButton != null)
            {
                this.applicationBarIconButton.IsEnabled = this.IsEnabled;
            }
        }
        #endregion

        #region Command
        /// <summary>
        /// Command Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                "Command",
                typeof(ICommand),
                typeof(BindableApplicationBarButton),
                new PropertyMetadata(null, OnCommandChanged));

        /// <summary>
        /// Gets or sets the Command property. This dependency property 
        /// indicates the command to execute when the button gets clicked.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Command property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnCommandChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (BindableApplicationBarButton)d;
            ICommand oldCommand = (ICommand)e.OldValue;
            ICommand newCommand = target.Command;
            target.OnCommandChanged(oldCommand, newCommand);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to
        /// the Command property.
        /// </summary>
        /// <param name="oldCommand">The old Command value.</param>
        /// <param name="newCommand">The new Command value.</param>
        protected virtual void OnCommandChanged(
            ICommand oldCommand, ICommand newCommand)
        {
            if (oldCommand != null)
            {
                oldCommand.CanExecuteChanged -=
                    this.CommandCanExecuteChanged;
            }

            if (newCommand != null)
            {
                this.IsEnabled =
                    newCommand.CanExecute(this.CommandParameter);
                newCommand.CanExecuteChanged +=
                    this.CommandCanExecuteChanged;
            }
        }

        private void CommandCanExecuteChanged(object sender, EventArgs e)
        {
            if (this.Command != null)
            {
                this.IsEnabled =
                    this.Command.CanExecute(this.CommandParameter);
            }
        }
        #endregion

        #region CommandParameter
        /// <summary>
        /// CommandParameter Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                "CommandParameter",
                typeof(object),
                typeof(BindableApplicationBarButton),
                new PropertyMetadata(null, OnCommandParameterChanged));

        /// <summary>
        /// Gets or sets the CommandParameter property.
        /// This dependency property indicates the parameter to be passed
        /// to the Command when the button gets pressed.
        /// </summary>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Handles changes to the CommandParameter property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnCommandParameterChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (BindableApplicationBarButton)d;
            object oldCommandParameter = e.OldValue;
            object newCommandParameter = target.CommandParameter;
            target.OnCommandParameterChanged(
                oldCommandParameter, newCommandParameter);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to
        /// the CommandParameter property.
        /// </summary>
        /// <param name="oldCommandParameter">
        /// The old CommandParameter value.
        /// </param>
        /// <param name="newCommandParameter">
        /// The new CommandParameter value.
        /// </param>
        protected virtual void OnCommandParameterChanged(
            object oldCommandParameter, object newCommandParameter)
        {
            if (this.Command != null)
            {
                this.IsEnabled =
                    this.Command.CanExecute(this.CommandParameter);
            }
        }
        #endregion

        /// <summary>
        /// Creates an associated <see cref="ApplicationBarIconButton"/> and
        /// attaches it to the specified application bar.
        /// </summary>
        /// <param name="parentApplicationBar">
        /// The application bar to attach to.
        /// </param>
        /// <param name="i">
        /// The index at which the associated
        /// <see cref="ApplicationBarIconButton"/> will be inserted.
        /// </param>
        public void Attach(ApplicationBar parentApplicationBar, int i)
        {
            Debug.Assert(
                this.IconUri != null, "IconUri property cannot be null.");

            if (this.applicationBarIconButton != null)
            {
                return;
            }

            this.applicationBar = parentApplicationBar;
            this.applicationBarIconButton =
                new ApplicationBarIconButton(this.IconUri)
                {
                    Text = string.IsNullOrEmpty(this.Text) ? "." : this.Text,
                    IsEnabled = this.IsEnabled
                };

            this.applicationBarIconButton.Click +=
                this.ApplicationBarIconButtonClick;

            try
            {
                this.applicationBar.Buttons.Insert(
                    i, this.applicationBarIconButton);
            }
            catch (InvalidOperationException ex)
            {
                // Up to 4 buttons supported in ApplicationBar.Buttons
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
        /// Detaches the associated <see cref="ApplicationBarIconButton"/>
        /// from the <see cref="ApplicationBar"/> and from this instance.
        /// </summary>
        public void Detach()
        {
            this.applicationBarIconButton.Click -=
                this.ApplicationBarIconButtonClick;
            this.applicationBar.Buttons.Remove(
                this.applicationBarIconButton);
            this.applicationBar = null;
            this.applicationBarIconButton = null;
        }

        private void ApplicationBarIconButtonClick(
            object sender, EventArgs e)
        {
            if (this.Command != null &&
                this.Command.CanExecute(this.CommandParameter))
            {
                this.Command.Execute(this.CommandParameter);
            }
        }
    }
}