#nullable enable

using System.Collections.Generic;

namespace Shutdown
{
  internal static class GeneralExtensions
  {

    /// <summary>
    /// Assigns '<paramref name="newValue"/>' to '<paramref name="currentValue"/>', only if they are different.
    /// </summary>
    /// <typeparam name="T">The type of the two values.</typeparam>
    /// <param name="currentValue">Reference to the stored value.</param>
    /// <param name="newValue">The new value to be stored. </param>
    /// <returns>Returns <see langword="true"/> if the value was updated, otherwise <see langword="false"/>. </returns>
    public static bool SetIfNotEqual<T>(ref T currentValue, T newValue)
    {
      if (EqualityComparer<T>.Default.Equals(currentValue, newValue)) return false;
      currentValue = newValue;
      return true;
    }



  }
}
