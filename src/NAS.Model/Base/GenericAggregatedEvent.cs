namespace NAS.Model.Base
{
  public class GenericAggregatedEvent<T> : AggregatedEventBase
  {
    public void Publish(T args)
    {
      InternalPublish(args);
    }

    public void Register(IEventSubscription subscription, Func<T, bool> callback = null)
    {
      InternalRegister(subscription, callback == null ? null : args => callback.Invoke((T)args));
    }

    public void Unregister(IEventSubscription subscription)
    {
      InternalUnregister(subscription);
    }
  }
}
