using System;

namespace PhoneGuitarTab.Controls
{
  /// <summary>
  /// Interface for a lazy data item
  /// </summary>
  public interface ILazyDataItem
  {
    /// <summary>
    /// Asks the item to move to a specific state.
    /// </summary>
    /// <remarks>
    /// The state machine is well-defined and can be enforced by using the LazyDataItemStateManagerHelper class
    /// </remarks>
    /// <param name="state">The state to go to</param>
    void GoToState(LazyDataLoadState state);

    /// <summary>
    /// Requests that the application pause any current work
    /// </summary>
    /// <remarks>
    /// Not all items will be able to support pausing. It is most critical to pause operations on the UI thread
    /// </remarks>
    void Pause();

    /// <summary>
    /// Unpauses a previously-paused item
    /// </summary>
    void Unpause();

    /// <summary>
    /// Returns whether or not the item is currently paused
    /// </summary>
    bool IsPaused { get; }

    /// <summary>
    /// Returns the current state of the item
    /// </summary>
    LazyDataLoadState CurrentState { get; }

    /// <summary>
    /// Raised when the state of the item changes
    /// </summary>
    /// <remarks>
    /// Avoid hooking this event if possible, and be sure to un-register from it when no longer needed
    /// </remarks>
    event EventHandler<LazyDataItemStateChangedEventArgs> CurrentStateChanged;

    /// <summary>
    /// Raised when the pause state of the item changes
    /// </summary>
    /// <remarks>
    /// Avoid hooking this event if possible, and be sure to un-register from it when no longer needed
    /// </remarks>
    event EventHandler<LazyDataItemPausedStateChangedEventArgs> PauseStateChanged;
  }
}
