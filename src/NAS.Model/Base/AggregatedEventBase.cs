namespace NAS.Model.Base
{
  public abstract class AggregatedEventBase
  {
    private readonly Dictionary<IEventSubscription, Func<object, bool>> _subscriptions = [];

    protected void InternalPublish(params object[] args)
    {
      foreach (var (EventSubscription, Callback) in _subscriptions)
      {
        if (Callback == null || Callback(args))
        {
          EventSubscription.Invoke(args);
        }
      }
    }

    protected void InternalRegister(IEventSubscription subscription, Func<object, bool> callback = null)
    {
      _subscriptions[subscription] = callback;
    }

    protected void InternalUnregister(IEventSubscription subscription)
    {
      _ = _subscriptions.Remove(subscription);
    }
  }
}
