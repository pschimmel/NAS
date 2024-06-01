namespace NAS.Models.Base
{
  public interface IEventSubscription
  {
    public void Invoke(object args);
  }
}