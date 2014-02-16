using System;

namespace PhoneGuitarTab.Controls
{
  /// <summary>
  /// Arguments for the <see cref="ILazyDataItem.PauseStateChanged"/> event
  /// </summary>
  public class LazyDataItemPausedStateChangedEventArgs : EventArgs
  {
    /// <summary>
    /// Whether the item is paused or not
    /// </summary>
    public bool IsPaused { get; private set; }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="isPaused">The paused state of the item</param>
    public LazyDataItemPausedStateChangedEventArgs(bool isPaused)
    {
      IsPaused = isPaused;
    }
  }
}
