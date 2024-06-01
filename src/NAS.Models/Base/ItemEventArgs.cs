using System;

namespace NAS.Models.Base
{
  /// <summary>
  /// Event arguments that can be used to transfer any object.
  /// </summary>
  public class ItemEventArgs<T> : EventArgs
  {
    public ItemEventArgs(T item)
    {
      Item = item;
    }
    public T Item { get; }
  }
}
