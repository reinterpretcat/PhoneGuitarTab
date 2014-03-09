using System;
using System.Windows;
using System.Windows.Data;

namespace PhoneGuitarTab.Controls
{
    /// <summary>
    /// Allows to subscribe for any control to an event triggered when the control's DataContext changes.
    /// </summary>
    public class FlexibleDataContextChangedHelper
    {
        #region Subscribe()
        public static void Subscribe<T>(T control, Action<T> callback)
            where T : FrameworkElement
        {
            FlexibleDataContextChangedHelper<T>.Subscribe(control, callback);
        }
        #endregion

        #region Unsubscribe()
        public static void Unsubscribe<T>(T control)
            where T : FrameworkElement
        {
            FlexibleDataContextChangedHelper<T>.Unsubscribe(control);
        }
        #endregion
    }

    /// <summary>
    /// Allows to subscribe for any control to an event triggered when the control's DataContext changes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class FlexibleDataContextChangedHelper<T> where T : FrameworkElement
    {
        private Action<T> callback;

        #region InternalDataContext
        private const string InternalDataContextPropertyName = "InternalDataContext";
        public static readonly DependencyProperty InternalDataContextProperty =
            DependencyProperty.Register(
                InternalDataContextPropertyName,
                typeof(Object),
                typeof(T),
                new PropertyMetadata(InternalDataContextChanged));

        private static void InternalDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            T control = (T)sender;
            var handler = GetFlexibleDataContextChangedHandler(control);

            if (handler != null)
                handler.InvokeCallback(control);
        }
        #endregion

        #region FlexibleDataContextChangedHandler

        /// <summary>
        /// FlexibleDataContextChangedHandler Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty FlexibleDataContextChangedHandlerProperty =
            DependencyProperty.RegisterAttached(
                "FlexibleDataContextChangedHandler",
                typeof(FlexibleDataContextChangedHelper<T>),
                typeof(FlexibleDataContextChangedHelper<T>), //typeof(T), //?
                new PropertyMetadata(null));

        /// <summary>
        /// Gets the FlexibleDataContextChangedHandler property. This dependency property 
        /// indicates the FlexibleDataContextChangedHelper instance responsible for routing the callback that is executed when the control's DataContext changes.
        /// </summary>
        public static FlexibleDataContextChangedHelper<T> GetFlexibleDataContextChangedHandler(DependencyObject d)
        {
            return (FlexibleDataContextChangedHelper<T>)d.GetValue(FlexibleDataContextChangedHandlerProperty);
        }

        /// <summary>
        /// Sets the FlexibleDataContextChangedHandler property. This dependency property 
        /// indicates the FlexibleDataContextChangedHelper instance responsible for routing the callback that is executed when the control's DataContext changes.
        /// </summary>
        public static void SetFlexibleDataContextChangedHandler(DependencyObject d, FlexibleDataContextChangedHelper<T> value)
        {
            d.SetValue(FlexibleDataContextChangedHandlerProperty, value);
        }

        #endregion

        #region Subscribe()
        public static void Subscribe(T control, Action<T> callback)
        {
            Unsubscribe(control);

            control.SetBinding(InternalDataContextProperty, new Binding());
            var handler = new FlexibleDataContextChangedHelper<T>(callback);
            SetFlexibleDataContextChangedHandler(control, handler);
        }
        #endregion

        #region Unsubscribe()
        public static void Unsubscribe(T control)
        {
            control.SetValue(InternalDataContextProperty, null);

            var handler = GetFlexibleDataContextChangedHandler(control);

            if (handler != null)
            {
                handler.callback = null;
                SetFlexibleDataContextChangedHandler(control, null);
            }
        }
        #endregion

        #region CTOR
        private FlexibleDataContextChangedHelper(Action<T> callback)
        {
            this.callback = callback;
        }
        #endregion

        #region InvokeCallback()
        private void InvokeCallback(T control)
        {
            if (this.callback != null)
                this.callback(control);
        }
        #endregion
    }
}