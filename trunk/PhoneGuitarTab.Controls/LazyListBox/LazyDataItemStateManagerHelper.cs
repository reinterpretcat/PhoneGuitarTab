using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace PhoneGuitarTab.Controls
{
  /// <summary>
  /// Helper class you can use to verify that the state machine is working correctly (ie, the
  /// code provided in this library is working correctly!).
  /// If you get an exception in this code, please let me know! (http://blogs.msdn.com/ptorr/)
  /// </summary>
  public static class LazyDataItemStateManagerHelper
  {
    /// <summary>
    /// Helper method to check whether a given state transition is valid
    /// </summary>
    /// <param name="currentState">The state your object is currently in</param>
    /// <param name="requestedState">The state it has been asked to move to</param>
    /// <remarks>
    /// You can call this method in your <see cref="ILazyDataItem.GoToState"/> method; it will no-op
    /// in retail builds and on debug builds it will fail with an exception if the transition is invalid
    /// </remarks>
    [Conditional("DEBUG")]
    public static void CheckTransition(LazyDataLoadState currentState, LazyDataLoadState requestedState)
    {
      // Call not needed...
      if (currentState == requestedState)
      {
        Debug.WriteLine("Uneccessary state transition from " + currentState + " to itself");
        return;
      }

      // Call to unloaded is always OK
      if (requestedState == LazyDataLoadState.Unloaded)
      {
        return;
      }

      switch (currentState)
      {
        case LazyDataLoadState.Unloaded:
          if (requestedState == LazyDataLoadState.Minimum)
            return;

          break;

        case LazyDataLoadState.Minimum:
          if (requestedState == LazyDataLoadState.Loading)
            return;

          break;

        case LazyDataLoadState.Loading:
          if (requestedState == LazyDataLoadState.Loaded || requestedState == LazyDataLoadState.Cached)
            return;

          break;

        case LazyDataLoadState.Loaded:
          if (requestedState == LazyDataLoadState.Cached)
            return;

          break;

        case LazyDataLoadState.Cached:
          if (requestedState == LazyDataLoadState.Reloading)
            return;

          break;

        case LazyDataLoadState.Reloading:
          if (requestedState == LazyDataLoadState.Loaded || requestedState == LazyDataLoadState.Cached)
            return;

          break;

        default:
          throw new InvalidOperationException("Unknown current state " + currentState.ToString());
      }

      if (Debugger.IsAttached)
        Debugger.Break();

      throw new InvalidStateTransitionException(currentState, requestedState);
    }
  }
}
