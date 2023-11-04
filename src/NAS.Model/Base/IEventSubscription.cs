namespace NAS.Model.Base
{
  public interface IEventSubscription
  {
    public void Invoke(object args);
  }
}