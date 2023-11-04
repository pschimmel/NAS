namespace NAS.Model.Base
{
  public class EventSubscription<T> : IEventSubscription
  {
    private readonly Action<T> _action;
    private readonly Func<T, bool> _callBack;

    public EventSubscription(Action<T> action, Func<T, bool> callBack = null)
    {
      _action = action;
      _callBack = callBack;
    }

    public void Invoke(object args)
    {
      if (_callBack == null || _callBack.Invoke((T)args))
      {
        _action.Invoke((T)args);
      }
    }
  }
}
