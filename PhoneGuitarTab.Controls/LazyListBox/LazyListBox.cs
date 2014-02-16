using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.ComponentModel;
using PhoneGuitarTab.Controls.Extensions;
using System.Diagnostics;

namespace PhoneGuitarTab.Controls
{
  /// <summary>
  /// A list box that lazily loads items
  /// </summary>
  public class LazyListBox : ListBox, ISupportOffsetChanges
  {
    #region Fields

    /// <summary>
    /// A simple version number used for checking validity of cached visibility state
    /// </summary>
    internal int VisibleSnapshotVersion { get; private set; }

    /// <summary>
    /// Whether or not the control's visual tree is loaded
    /// </summary>
    bool visualTreeCreated = false;

    /// <summary>
    /// Whether the offset has been changed programmatically, which will require
    /// us to re-compute visible items
    /// </summary>
    double targetScrollOffset = double.NaN;

    /// <summary>
    /// Whether the items have already been drawn due to programmatic list offset
    /// changes, and therefore don't need to be drawn again. Performance improvement
    /// </summary>
    bool visibleItemsComputed = false;

    /// <summary>
    /// Whether or not a deferred request to run OnListChanged has already been made
    /// </summary>
    bool alreadyDeferredListChanged = false;

    /// <summary>
    /// The items that are currently visible in the list; only valid once it has stopped scrolling
    /// </summary>
    List<LazyListBoxItem> visibleItems = null;

    /// <summary>
    /// The items that are not visible in the list; only valid once it has stopped scrolling
    /// </summary>
    List<LazyListBoxItem> invisibleItems = null;

    /// <summary>
    /// The stack panel in the list box
    /// </summary>
    VirtualizingStackPanel stackPanel;

    /// <summary>
    /// Scroll viewer inside the list box
    /// </summary>
    ScrollViewer scrollViewer = null;

    #endregion

    #region IsScrolling property and events

    /// <summary>
    /// Name of the visual state for when the list is scrolling
    /// </summary>
    const string ScrollingVisualStateName = "Scrolling";

    /// <summary>
    /// Name of the visual state group that has the scroll states in it
    /// </summary>
    const string ScrollingVisualStateGroupName = "ScrollStates";

    /// <summary>
    /// The visual state group that tells us when the scrollviewer is scrolling (or not)
    /// </summary>
    VisualStateGroup scrollViewerVisualStateGroup = null;

    /// <summary>
    /// The property is normally read-only, but we need to enable ourselves to write to it
    /// </summary>
    bool allowChangesToIsScrolling = false;

    /// <summary>
    /// The event people can subscribe to
    /// </summary>
    public event EventHandler<ScrollingStateChangedEventArgs> ScrollingStateChanged;

    /// <summary>
    /// DependencyProperty that backs the <see cref="IsScrolling"/> property
    /// </summary>
    public static readonly DependencyProperty IsScrollingProperty = DependencyProperty.Register(
      "IsScrolling",
      typeof(bool),
      typeof(LazyListBox),
      new PropertyMetadata(false, IsScrollingPropertyChanged));

    /// <summary>
    /// Whether the list is currently scrolling or not
    /// </summary>
    public bool IsScrolling
    {
      get
      {
        return (bool)GetValue(IsScrollingProperty);
      }

      internal set
      {
        // "Unlock" the ability to set the property
        allowChangesToIsScrolling = true;
        SetValue(IsScrollingProperty, value);
        allowChangesToIsScrolling = false;
      }
    }

    /// <summary>
    /// Handler for when the IsScrolling dependency property changes
    /// </summary>
    /// <param name="source">The object that has the property</param>
    /// <param name="e">Args</param>
    static void IsScrollingPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
    {
      LazyListBox listbox = source as LazyListBox;

      // The property is supposed to be read-only, although we are allowed to change it ourselves
      if (listbox.allowChangesToIsScrolling != true)
      {
        listbox.IsScrolling = (bool)e.OldValue;
        throw new InvalidOperationException("IsScrolling property is read-only");
      }

      if ((bool)e.NewValue == true)
      {
        // Ask all the items in the list to pause what they're doing
        foreach (LazyListBoxItem item in listbox.visibleItems)
          item.Pause();
      }

      // Call the virtual notification method for anyone who derives from this class
      ScrollingStateChangedEventArgs scrollingArgs = new ScrollingStateChangedEventArgs((bool)e.OldValue, (bool)e.NewValue);
      listbox.OnScrollingStateChanged(scrollingArgs);

      // Raise the event, if anyone is listening to it
      var handler = listbox.ScrollingStateChanged;
      if (handler != null)
        handler(listbox, scrollingArgs);

      // If the list has stopped scrolling, recompute the visible items on the next tick
      if ((bool)e.NewValue == false)
      {
#if ONLISTCHANGESCOMPLETE_LOGGING
        Debug.WriteLine("IsScrollingPropertyChanged calling DeferredOnListChangesComplete because scrolling just finished");
#endif
        listbox.DeferredOnListChangesComplete();
      }
    }

    /// <summary>
    /// Notification that the scrolling state has changed
    /// </summary>
    /// <param name="e">Scrolling parameters</param>
    /// <remarks>
    /// The default implementation does nothing
    /// </remarks>
    protected virtual void OnScrollingStateChanged(ScrollingStateChangedEventArgs e)
    {
    }

    /// <summary>
    /// Hook the scroll-change events from the underlying ScrollViewer
    /// </summary>
    void HookScrollingEvents()
    {
      // This will crash if called in design mode ;-)
      if (DesignerProperties.IsInDesignTool)
        return;

      // Visual States are always on the first child of the control template
      FrameworkElement element = null;
      try
      {
        element = scrollViewer.GetVisualChild(0);
      }
      catch { }

      // No events to hook
      if (element == null)
        return;

      // Get the visual state group that reports scrolling state changes, and hook its event
      scrollViewerVisualStateGroup = element.GetVisualStateGroup(ScrollingVisualStateGroupName);
      if (scrollViewerVisualStateGroup != null)
        scrollViewerVisualStateGroup.CurrentStateChanging += ScrollingStateChanging;
    }

    /// <summary>
    /// Unhook the scroll-change events from the underlying ScrollViewer
    /// </summary>
    void UnhookScrollingEvents()
    {
      // This will crash if called in design mode ;-)
      if (DesignerProperties.IsInDesignTool)
        return;

      if (scrollViewerVisualStateGroup != null)
        scrollViewerVisualStateGroup.CurrentStateChanging -= ScrollingStateChanging;
    }

    /// <summary>
    /// Simple handler called with the ScrollState visual state changes
    /// </summary>
    /// <param name="sender">The visual state group</param>
    /// <param name="e">The new value</param>
    /// <remarks>
    /// This simply updates the IsScrolling property based on the name of the new group
    /// </remarks>
    void ScrollingStateChanging(object sender, VisualStateChangedEventArgs e)
    {
      IsScrolling = (e.NewState.Name == ScrollingVisualStateName);
    }

    #endregion

    #region LoadedItemTemplate property

    /// <summary>
    /// DependencyProperty that backs the <see cref="LoadedItemTemplate"/> property
    /// </summary>
    public static readonly DependencyProperty LoadedItemTemplateProperty = DependencyProperty.Register(
      "LoadedItemTemplate",
      typeof(DataTemplate),
      typeof(LazyListBox),
      null);

    /// <summary>
    /// The "full" template to use when an item is fully loaded and visible in the list
    /// </summary>
    public DataTemplate LoadedItemTemplate
    {
      get { return (DataTemplate)GetValue(LoadedItemTemplateProperty); }
      set { SetValue(LoadedItemTemplateProperty, value); }
    }
    #endregion

    #region CachedItemTemplate property

    /// <summary>
    /// DependencyProperty that backs the <see cref="CachedItemTemplate"/> property
    /// </summary>
    public static readonly DependencyProperty CachedItemTemplateProperty = DependencyProperty.Register(
      "CachedItemTemplate",
      typeof(DataTemplate),
      typeof(LazyListBox),
      null);

    /// <summary>
    /// The item template to use for cached items (those that have previously been loaded, but are no longer visible)
    /// </summary>
    public DataTemplate CachedItemTemplate
    {
      get { return (DataTemplate)GetValue(CachedItemTemplateProperty); }
      set { SetValue(CachedItemTemplateProperty, value); }
    }
    #endregion

    /// <summary>
    /// Creates a new instance of the LazyListBox class
    /// </summary>
    public LazyListBox()
    {
      Loaded += new RoutedEventHandler(LazyListBox_Loaded);
      Unloaded += new RoutedEventHandler(LazyListBox_Unloaded);
    }

    /// <summary>
    /// Called when the listbox is added to the visual tree
    /// </summary>
    /// <param name="sender">The listbox</param>
    /// <param name="e">Args</param>
    /// <remarks>
    /// This event is used to trigger a re-computation of visible items
    /// </remarks>
    void LazyListBox_Loaded(object sender, RoutedEventArgs e)
    {
#if ONLISTCHANGESCOMPLETE_LOGGING
      Debug.WriteLine("Loaded calling OnListChangesComplete because the control just loaded");
#endif
      OnListChangesComplete();
    }

    /// <summary>
    /// Called when the listbox is removed from the visual tree
    /// </summary>
    /// <param name="sender">The listbox</param>
    /// <param name="e">Args</param>
    /// <remarks>
    /// This event is used to notify all items that they are no longer visible
    /// </remarks>
    void LazyListBox_Unloaded(object sender, RoutedEventArgs e)
    {
      visibleItemsComputed = false;

      if (DesignerProperties.IsInDesignTool)
        return;

      // Notify all the items that they are no longer visible
      foreach (LazyListBoxItem item in visibleItems)
        item.SetIsVisible(false, VisibleSnapshotVersion);
    }

    /// <summary>
    /// Returns an enumerator of all the currently-visible items in the list
    /// </summary>
    /// <returns>The visible items</returns>
    public IEnumerable<LazyListBoxItem> GetVisibleItems()
    {
      // Return a non-mutable version
      return GetVisibleItemsAsMutableList().AsEnumerable<LazyListBoxItem>();
    }

    /// <summary>
    /// Returns a mutable list of the visible items
    /// </summary>
    /// <returns>The visible items</returns>
    /// <remarks>
    /// Since this is a list that can be modified, it is only used internally; 
    /// public callers can only get an enumerator via <see cref="GetVisibeItems"/>
    /// </remarks>
    internal List<LazyListBoxItem> GetVisibleItemsAsMutableList()
    {
      if (!visibleItemsComputed)
        ComputeVisibleItems();

      return visibleItems;
    }

    /// <summary>
    /// Update internal state once the list has finished changing (scrolling, items added or
    /// removed, etc.)
    /// </summary>
    /// <remarks>
    /// This method is an expensive method to call, so it avoids doing any work if it believes
    /// there is nothing to do yet, or that it will be called again very soon to compute a
    /// different set of items (eg, becuase the list just got scrolled programmatically)
    /// You can define the ONLISTCHANGESCOMPLETE_LOGGING macro to see debug spew about
    /// who is calling this method, and why.
    /// </remarks>
    void OnListChangesComplete()
    {
#if ONLISTCHANGESCOMPLETE_LOGGING
      Debug.WriteLine("OnListChangesComplete running...");
#endif

      // No work to do
      if (visibleItemsComputed)
      {
#if ONLISTCHANGESCOMPLETE_LOGGING
        Debug.WriteLine("    ...and it is BAILING because it believes there is nothing to do...");
#endif
        return;
      }

      // We are going to be called again very soon to scroll to a new offset, so no point in
      // calculating visibility for the current offset. Instead we defer to the next tick
      if (double.IsNaN(targetScrollOffset) == false && targetScrollOffset != scrollViewer.VerticalOffset)
      {
#if ONLISTCHANGESCOMPLETE_LOGGING
        Debug.WriteLine("    ...and it is calling DeferredOnListChangesComplete because the scroll offset is wrong (current=" + scrollViewer.VerticalOffset + ", expected=" + targetScrollOffset);
#endif

        DeferredOnListChangesComplete();
        return;
      }

      // This is an expensive operation!!!
      Debug.WriteLine("OnListChangesComplete is running... this is EXPENSIVE and should only happen at load time and when scrolling finishes");

      ComputeVisibleItems();

      // if all items are invisible, try again on the next tick.
      if (visibleItems.Count == 0 && invisibleItems.Count > 0)
      {
#if ONLISTCHANGESCOMPLETE_LOGGING
        Debug.WriteLine("    ...and it calling DeferredOnListChangesComplete because there were no visible items found");
#endif

        DeferredOnListChangesComplete();
        return;
      }

      // Notify all visible items of their visibility
      foreach (LazyListBoxItem item in visibleItems)
        item.SetIsVisible(true, VisibleSnapshotVersion);

      // Notify all invisible items of their invisibility
      foreach (LazyListBoxItem item in invisibleItems)
        item.SetIsVisible(false, VisibleSnapshotVersion);

      // No need to do this work again, until something changes
      visibleItemsComputed = true;
    }

    /// <summary>
    /// Compute the visible (and invisible!) items in the list.
    /// </summary>
    void ComputeVisibleItems()
    {
      List<LazyListBoxItem> before = null;
      List<LazyListBoxItem> visible = null;
      List<LazyListBoxItem> after = null;

      // This is where the magic happens...
      stackPanel.GetVisualDescendents<LazyListBoxItem>(true).GetVisibleItems(scrollViewer, stackPanel.Orientation, out before, out visible, out after);

      visibleItems = visible;
      invisibleItems = before.Concat(after).ToList();

      // Update the "version" so that items know if their cached visibility state is valid or not
      VisibleSnapshotVersion++;
    }

    /// <summary>
    /// Standard listbox method to recycle items
    /// </summary>
    /// <param name="element">The LazyListBoxItem being recycled</param>
    /// <param name="item">The old item that's about to get thrown out</param>
    protected override void ClearContainerForItemOverride(DependencyObject element, object item)
    {
      LazyListBoxItem listBoxItem = (element as LazyListBoxItem);

#if false
      // Enable this if you are having issues with recycling
      Debug.WriteLine("Clearing container " + listBoxItem.Id + " for item " + item.ToString());
#endif

      // If it's a special ILazyDataItem, tell it to unload
      ILazyDataItem lazyItem = item as ILazyDataItem;
      if (lazyItem != null)
        lazyItem.GoToState(LazyDataLoadState.Unloaded);

      base.ClearContainerForItemOverride(element, item);
    }

    /// <summary>
    /// Standard method for recycling containers
    /// </summary>
    /// <param name="element">The LazyListBoxItem being prepared for a new object</param>
    /// <param name="item">The item that's about to be put into the container</param>
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      LazyListBoxItem listBoxItem = (element as LazyListBoxItem);

#if false
      // Enable this if you are having issues with container recycling
      Debug.WriteLine("Preparing container " + listBoxItem.Id + " for item " + item.ToString());
#endif

      base.PrepareContainerForItemOverride(element, item);

      // If it's a special ILazyDataItem, tell it to go to the minimum loaded state
      ILazyDataItem lazyItem = item as ILazyDataItem;
      if (lazyItem != null && lazyItem.CurrentState == LazyDataLoadState.Unloaded)
        lazyItem.GoToState(LazyDataLoadState.Minimum);

      // Reset to the basic item template
      listBoxItem.ContentTemplate = ItemTemplate;
    }

    /// <summary>
    /// Standard control method to apply the template
    /// </summary>
    public override void OnApplyTemplate()
    {
      // Clean up any existing template items
      UnhookScrollingEvents();
      scrollViewer = null;

      // Even though the template is applied, the full visual tree doesn't exist yet, so anyone
      // that relies on it should not do any work
      visualTreeCreated = false;

      // Allow listbox to apply its template
      base.OnApplyTemplate();

      // Verify that this is in fact a proper listbox that can scroll
      scrollViewer = GetTemplateChild("ScrollViewer") as ScrollViewer;
      if (scrollViewer == null)
        throw new InvalidOperationException("LazyListBox can only be used with a proper ListBox template");
    }

    /// <summary>
    /// Helper method to call OnListChangesComplete on the next tick
    /// </summary>
    /// <remarks>
    /// Many operations need the list to "settle down" before they can do meaningful work. This method
    /// does the dispatch and also avoids multiple (redundant) dispatches from different callers.
    /// </remarks>
    void DeferredOnListChangesComplete()
    {
      // Already going to do the work... no need to do it again
      if (alreadyDeferredListChanged)
      {
#if ONLISTCHANGESCOMPLETE_LOGGING
        Debug.WriteLine("DeferredOnListChangesComplete ignored a redundant request to defer loading");
#endif
        return;
      }

      alreadyDeferredListChanged = true;

      // Mark items as dirty
      visibleItemsComputed = false;

      Dispatcher.BeginInvoke(delegate
      {
        OnListChangesComplete();

        // Important to reset this AFTER calling OnListChangesComplete so that it doesn't run forever if 
        // OnListChangesComplete calls DeferredOnListChangesComplete for any reason
        alreadyDeferredListChanged = false;
      }
      );
    }

    /// <summary>
    /// Standard control override
    /// </summary>
    /// <param name="finalSize">Size to arrange items for</param>
    /// <returns>Final size</returns>
    /// <remarks>
    /// This method is used to complete hookup of the visual tree (which cannot be done in OnApplyTemplate).
    /// It also triggers a refresh of the visible items if we have been programmatically scrolled
    /// </remarks>
    protected override Size ArrangeOverride(Size finalSize)
    {
      // Let the base compute the actual size... we don't care about that
      Size size = base.ArrangeOverride(finalSize);

      // Hook up to the visual tree
      CompleteVisualSetup();

      // If we have a programmatically-set scroll offset, we need to recompute list changes
      if (double.IsNaN(targetScrollOffset) == false)
      {

#if ONLISTCHANGESCOMPLETE_LOGGING
        Debug.WriteLine("ArrangeOverride calling OnListChangesComplete because target offset is not NaN");
#endif

        OnListChangesComplete();

        // No need to do this again
        targetScrollOffset = double.NaN;
      }

      // size computed by the base class
      return size;
    }

    /// <summary>
    /// Finish setting up the internal state, once the visual tree has been created
    /// </summary>
    void CompleteVisualSetup()
    {
      if (visualTreeCreated)
        return;

      // Hook the scrolling events of the scrollviewer, which will now be created
      HookScrollingEvents();

      // This only works if you have a virtualizing stack panel (ie, you are using binding with an item template)
      stackPanel = scrollViewer.GetVisualDescendents<VirtualizingStackPanel>(true).FirstOrDefault();
      if (stackPanel == null)
        throw new InvalidOperationException("LazyListBox can only be used with a virtualized list; are you missing an ItemTemplate?");

      visualTreeCreated = true;
    }

    /// <summary>
    /// Standard ListBox method to return item containers (which are always LazyListBoxItems for us)
    /// </summary>
    /// <returns>Returns a new LazyListBoxItem</returns>
    protected override DependencyObject GetContainerForItemOverride()
    {
      LazyListBoxItem item = new LazyListBoxItem();
      item.Owner = this;
      if (ItemContainerStyle != null)
        item.Style = ItemContainerStyle;

      return item;
    }

    /// <summary>
    /// Standard ListBox method to see if an item needs to be 'wrapped' in a container or not
    /// </summary>
    /// <param name="item">The item to check</param>
    /// <returns>true if the item is a LazyListBoxItem</returns>
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return (item is LazyListBoxItem);
    }

    /// <summary>
    /// Standard ListBox method called whenever the underlying list changes
    /// </summary>
    /// <param name="e">Info about the changed items</param>
    protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      base.OnItemsChanged(e);

      // Can't do anything if we don't have a visual tree yet
      if (visualTreeCreated)
      {
#if ONLISTCHANGESCOMPLETE_LOGGING
        Debug.WriteLine("OnItemsChanged calling DeferredOnListChangesComplete because the items in the list just changed");
#endif
        DeferredOnListChangesComplete();
      }
    }

    /// <summary>
    /// Gets the container that is currently hosting the item, or null if it isn't virtualized
    /// </summary>
    /// <param name="itemIndex">The item to get the container for</param>
    /// <returns>The LazyListBoxItem that holds the item, or null if it is not currently virtualized</returns>
    public LazyListBoxItem GetContainerForItem(int itemIndex)
    {
      return (stackPanel.ItemContainerGenerator as ItemContainerGenerator).ContainerFromIndex(itemIndex) as LazyListBoxItem;
    }

    /// <summary>
    /// Gets the index of the item housed by the given container
    /// </summary>
    /// <param name="container">The container to check</param>
    /// <returns>The index of the item that is housed in this container</returns>
    public int GetItemIndexFromContainer(LazyListBoxItem container)
    {
      return (stackPanel.ItemContainerGenerator as ItemContainerGenerator).IndexFromContainer(container);
    }

    #region ISupportOffsetChanges Members

    /// <summary>
    /// Notification that the offset has changed programmatically
    /// </summary>
    /// <param name="offset">The new offset</param>
    public void HorizontalOffsetChanged(double offset)
    {
      // Don't support horizontal lists right now
     // throw new NotSupportedException("Horizontal scrolling is not implemented");
        if (stackPanel.Orientation == Orientation.Horizontal)
        {
            visibleItemsComputed = false;
            targetScrollOffset = offset;
            InvalidateArrange();
        }
    }

    /// <summary>
    /// Notification that the offset has changed programmatically
    /// </summary>
    /// <param name="offset">The new offset</param>
    public void VerticalOffsetChanged(double offset)
    {
      // Set the targetOffset; the next ArrangeOverride will pick this up and compute
      // the newly visible items
      if (stackPanel.Orientation == Orientation.Vertical)
      {
        visibleItemsComputed = false;
        targetScrollOffset = offset;
        InvalidateArrange();
      }

      return;
    }

    #endregion
  }
}
