namespace NAS.Model.Base
{
  public class EmptyAggregatedEvent : AggregatedEventBase
  {
    public void Publish()
    {
      InternalPublish(null);
    }

    public void Register(IEventSubscription subscription)
    {
      InternalRegister(subscription, null);
    }

    public void Unregister(IEventSubscription subscription)
    {
      InternalUnregister(subscription);
    }
  }
}
