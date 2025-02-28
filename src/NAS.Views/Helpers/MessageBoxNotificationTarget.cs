using System.Windows;
using NAS.Resources;
using NAS.ViewModels.Helpers;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace NAS.Views.Helpers
{
  /// <summary>
  /// Target for messages that will be shown in a message box.
  /// </summary>
  public class MessageBoxNotificationTarget : IUserNotificationTarget
  {
    /// <summary>
    /// Shows an error message.
    /// All actions are optional.
    /// </summary>
    public void Error(string message, Action okAction = null, Action cancelAction = null)
    {
      var button = MessageBoxButton.OK;

      if (cancelAction != null)
      {
        button = MessageBoxButton.OKCancel;
      }

      if (MessageBox.Show(message, NASResources.Error, button, MessageBoxImage.Error) == MessageBoxResult.OK)
      {
        okAction?.Invoke();
      }
      else
      {
        cancelAction?.Invoke();
      }
    }

    /// <summary>
    /// Shows a warning message.
    /// All actions are optional.
    /// </summary>
    public void Warning(string message, Action okAction = null, Action cancelAction = null)
    {
      var button = MessageBoxButton.OK;

      if (cancelAction != null)
      {
        button = MessageBoxButton.OKCancel;
      }

      if (MessageBox.Show(message, NASResources.Warning, button, MessageBoxImage.Warning) == MessageBoxResult.OK)
      {
        okAction?.Invoke();
      }
      else
      {
        cancelAction?.Invoke();
      }
    }

    /// <summary>
    /// Shows an information message.
    /// </summary>
    public void Information(string message)
    {
      MessageBox.Show(message, NASResources.Information, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    /// <summary>
    /// Shows a question message.
    /// All actions are optional.
    /// </summary>
    public void Question(string message, Action yesAction = null, Action noAction = null, Action cancelAction = null)
    {
      var button = MessageBoxButton.YesNo;

      if (cancelAction != null)
      {
        button = MessageBoxButton.YesNoCancel;
      }

      var result = MessageBox.Show(message, NASResources.Question, button, MessageBoxImage.Question);

      switch (result)
      {
        case MessageBoxResult.Yes:
          yesAction?.Invoke();
          break;
        case MessageBoxResult.No:
          noAction?.Invoke();
          break;
        default:
          cancelAction?.Invoke();
          break;
      }
    }
  }
}
