namespace NAS.Model.Base
{
  /// <summary>
  /// Event arguments that can be used to transfer any object.
  /// </summary>
  public class RequestItemEventArgs<T, U> : ItemEventArgs<T>
  {

    public RequestItemEventArgs(T item)
      : base(item)
    { }

    public U ReturnedItem { get; set; }
  }
}
