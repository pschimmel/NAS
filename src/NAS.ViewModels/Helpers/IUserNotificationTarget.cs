namespace NAS.ViewModels.Helpers
{
  public interface IUserNotificationTarget
  {
    void Error(string message, Action okAction, Action cancelAction = null);

    void Warning(string message, Action okAction, Action cancelAction = null);

    void Information(string message);

    void Question(string message, Action yesAction, Action noAction, Action cancelAction = null);
  }
}
