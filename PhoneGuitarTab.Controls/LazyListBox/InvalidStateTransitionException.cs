using System;

namespace PhoneGuitarTab.Controls
{
  /// <summary>
  /// Special exception for when an invalid state transition occurs
  /// </summary>
  /// <remarks>
  /// This is a custom exception so that it can be explicitly caught in code if needed (vs. a normal
  /// InvalidOperation)
  /// </remarks>
  public class InvalidStateTransitionException : InvalidOperationException
  {
    /// <summary>
    /// The current state of the item
    /// </summary>
    public LazyDataLoadState CurrentState { get; private set; }

    /// <summary>
    /// The state it was asked to move to
    /// </summary>
    public LazyDataLoadState RequestedState { get; private set; }

    /// <summary>
    /// Create a new instance of the exception
    /// </summary>
    /// <param name="currentState">Current state</param>
    /// <param name="requestedState">Requested state</param>
    public InvalidStateTransitionException(LazyDataLoadState currentState, LazyDataLoadState requestedState)
    {
      CurrentState = currentState;
      RequestedState = requestedState;
    }
  }
}
