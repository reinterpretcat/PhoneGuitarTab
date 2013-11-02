
using System.Windows.Input;

namespace PhoneGuitarTab.UI.Infrastructure
{
    using System;
    using System.Windows;

    using Coding4Fun.Toolkit.Controls;

    /// <summary>
    /// Incapsulates underlying dialog API
    /// </summary>
    public interface IDialogController
    {
        void Show(string message);
        void Show(string title, string message);
        void Show(string title, string message, DialogActionContainer actionContainer);
    }

    /// <summary>
    /// Contains typical event handlers for dialog
    /// </summary>
    public class DialogActionContainer
    {
        public EventHandler<GestureEventArgs> OnTapAction { get; set; }
        public EventHandler<GestureEventArgs> OnDoubleTapAction { get; set; }
        public EventHandler<GestureEventArgs> OnHoldAction { get; set; }

        public DialogActionContainer()
        {
            OnTapAction = (o, e) => { };
            OnDoubleTapAction = (o, e) => { };
            OnHoldAction = (o, e) => { };
        }
    }

    /// <summary>
    /// Uses  built-in MessageBox to display messages
    /// </summary>
    public class DefaultDialogController : IDialogController
    {

        public void Show(string message)
        {
            MessageBox.Show(message);
        }

        public void Show(string title, string message)
        {
            MessageBox.Show(title, message, MessageBoxButton.OK);
        }

        public void Show(string title, string message, DialogActionContainer actionContainer)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Uses toast control for better UX
    /// </summary>
    public class ToastDialogController : IDialogController
    {

        public void Show(string message)
        {
            CreateToastPromt("", message).Show();
        }

        public void Show(string title, string message)
        {
            CreateToastPromt(title, message).Show();
        }

        public void Show(string title, string message, DialogActionContainer actionContainer)
        {
            var toast = CreateToastPromt(title, message);

            toast.Tap += actionContainer.OnTapAction;
            toast.DoubleTap += actionContainer.OnDoubleTapAction;
            toast.Hold += actionContainer.OnHoldAction;

            toast.Show();
        }

        private static ToastPrompt CreateToastPromt(string title, string message)
        {
            return new ToastPrompt()
            {
                Title = message,
                Message = title,
                TextWrapping = TextWrapping.Wrap,
                TextOrientation = System.Windows.Controls.Orientation.Vertical
            };
        }
    }
}
