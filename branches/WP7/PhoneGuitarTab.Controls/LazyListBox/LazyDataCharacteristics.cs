using System;

namespace PhoneGuitarTab.Controls
{
  /// <summary>
  /// Bitfields that can be used to help deal with LazyDataLoadState
  /// </summary>
  /// <remarks>
  /// These flags aren't ever used in the implementation, but they may be useful
  /// as you write your own data items / state machine
  /// </remarks>
  [Flags]
  public enum LazyDataCharacteristics
  {
    None,
    SmallAndFast = 1,
    SmallAndSlow = 2,
    LargeAndFast = 4,
    LargeAndSlow = 8,
    Small = SmallAndFast | SmallAndSlow,
    Large = LargeAndFast | LargeAndSlow,
    Fast = SmallAndFast | LargeAndFast,
    Slow = SmallAndSlow | LargeAndSlow,
    All = Small | Large | Fast | Slow,
  }
}
