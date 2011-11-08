using System;
using System.Windows.Controls;

namespace PhoneGuitarTab.Controls
{
  /// <summary>
  /// Container used to hold items in a LazyListBox
  /// </summary>
  public class LazyListBoxItem : ListBoxItem
  {
    /// <summary>
    /// Used to check validity of the cached visibility state
    /// </summary>
    int visibleSnapshotVersion = -1;

    /// <summary>
    /// The LazyListBox that owns this item
    /// </summary>
    internal LazyListBox Owner { get; set; }

    /// <summary>
    /// Cached value of whether the item is visible or not
    /// </summary>
    bool cachedIsVisible = false;

    /// <summary>
    /// Whether or not the item is visible
    /// </summary>
    /// <remarks>
    /// An item that isn't virtualized is by definition not visible. Items that are virtualized may
    /// still be invisible if they are scrolled off the screen
    /// </remarks>
    public bool IsVisible
    {
      get
      {
        // Can't be visible if we have no owner
        if (Owner == null)
          return false;

        // If the cached value is not out of date, return it
        if (visibleSnapshotVersion == Owner.VisibleSnapshotVersion)
          return cachedIsVisible;

        // Otherwise, check if we exist in the visible items list
        if (Owner.GetVisibleItemsAsMutableList().Contains(this))
          cachedIsVisible = true;
        else
          cachedIsVisible = false;

        // Remember the current version for next time
        visibleSnapshotVersion = Owner.VisibleSnapshotVersion;
        return cachedIsVisible;
      }
    }

    /// <summary>
    /// Forces the item to be visible or invisible
    /// </summary>
    /// <param name="isVisible">True if the item is visible, false otherwise</param>
    /// <param name="currentSnapshotVersion">Version information used for cache purposes</param>
    internal void SetIsVisible(bool isVisible, int currentSnapshotVersion)
    {
      // We have special behaviour if the item is an ILazyDataItem
      ILazyDataItem lazyItem = DataContext as ILazyDataItem;

      cachedIsVisible = isVisible;
      visibleSnapshotVersion = currentSnapshotVersion;

      if (isVisible)
      {
        // If it's a special item, tell it to change states based on its current state
        if (lazyItem != null)
        {
          LazyDataLoadState currentState = lazyItem.CurrentState;

          // It was cached, so tell it to re-load
          if (currentState == LazyDataLoadState.Cached)
            lazyItem.GoToState(LazyDataLoadState.Reloading);

          // It was loading and we asked it to pause; unpause it
          else if (currentState == LazyDataLoadState.Loading || currentState == LazyDataLoadState.Reloading)
          {
            if (lazyItem.IsPaused)
              lazyItem.Unpause();
          }

          // Otherwise, tell it to load unless it is already loading
          else if (currentState != LazyDataLoadState.Loaded)
            lazyItem.GoToState(LazyDataLoadState.Loading);
        }

        // Set the loaded item template, if it exists
        if (Owner.LoadedItemTemplate != null)
          ContentTemplate = Owner.LoadedItemTemplate;
      }
      else
      {
        bool alreadyResetTemplate = false;

        // If it's a special item, tell it to change state
        if (lazyItem != null)
        {
          // if the slow items are already loaded, switch to the cached template (vs the default template)
          if (lazyItem.CurrentState == LazyDataLoadState.Loaded)
          {
            lazyItem.GoToState(LazyDataLoadState.Cached);
            if (Owner.CachedItemTemplate != null)
            {
              ContentTemplate = Owner.CachedItemTemplate;
              alreadyResetTemplate = true;
            }
          }
        }

        // If we didn't set it to the CachedItemTemplate, then reset to the ItemTemplate
        if (alreadyResetTemplate != true && Owner.Template != null)
          ContentTemplate = Owner.ItemTemplate;
      }
    }

    /// <summary>
    /// Tell the item to pause any work it is doing, typically because the list is scrolling
    /// </summary>
    internal void Pause()
    {
      // Only makes sense for special items
      if (DataContext is ILazyDataItem)
        (DataContext as ILazyDataItem).Pause();
    }

#if DEBUG_INSTANCE_TRACKING
    // define DEBUG_INSTANCE_TRACKING to see how items are created and destroyed

    /// <summary>
    /// The next sequential ID for instance tracking
    /// </summary>
    static int nexId = 0;

    /// <summary>
    /// Property used for instance tracking
    /// </summary>
    internal int Id { get; private set; }

    ~LazyListBoxItem()
    {
      Debug.WriteLine("** Destroying LazyListBoxItem #" + Id);
    }
#endif

    /// <summary>
    /// Create a new LazyListBoxItem
    /// </summary>
    public LazyListBoxItem()
    {
      DefaultStyleKey = typeof(LazyListBoxItem);

#if DEBUG_INSTANCE_TRACKING
      // define DEBUG_INSTANCE_TRACKING to see how items are created and destroyed
      Id = nexId++;
      Debug.WriteLine("** Creating LazyListBoxItem #" + Id);
#endif
    }
  }
}
