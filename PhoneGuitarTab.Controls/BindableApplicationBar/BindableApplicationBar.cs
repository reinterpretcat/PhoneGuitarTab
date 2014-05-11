using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace PhoneGuitarTab.Controls
{
    /// <summary>
    ///     Serves as a bindable control wrapping the native ApplicationBar
    ///     for use in MVVM applications.
    /// </summary>
    /// <remarks>
    ///     TODO: Find out if bindings can work through ElementName - check for namescope issues.
    ///     TODO: Find out if current/max number of buttons/menu items can be exposed as bindable properties.
    ///     TODO: Figure out the best handling of cases when a button or menu item is added beyond the maximum number
    ///     allowable.
    /// </remarks>
    [ContentProperty("Buttons")]
    public class BindableApplicationBar : Control
    {
        private ApplicationBar applicationBar;
        private PhoneApplicationPage page;

        private bool backgroundColorChanged;
        private bool foregroundColorChanged;
        private bool isMenuEnabledChanged;
        private bool isVisibleChanged;
        private bool modeChanged;
        private bool bindableOpacityChanged;

        private readonly DependencyObjectCollection<BindableApplicationBarButton>
            buttonsSourceButtons =
                new DependencyObjectCollection<BindableApplicationBarButton>();

        private readonly DependencyObjectCollection<BindableApplicationBarMenuItem>
            menuItemsSourceMenuItems =
                new DependencyObjectCollection<BindableApplicationBarMenuItem>();

        #region Buttons

        /// <summary>
        ///     Buttons Dependency Property
        /// </summary>
        public static readonly DependencyProperty ButtonsProperty =
            DependencyProperty.Register(
                "Buttons",
                typeof (DependencyObjectCollection<BindableApplicationBarButton>),
                typeof (BindableApplicationBar),
                new PropertyMetadata(null, OnButtonsChanged));

        /// <summary>
        ///     Gets or sets the Buttons property. This dependency property
        ///     indicates the list of BindableApplicationBarButton objects that
        ///     map to ApplicationBarIconButton generated for the ApplicationBar.
        /// </summary>
        public DependencyObjectCollection<BindableApplicationBarButton> Buttons
        {
            get { return (DependencyObjectCollection<BindableApplicationBarButton>) GetValue(ButtonsProperty); }

            set { SetValue(ButtonsProperty, value); }
        }

        /// <summary>
        ///     Handles changes to the Buttons property.
        /// </summary>
        /// <param name="d">
        ///     The <see cref="DependencyObject" /> on which
        ///     the property has changed value.
        /// </param>
        /// <param name="e">
        ///     Event data that is issued by any event that
        ///     tracks changes to the effective value of this property.
        /// </param>
        private static void OnButtonsChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (BindableApplicationBar) d;
            var oldButtons =
                (DependencyObjectCollection<BindableApplicationBarButton>)
                    e.OldValue;
            var newButtons = target.Buttons;
            target.OnButtonsChanged(oldButtons, newButtons);
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to
        ///     the Buttons property.
        /// </summary>
        /// <param name="oldButtons">The old buttons.</param>
        /// <param name="newButtons">The new buttons.</param>
        protected virtual void OnButtonsChanged(
            DependencyObjectCollection<BindableApplicationBarButton> oldButtons,
            DependencyObjectCollection<BindableApplicationBarButton> newButtons)
        {
            if (oldButtons != null)
            {
                oldButtons.CollectionChanged -= ButtonsCollectionChanged;
            }

            if (newButtons != null)
            {
                newButtons.CollectionChanged += ButtonsCollectionChanged;
            }
        }

        #endregion

        #region MenuItems

        /// <summary>
        ///     MenuItems Dependency Property
        /// </summary>
        public static readonly DependencyProperty MenuItemsProperty =
            DependencyProperty.Register(
                "MenuItems",
                typeof (DependencyObjectCollection<BindableApplicationBarMenuItem>),
                typeof (BindableApplicationBar),
                new PropertyMetadata(null, OnMenuItemsChanged));

        /// <summary>
        ///     Gets or sets the MenuItems property. This dependency property
        ///     indicates the list of BindableApplicationBarMenuItem objects that
        ///     map to ApplicationBarMenuItem generated for the ApplicationBar.
        /// </summary>
        public DependencyObjectCollection<BindableApplicationBarMenuItem> MenuItems
        {
            get { return (DependencyObjectCollection<BindableApplicationBarMenuItem>) GetValue(MenuItemsProperty); }
            set { SetValue(MenuItemsProperty, value); }
        }

        /// <summary>
        ///     Handles changes to the MenuItems property.
        /// </summary>
        /// <param name="d">
        ///     The <see cref="DependencyObject" /> on which
        ///     the property has changed value.
        /// </param>
        /// <param name="e">
        ///     Event data that is issued by any event that
        ///     tracks changes to the effective value of this property.
        /// </param>
        private static void OnMenuItemsChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BindableApplicationBar target = (BindableApplicationBar) d;
            DependencyObjectCollection<BindableApplicationBarMenuItem> oldMenuItems =
                (DependencyObjectCollection<BindableApplicationBarMenuItem>) e.OldValue;
            DependencyObjectCollection<BindableApplicationBarMenuItem> newMenuItems =
                target.MenuItems;
            target.OnMenuItemsChanged(oldMenuItems, newMenuItems);
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to
        ///     the MenuItems property.
        /// </summary>
        /// <param name="oldMenuItems">The old menu items.</param>
        /// <param name="newMenuItems">The new menu items.</param>
        protected virtual void OnMenuItemsChanged(
            DependencyObjectCollection<BindableApplicationBarMenuItem> oldMenuItems,
            DependencyObjectCollection<BindableApplicationBarMenuItem> newMenuItems)
        {
            if (oldMenuItems != null)
            {
                oldMenuItems.CollectionChanged -=
                    MenuItemsCollectionChanged;
            }

            if (newMenuItems != null)
            {
                newMenuItems.CollectionChanged +=
                    MenuItemsCollectionChanged;
            }
        }

        #endregion

        #region ButtonsSource

        /// <summary>
        ///     ButtonsSource Dependency Property
        /// </summary>
        public static readonly DependencyProperty ButtonsSourceProperty =
            DependencyProperty.Register(
                "ButtonsSource",
                typeof (IEnumerable),
                typeof (BindableApplicationBar),
                new PropertyMetadata(null, OnButtonsSourceChanged));

        /// <summary>
        ///     Gets or sets the ButtonsSource property. This dependency property
        ///     indicates the collection from which to build the button objects,
        ///     using the <see cref="ButtonTemplate" /> DataTemplate.
        /// </summary>
        public IEnumerable ButtonsSource
        {
            get { return (IEnumerable) GetValue(ButtonsSourceProperty); }
            set { SetValue(ButtonsSourceProperty, value); }
        }

        /// <summary>
        ///     Handles changes to the ButtonsSource property.
        /// </summary>
        /// <param name="d">
        ///     The <see cref="DependencyObject" /> on which
        ///     the property has changed value.
        /// </param>
        /// <param name="e">
        ///     Event data that is issued by any event that
        ///     tracks changes to the effective value of this property.
        /// </param>
        private static void OnButtonsSourceChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BindableApplicationBar target = (BindableApplicationBar) d;
            IEnumerable oldButtonsSource = (IEnumerable) e.OldValue;
            IEnumerable newButtonsSource = target.ButtonsSource;
            target.OnButtonsSourceChanged(oldButtonsSource, newButtonsSource);
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to
        ///     the ButtonsSource property.
        /// </summary>
        /// <param name="oldButtonsSource">The old buttons source.</param>
        /// <param name="newButtonsSource">The new buttons source.</param>
        protected virtual void OnButtonsSourceChanged(
            IEnumerable oldButtonsSource, IEnumerable newButtonsSource)
        {
            if (oldButtonsSource != null &&
                oldButtonsSource is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged) oldButtonsSource)
                    .CollectionChanged -= ButtonsSourceCollectionChanged;
            }

            GenerateButtonsFromSource();

            if (newButtonsSource != null &&
                newButtonsSource is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged) newButtonsSource)
                    .CollectionChanged += ButtonsSourceCollectionChanged;
            }
        }

        private void ButtonsSourceCollectionChanged(
            object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var buttonSource in e.OldItems)
                {
                    // Copy item reference to prevent access to modified closure
                    var dataContext = buttonSource;
                    var button =
                        buttonsSourceButtons.FirstOrDefault(
                            b => b.DataContext == dataContext);

                    if (button != null)
                    {
                        buttonsSourceButtons.Remove(button);
                    }
                }
            }

            if (ButtonsSource != null &&
                ButtonTemplate != null &&
                e.NewItems != null)
            {
                foreach (var buttonSource in e.NewItems)
                {
                    var button = (BindableApplicationBarButton)
                        ButtonTemplate.LoadContent();

                    if (button == null)
                    {
                        throw new InvalidOperationException(
                            "BindableApplicationBar cannot use the ButtonsSource property without a valid ButtonTemplate");
                    }

                    button.DataContext = buttonSource;
                    buttonsSourceButtons.Add(button);
                }
            }
        }

        #endregion

        #region GenerateButtonsFromSource()

        private void GenerateButtonsFromSource()
        {
            buttonsSourceButtons.Clear();

            if (ButtonsSource != null && ButtonTemplate != null)
            {
                foreach (var buttonSource in ButtonsSource)
                {
                    var button = (BindableApplicationBarButton)
                        ButtonTemplate.LoadContent();

                    if (button == null)
                    {
                        throw new InvalidOperationException(
                            "BindableApplicationBar cannot use the ButtonsSource property without a valid ButtonTemplate");
                    }

                    button.DataContext = buttonSource;
                    buttonsSourceButtons.Add(button);
                }
            }
        }

        #endregion

        #region GenerateMenuItemsFromSource()

        private void GenerateMenuItemsFromSource()
        {
            menuItemsSourceMenuItems.Clear();

            if (MenuItemsSource != null &&
                MenuItemTemplate != null)
            {
                foreach (var menuItemSource in MenuItemsSource)
                {
                    var menuItem = (BindableApplicationBarMenuItem)
                        MenuItemTemplate.LoadContent();

                    if (menuItem == null)
                    {
                        throw new InvalidOperationException(
                            "BindableApplicationBar cannot use the MenuItemsSource property without a valid MenuItemTemplate");
                    }

                    menuItem.DataContext = menuItemSource;
                    menuItemsSourceMenuItems.Add(menuItem);
                }
            }
        }

        #endregion

        #region ButtonTemplate

        /// <summary>
        ///     ButtonTemplate Dependency Property
        /// </summary>
        public static readonly DependencyProperty ButtonTemplateProperty =
            DependencyProperty.Register(
                "ButtonTemplate",
                typeof (DataTemplate),
                typeof (BindableApplicationBar),
                new PropertyMetadata(null, OnButtonTemplateChanged));

        /// <summary>
        ///     Gets or sets the ButtonTemplate property.
        ///     This dependency property indicates the template for a
        ///     <see cref="BindableApplicationBarButton" /> that is used together
        ///     with the <see cref="ButtonsSource" /> collection
        ///     to create the application bar buttons.
        /// </summary>
        /// <remarks>
        ///     The default template defines the property names to match
        ///     the names of properties of a BindableApplicationBarButton.
        /// </remarks>
        public DataTemplate ButtonTemplate
        {
            get { return (DataTemplate) GetValue(ButtonTemplateProperty); }
            set { SetValue(ButtonTemplateProperty, value); }
        }

        /// <summary>
        ///     Handles changes to the ButtonTemplate property.
        /// </summary>
        /// <param name="d">
        ///     The dependency property.
        /// </param>
        /// <param name="e">
        ///     The event arguments.
        /// </param>
        private static void OnButtonTemplateChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BindableApplicationBar target = (BindableApplicationBar) d;
            DataTemplate oldButtonTemplate = (DataTemplate) e.OldValue;
            DataTemplate newButtonTemplate = target.ButtonTemplate;
            target.OnButtonTemplateChanged(
                oldButtonTemplate, newButtonTemplate);
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to
        ///     the ButtonTemplate property.
        /// </summary>
        /// <param name="oldButtonTemplate">The old button template.</param>
        /// <param name="newButtonTemplate">The new button template.</param>
        protected virtual void OnButtonTemplateChanged(
            DataTemplate oldButtonTemplate, DataTemplate newButtonTemplate)
        {
            GenerateButtonsFromSource();
        }

        #endregion

        #region MenuItemsSource

        /// <summary>
        ///     MenuItemsSource Dependency Property
        /// </summary>
        public static readonly DependencyProperty MenuItemsSourceProperty =
            DependencyProperty.Register(
                "MenuItemsSource",
                typeof (IEnumerable),
                typeof (BindableApplicationBar),
                new PropertyMetadata(null, OnMenuItemsSourceChanged));

        /// <summary>
        ///     Gets or sets the MenuItemsSource property.
        ///     This dependency property indicates the collection from which
        ///     to build the MenuItem objects,
        ///     using the <see cref="MenuItemTemplate" /> DataTemplate.
        /// </summary>
        public IEnumerable MenuItemsSource
        {
            get { return (IEnumerable) GetValue(MenuItemsSourceProperty); }
            set { SetValue(MenuItemsSourceProperty, value); }
        }

        /// <summary>
        ///     Handles changes to the MenuItemsSource property.
        /// </summary>
        /// <param name="d">
        ///     The <see cref="DependencyObject" /> on which
        ///     the property has changed value.
        /// </param>
        /// <param name="e">
        ///     Event data that is issued by any event that
        ///     tracks changes to the effective value of this property.
        /// </param>
        private static void OnMenuItemsSourceChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BindableApplicationBar target = (BindableApplicationBar) d;
            IEnumerable oldMenuItemsSource = (IEnumerable) e.OldValue;
            IEnumerable newMenuItemsSource = target.MenuItemsSource;
            target.OnMenuItemsSourceChanged(
                oldMenuItemsSource, newMenuItemsSource);
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to
        ///     the MenuItemsSource property.
        /// </summary>
        /// <param name="oldMenuItemsSource">
        ///     The old MenuItemsSource value.
        /// </param>
        /// <param name="newMenuItemsSource">
        ///     The new MenuItemsSource value.
        /// </param>
        protected virtual void OnMenuItemsSourceChanged(
            IEnumerable oldMenuItemsSource, IEnumerable newMenuItemsSource)
        {
            if (oldMenuItemsSource != null &&
                oldMenuItemsSource is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged) oldMenuItemsSource)
                    .CollectionChanged -=
                    MenuItemsSourceCollectionChanged;
            }

            GenerateMenuItemsFromSource();

            if (newMenuItemsSource != null &&
                newMenuItemsSource is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged) newMenuItemsSource)
                    .CollectionChanged +=
                    MenuItemsSourceCollectionChanged;
            }
        }

        private void MenuItemsSourceCollectionChanged(
            object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var menuItemSource in e.OldItems)
                {
                    // Copy item reference to prevent access to modified closure
                    var dataContext = menuItemSource;
                    var menuItem =
                        menuItemsSourceMenuItems.FirstOrDefault(
                            b => b.DataContext == dataContext);

                    if (menuItem != null)
                    {
                        menuItemsSourceMenuItems.Remove(menuItem);
                    }
                }
            }

            if (MenuItemsSource != null &&
                MenuItemTemplate != null &&
                e.NewItems != null)
            {
                foreach (var menuItemSource in e.NewItems)
                {
                    var menuItem = (BindableApplicationBarMenuItem)
                        MenuItemTemplate.LoadContent();

                    if (menuItem == null)
                    {
                        throw new InvalidOperationException(
                            "BindableApplicationBar cannot use the MenuItemsSource property without a valid MenuItemTemplate");
                    }

                    menuItem.DataContext = menuItemSource;
                    menuItemsSourceMenuItems.Add(menuItem);
                }
            }
        }

        #endregion

        #region MenuItemTemplate

        /// <summary>
        ///     MenuItemTemplate Dependency Property
        /// </summary>
        public static readonly DependencyProperty MenuItemTemplateProperty =
            DependencyProperty.Register(
                "MenuItemTemplate",
                typeof (DataTemplate),
                typeof (BindableApplicationBar),
                new PropertyMetadata(null, OnMenuItemTemplateChanged));

        /// <summary>
        ///     Gets or sets the MenuItemTemplate property.
        ///     This dependency property indicates the template
        ///     for a <see cref="BindableApplicationBarMenuItem" /> that is used
        ///     together with the <see cref="MenuItemsSource" /> collection
        ///     to create the application bar MenuItems.
        /// </summary>
        /// <remarks>
        ///     The default template defines the property names to match
        ///     the names of properties of a BindableApplicationBarMenuItem.
        /// </remarks>
        public DataTemplate MenuItemTemplate
        {
            get { return (DataTemplate) GetValue(MenuItemTemplateProperty); }
            set { SetValue(MenuItemTemplateProperty, value); }
        }

        /// <summary>
        ///     Handles changes to the MenuItemTemplate property.
        /// </summary>
        /// <param name="d">
        ///     The <see cref="DependencyObject" /> on which
        ///     the property has changed value.
        /// </param>
        /// <param name="e">
        ///     Event data that is issued by any event that
        ///     tracks changes to the effective value of this property.
        /// </param>
        private static void OnMenuItemTemplateChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BindableApplicationBar target = (BindableApplicationBar) d;
            DataTemplate oldMenuItemTemplate = (DataTemplate) e.OldValue;
            DataTemplate newMenuItemTemplate = target.MenuItemTemplate;
            target.OnMenuItemTemplateChanged(
                oldMenuItemTemplate, newMenuItemTemplate);
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to
        ///     the MenuItemTemplate property.
        /// </summary>
        /// <param name="oldMenuItemTemplate">
        ///     The old MenuItemTemplate value.
        /// </param>
        /// <param name="newMenuItemTemplate">
        ///     The new MenuItemTemplate value.
        /// </param>
        protected virtual void OnMenuItemTemplateChanged(
            DataTemplate oldMenuItemTemplate,
            DataTemplate newMenuItemTemplate)
        {
            GenerateMenuItemsFromSource();
        }

        #endregion

        #region IsVisible

        /// <summary>
        ///     IsVisible Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register(
                "IsVisible",
                typeof (bool),
                typeof (BindableApplicationBar),
                new PropertyMetadata(true, OnIsVisibleChanged));

        /// <summary>
        ///     Gets or sets a value indicating whether
        ///     the attached ApplicationBar is visible.
        /// </summary>
        public bool IsVisible
        {
            get { return (bool) GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        /// <summary>
        ///     Handles changes to the IsVisible property.
        /// </summary>
        /// <param name="d">
        ///     The <see cref="DependencyObject" /> on which
        ///     the property has changed value.
        /// </param>
        /// <param name="e">
        ///     Event data that is issued by any event that
        ///     tracks changes to the effective value of this property.
        /// </param>
        private static void OnIsVisibleChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BindableApplicationBar target = (BindableApplicationBar) d;
            bool oldIsVisible = (bool) e.OldValue;
            bool newIsVisible = target.IsVisible;
            target.OnIsVisibleChanged(oldIsVisible, newIsVisible);
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to
        ///     the IsVisible property.
        /// </summary>
        /// <param name="oldIsVisible">The old IsVisible value.</param>
        /// <param name="newIsVisible">The new IsVisible value.</param>
        protected virtual void OnIsVisibleChanged(
            bool oldIsVisible, bool newIsVisible)
        {
            if (applicationBar != null)
            {
                applicationBar.IsVisible = newIsVisible;
            }

            isVisibleChanged = true;
        }

        #endregion

        #region IsMenuEnabled

        /// <summary>
        ///     IsMenuEnabled Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsMenuEnabledProperty =
            DependencyProperty.Register(
                "IsMenuEnabled",
                typeof (bool),
                typeof (BindableApplicationBar),
                new PropertyMetadata(true, OnIsMenuEnabledChanged));

        /// <summary>
        ///     Gets or sets a value indicating whether the menu is enabled.
        /// </summary>
        public bool IsMenuEnabled
        {
            get { return (bool) GetValue(IsMenuEnabledProperty); }
            set { SetValue(IsMenuEnabledProperty, value); }
        }

        /// <summary>
        ///     Handles changes to the IsMenuEnabled property.
        /// </summary>
        /// <param name="d">
        ///     The <see cref="DependencyObject" /> on which
        ///     the property has changed value.
        /// </param>
        /// <param name="e">
        ///     Event data that is issued by any event that
        ///     tracks changes to the effective value of this property.
        /// </param>
        private static void OnIsMenuEnabledChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BindableApplicationBar target = (BindableApplicationBar) d;
            bool oldIsMenuEnabled = (bool) e.OldValue;
            bool newIsMenuEnabled = target.IsMenuEnabled;
            target.OnIsMenuEnabledChanged(
                oldIsMenuEnabled, newIsMenuEnabled);
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to
        ///     the IsMenuEnabled property.
        /// </summary>
        /// <param name="oldIsMenuEnabled">
        ///     The old IsMenuEnabled value.
        /// </param>
        /// <param name="newIsMenuEnabled">
        ///     The new IsMenuEnabled value.
        /// </param>
        protected virtual void OnIsMenuEnabledChanged(
            bool oldIsMenuEnabled, bool newIsMenuEnabled)
        {
            if (applicationBar != null)
            {
                applicationBar.IsMenuEnabled = newIsMenuEnabled;
            }

            isMenuEnabledChanged = true;
        }

        #endregion

        #region IsMenuVisible

        /// <summary>
        ///     IsMenuVisible Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsMenuVisibleProperty =
            DependencyProperty.Register(
                "IsMenuVisible",
                typeof (bool),
                typeof (BindableApplicationBar),
                new PropertyMetadata(false, OnIsMenuVisibleChanged));

        /// <summary>
        ///     Gets or sets a value indicating whether the menu is visible.
        /// </summary>
        public bool IsMenuVisible
        {
            get { return (bool) GetValue(IsMenuVisibleProperty); }
            set { SetValue(IsMenuVisibleProperty, value); }
        }

        /// <summary>
        ///     Handles changes to the IsMenuVisible property.
        /// </summary>
        /// <param name="d">
        ///     The <see cref="DependencyObject" /> on which
        ///     the property has changed value.
        /// </param>
        /// <param name="e">
        ///     Event data that is issued by any event that
        ///     tracks changes to the effective value of this property.
        /// </param>
        private static void OnIsMenuVisibleChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BindableApplicationBar target = (BindableApplicationBar) d;
            bool oldIsMenuVisible = (bool) e.OldValue;
            bool newIsMenuVisible = target.IsMenuVisible;
            target.OnIsMenuVisibleChanged(
                oldIsMenuVisible, newIsMenuVisible);
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to
        ///     the IsMenuVisible property.
        /// </summary>
        /// <param name="oldIsMenuVisible">
        ///     The old IsMenuVisible value.
        /// </param>
        /// <param name="newIsMenuVisible">
        ///     The new IsMenuVisible value.
        /// </param>
        protected virtual void OnIsMenuVisibleChanged(
            bool oldIsMenuVisible, bool newIsMenuVisible)
        {
            if (isMenuVisible != newIsMenuVisible)
            {
                // Make sure the property is read-only.
                IsMenuVisible = isMenuVisible;
            }
        }

        /// <summary>
        ///     Used to make sure the dependency property is read-only.
        /// </summary>
        private bool isMenuVisible;

        #endregion

        #region BackgroundColor

        /// <summary>
        ///     BackgroundColor Dependency Property
        /// </summary>
        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register(
                "BackgroundColor",
                typeof (Color),
                typeof (BindableApplicationBar),
                new PropertyMetadata(
                    Colors.Magenta, OnBackgroundColorChanged));

        /// <summary>
        ///     Gets or sets the BackgroundColor property.
        ///     This dependency property indicates the BackgroundColor
        ///     of the ApplicationBar.
        /// </summary>
        public Color BackgroundColor
        {
            get { return (Color) GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        /// <summary>
        ///     Handles changes to the BackgroundColor property.
        /// </summary>
        /// <param name="d">
        ///     The <see cref="DependencyObject" /> on which
        ///     the property has changed value.
        /// </param>
        /// <param name="e">
        ///     Event data that is issued by any event that
        ///     tracks changes to the effective value of this property.
        /// </param>
        private static void OnBackgroundColorChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BindableApplicationBar target = (BindableApplicationBar) d;
            Color oldBackgroundColor = (Color) e.OldValue;
            Color newBackgroundColor = target.BackgroundColor;
            target.OnBackgroundColorChanged(
                oldBackgroundColor, newBackgroundColor);
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to
        ///     the BackgroundColor property.
        /// </summary>
        /// <param name="oldBackgroundColor">
        ///     The old BackgroundColor value.
        /// </param>
        /// <param name="newBackgroundColor">
        ///     The new BackgroundColor value.
        /// </param>
        protected virtual void OnBackgroundColorChanged(
            Color oldBackgroundColor, Color newBackgroundColor)
        {
            if (applicationBar != null)
            {
                applicationBar.BackgroundColor = BackgroundColor;
            }

            backgroundColorChanged = true;
        }

        #endregion

        #region ForegroundColor

        /// <summary>
        ///     ForegroundColor Dependency Property
        /// </summary>
        public static readonly DependencyProperty ForegroundColorProperty =
            DependencyProperty.Register(
                "ForegroundColor",
                typeof (Color),
                typeof (BindableApplicationBar),
                new PropertyMetadata(
                    Colors.Magenta, OnForegroundColorChanged));

        /// <summary>
        ///     Gets or sets the ForegroundColor property. This dependency
        ///     property indicates the ForegroundColor of the ApplicationBar.
        /// </summary>
        public Color ForegroundColor
        {
            get { return (Color) GetValue(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }

        /// <summary>
        ///     Handles changes to the ForegroundColor property.
        /// </summary>
        /// <param name="d">
        ///     The <see cref="DependencyObject" /> on which
        ///     the property has changed value.
        /// </param>
        /// <param name="e">
        ///     Event data that is issued by any event that
        ///     tracks changes to the effective value of this property.
        /// </param>
        private static void OnForegroundColorChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BindableApplicationBar target = (BindableApplicationBar) d;
            Color oldForegroundColor = (Color) e.OldValue;
            Color newForegroundColor = target.ForegroundColor;
            target.OnForegroundColorChanged(
                oldForegroundColor, newForegroundColor);
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to
        ///     the ForegroundColor property.
        /// </summary>
        /// <param name="oldForegroundColor">
        ///     The old ForegroundColor value.
        /// </param>
        /// <param name="newForegroundColor">
        ///     The new ForegroundColor value.
        /// </param>
        protected virtual void OnForegroundColorChanged(
            Color oldForegroundColor, Color newForegroundColor)
        {
            if (applicationBar != null)
            {
                applicationBar.ForegroundColor = ForegroundColor;
            }

            foregroundColorChanged = true;
        }

        #endregion

        #region Mode

        /// <summary>
        ///     Mode Dependency Property
        /// </summary>
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(
                "Mode",
                typeof (ApplicationBarMode),
                typeof (BindableApplicationBar),
                new PropertyMetadata(
                    ApplicationBarMode.Default, OnModeChanged));

        /// <summary>
        ///     Gets or sets the Mode property. This dependency property
        ///     indicates the Mode of the ApplicationBar.
        /// </summary>
        public ApplicationBarMode Mode
        {
            get { return (ApplicationBarMode) GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        /// <summary>
        ///     Handles changes to the Mode property.
        /// </summary>
        /// <param name="d">
        ///     The <see cref="DependencyObject" /> on which
        ///     the property has changed value.
        /// </param>
        /// <param name="e">
        ///     Event data that is issued by any event that
        ///     tracks changes to the effective value of this property.
        /// </param>
        private static void OnModeChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BindableApplicationBar target = (BindableApplicationBar) d;
            ApplicationBarMode oldMode = (ApplicationBarMode) e.OldValue;
            ApplicationBarMode newMode = target.Mode;
            target.OnModeChanged(oldMode, newMode);
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to
        ///     the Mode property.
        /// </summary>
        /// <param name="oldMode">The old Mode value.</param>
        /// <param name="newMode">The new Mode value.</param>
        protected virtual void OnModeChanged(
            ApplicationBarMode oldMode, ApplicationBarMode newMode)
        {
            if (applicationBar != null)
            {
                applicationBar.Mode = newMode;
            }

            modeChanged = true;
        }

        #endregion

        #region BindableOpacity

        /// <summary>
        ///     BindableOpacity Dependency Property
        /// </summary>
        public static readonly DependencyProperty BindableOpacityProperty =
            DependencyProperty.Register(
                "BindableOpacity",
                typeof (double),
                typeof (BindableApplicationBar),
                new PropertyMetadata(1.0, OnBindableOpacityChanged));

        /// <summary>
        ///     Gets or sets the BindableOpacity property.
        ///     This dependency property indicates the Opacity of
        ///     the ApplicationBar.
        ///     The <see cref="UIElement.Opacity" /> property can be used instead,
        ///     since BindableOpacity is only used to allow handling changes to
        ///     <see cref="UIElement.Opacity" />.
        /// </summary>
        public double BindableOpacity
        {
            get { return (double) GetValue(BindableOpacityProperty); }
            set { SetValue(BindableOpacityProperty, value); }
        }

        /// <summary>
        ///     Handles changes to the BindableOpacity property.
        /// </summary>
        /// <param name="d">
        ///     The <see cref="DependencyObject" /> on which
        ///     the property has changed value.
        /// </param>
        /// <param name="e">
        ///     Event data that is issued by any event that
        ///     tracks changes to the effective value of this property.
        /// </param>
        private static void OnBindableOpacityChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BindableApplicationBar target = (BindableApplicationBar) d;
            double oldBindableOpacity = (double) e.OldValue;
            double newBindableOpacity = target.BindableOpacity;
            target.OnBindableOpacityChanged(
                oldBindableOpacity, newBindableOpacity);
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to
        ///     the BindableOpacity property.
        /// </summary>
        /// <param name="oldBindableOpacity">
        ///     The old BindableOpacity value.
        /// </param>
        /// <param name="newBindableOpacity">
        ///     The new BindableOpacity value.
        /// </param>
        protected virtual void OnBindableOpacityChanged(
            double oldBindableOpacity, double newBindableOpacity)
        {
            if (applicationBar != null)
            {
                applicationBar.Opacity = newBindableOpacity;
            }

            bindableOpacityChanged = true;
        }

        #endregion

        #region CTOR

        /// <summary>
        ///     Initializes a new instance of the <see cref="BindableApplicationBar" /> class.
        /// </summary>
        public BindableApplicationBar()
        {
            Buttons = new DependencyObjectCollection<BindableApplicationBarButton>();
            MenuItems = new DependencyObjectCollection<BindableApplicationBarMenuItem>();

            DefaultStyleKey = typeof (BindableApplicationBar);
            Buttons.CollectionChanged +=
                ButtonsCollectionChanged;
            MenuItems.CollectionChanged +=
                MenuItemsCollectionChanged;
            buttonsSourceButtons.CollectionChanged +=
                ButtonsCollectionChanged;
            menuItemsSourceMenuItems.CollectionChanged +=
                MenuItemsCollectionChanged;
            SetBinding(
                BindableOpacityProperty,
                new Binding("Opacity")
                {
                    RelativeSource =
                        new RelativeSource(RelativeSourceMode.Self)
                });
        }

        #endregion

        #region Attach()

        /// <summary>
        ///     Attaches the BindableApplicationBar to the specified page,
        ///     creating the ApplicationBar if required and adding the buttons
        ///     and menu items specified in the Buttons and MenuItems properties.
        /// </summary>
        /// <param name="parentPage">The parentPage to attach to.</param>
        public void Attach(PhoneApplicationPage parentPage)
        {
            page = parentPage;
            applicationBar =
                (ApplicationBar) ( //parentPage.ApplicationBar ?? 
                    (parentPage.ApplicationBar = new ApplicationBar()));
            applicationBar.StateChanged +=
                ApplicationBarStateChanged;

            if (GetBindingExpression(DataContextProperty) == null &&
                DataContext == null)
            {
                SetBinding(
                    DataContextProperty,
                    new Binding("DataContext") {Source = page});
            }

            SynchronizeProperties();
            AttachButtons(Buttons);
            AttachButtons(buttonsSourceButtons);
            AttachMenuItems(MenuItems);
            AttachMenuItems(menuItemsSourceMenuItems);
        }

        #endregion

        #region SynchronizeProperties()

        private void SynchronizeProperties()
        {
            if (isVisibleChanged)
            {
                applicationBar.IsVisible = IsVisible;
            }
            else if (GetBindingExpression(IsVisibleProperty) == null)
            {
                IsVisible = applicationBar.IsVisible;
            }

            if (isMenuEnabledChanged)
            {
                applicationBar.IsMenuEnabled = IsMenuEnabled;
            }
            else if (GetBindingExpression(IsMenuEnabledProperty) == null)
            {
                IsMenuEnabled = applicationBar.IsMenuEnabled;
            }

            if (backgroundColorChanged)
            {
                applicationBar.BackgroundColor = BackgroundColor;
            }
            else if (GetBindingExpression(BackgroundColorProperty) == null)
            {
                BackgroundColor = applicationBar.BackgroundColor;
            }

            if (foregroundColorChanged)
            {
                applicationBar.ForegroundColor = ForegroundColor;
            }
            else if (GetBindingExpression(ForegroundColorProperty) == null)
            {
                ForegroundColor = applicationBar.ForegroundColor;
            }

            if (modeChanged)
            {
                applicationBar.Mode = Mode;
            }
            else if (GetBindingExpression(ModeProperty) == null)
            {
                Mode = applicationBar.Mode;
            }

            if (bindableOpacityChanged)
            {
                applicationBar.Opacity = BindableOpacity;
            }
            else if (GetBindingExpression(BindableOpacityProperty) == null)
            {
                BindableOpacity = applicationBar.Opacity;
            }
        }

        #endregion

        #region AttachButtons()

        private void AttachButtons(
            IEnumerable<BindableApplicationBarButton> buttons)
        {
            int i = applicationBar.Buttons.Count;

            foreach (var button in buttons)
            {
                button.Attach(applicationBar, i++);

                if (button.GetBindingExpression(
                    FrameworkElement.DataContextProperty) == null &&
                    button.DataContext == null)
                {
                    button.SetBinding(
                        FrameworkElement.DataContextProperty,
                        new Binding("DataContext") {Source = this});
                }
            }
        }

        #endregion

        #region AttachMenuItems()

        private void AttachMenuItems(
            IEnumerable<BindableApplicationBarMenuItem> menuItems)
        {
            int j = applicationBar.MenuItems.Count;

            foreach (var menuItem in menuItems)
            {
                menuItem.Attach(applicationBar, j++);

                if (menuItem.GetBindingExpression(
                    FrameworkElement.DataContextProperty) == null &&
                    menuItem.DataContext == null)
                {
                    menuItem.SetBinding(
                        FrameworkElement.DataContextProperty,
                        new Binding("DataContext") {Source = this});
                }
            }
        }

        #endregion

        #region Detach()

        /// <summary>
        ///     Detaches from the specified page, removing all the buttons
        ///     and menu items specified in the Buttons and MenuItems properties.
        /// </summary>
        /// <remarks>
        ///     Note: The code in this method has not been tested and is likely
        ///     not to work properly, will possibly raise exceptions
        ///     and leak memory.
        /// </remarks>
        /// <param name="parentPage">The parentPage.</param>
        public void Detach(PhoneApplicationPage parentPage)
        {
            if (parentPage != page)
            {
                throw new InvalidOperationException();
            }

            if (page != parentPage)
            {
                return;
            }

            var binding = GetBindingExpression(DataContextProperty);

            if (binding != null &&
                binding.ParentBinding.Source == page)
            {
                DataContext = null;
            }

            DetachButtons(buttonsSourceButtons);
            DetachButtons(Buttons);
            DetachMenuItems(menuItemsSourceMenuItems);
            DetachMenuItems(MenuItems);

            applicationBar.StateChanged -=
                ApplicationBarStateChanged;
            applicationBar = null;
        }

        #endregion

        #region DetachButtons()

        private void DetachButtons(
            IEnumerable<BindableApplicationBarButton> buttons)
        {
            foreach (var button in buttons)
            {
                button.Detach();

                var binding = button.GetBindingExpression(
                    FrameworkElement.DataContextProperty);

                if (binding != null &&
                    binding.ParentBinding.Source == this)
                {
                    DataContext = null;
                }
            }
        }

        #endregion

        #region DetachMenuItems()

        private void DetachMenuItems(
            IEnumerable<BindableApplicationBarMenuItem> menuItems)
        {
            foreach (var menuItem in menuItems)
            {
                menuItem.Detach();

                var binding = menuItem.GetBindingExpression(
                    FrameworkElement.DataContextProperty);

                if (binding != null &&
                    binding.ParentBinding.Source == this)
                {
                    DataContext = null;
                }
            }
        }

        #endregion

        #region AttachButton()

        private void AttachButton(BindableApplicationBarButton button, int i)
        {
            if (button.GetBindingExpression(
                FrameworkElement.DataContextProperty) == null &&
                button.GetValue(
                    FrameworkElement.DataContextProperty) == null)
            {
                button.DataContext = DataContext;
            }

            button.Attach(applicationBar, i);
        }

        #endregion

        #region DetachButton()

        private void DetachButton(BindableApplicationBarButton button)
        {
            if (button.GetBindingExpression(DataContextProperty) == null &&
                button.GetValue(DataContextProperty) == DataContext)
            {
                button.DataContext = null;
            }

            button.Detach();
        }

        #endregion

        #region AttachMenuItem()

        private void AttachMenuItem(
            BindableApplicationBarMenuItem menuItem, int i)
        {
            if (menuItem.GetBindingExpression(
                FrameworkElement.DataContextProperty) == null &&
                menuItem.GetValue(
                    FrameworkElement.DataContextProperty) == null)
            {
                menuItem.DataContext = DataContext;
            }

            menuItem.Attach(applicationBar, i);
        }

        #endregion

        #region DetachMenuItem()

        private void DetachMenuItem(BindableApplicationBarMenuItem menuItem)
        {
            if (menuItem.GetBindingExpression(DataContextProperty) == null &&
                menuItem.GetValue(DataContextProperty) == DataContext)
            {
                menuItem.DataContext = null;
            }

            menuItem.Detach();
        }

        #endregion

        #region ApplicationBarStateChanged()

        private void ApplicationBarStateChanged(
            object sender, ApplicationBarStateChangedEventArgs e)
        {
            isMenuVisible = e.IsMenuVisible;
            IsMenuVisible = isMenuVisible;
        }

        #endregion

        #region ButtonsCollectionChanged()

        private void ButtonsCollectionChanged(
            object sender, NotifyCollectionChangedEventArgs e)
        {
            if (applicationBar == null)
            {
                return;
            }

            if (e.OldItems != null)
            {
                foreach (var oldItem in e.OldItems)
                {
                    DetachButton((BindableApplicationBarButton) oldItem);
                }
            }

            if (e.NewItems != null)
            {
                int i = 0;

                foreach (var newItem in e.NewItems)
                {
                    AttachButton(
                        (BindableApplicationBarButton) newItem,
                        e.NewStartingIndex + i++);
                }
            }
        }

        #endregion

        #region MenuItemsCollectionChanged()

        private void MenuItemsCollectionChanged(
            object sender, NotifyCollectionChangedEventArgs e)
        {
            if (applicationBar == null)
            {
                return;
            }

            if (e.OldItems != null)
            {
                foreach (var oldItem in e.OldItems)
                {
                    DetachMenuItem(
                        (BindableApplicationBarMenuItem) oldItem);
                }
            }

            if (e.NewItems != null)
            {
                int i = 0;

                foreach (var newItem in e.NewItems)
                {
                    AttachMenuItem(
                        (BindableApplicationBarMenuItem) newItem,
                        e.NewStartingIndex + i++);
                }
            }
        }

        #endregion
    }
}