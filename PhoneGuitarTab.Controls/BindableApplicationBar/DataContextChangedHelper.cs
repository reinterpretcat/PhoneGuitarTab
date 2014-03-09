using System;
using System.Windows;
using System.Windows.Data;

namespace PhoneGuitarTab.Controls
{
    public interface IDataContextChangedHandler<in T> where T : FrameworkElement
    {
        void DataContextChanged(T sender, DependencyPropertyChangedEventArgs e);
    }

    /// <summary>
    /// Allows controls that implement IDataContextChanged to subscribe to an event triggered when their DataContext changes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class DataContextChangedHelper<T> where T : FrameworkElement, IDataContextChangedHandler<T>
    {
        private const string InternalDataContextPropertyName = "InternalDataContext";

        public static readonly DependencyProperty InternalDataContextProperty =
            DependencyProperty.Register(InternalDataContextPropertyName,
                                        typeof(Object),
                                        typeof(T),
                                        new PropertyMetadata(InternalDataContextChanged));

        private static void InternalDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            T control = (T)sender;

            // Invoking IDataContextChangedHandler.DataContextChanged
            control.DataContextChanged(control, e);
        }

        /// <summary>
        /// Subscribes a control that implements IDataContextChangedHandler to receive DataContextChange events.
        /// </summary>
        /// <param name="control"></param>
        public static void SubscribeToDataContextChanged(T control)
        {
            // Binding to the control itself allows the control to find when its DataContext changes.
            control.SetBinding(InternalDataContextProperty, new Binding());
        }
    }
}
