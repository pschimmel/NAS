using System.Windows;
using NAS.Resources;
using NAS.ViewModels.Helpers;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace NAS.Views.Helpers
{
  public class MessageBoxNotificationTarget : IUserNotificationTarget
  {
    public void Error(string message, Action okAction = null, Action cancelAction = null)
    {
      var button = MessageBoxButton.OK;

      if (cancelAction != null)
      {
        button = MessageBoxButton.OKCancel;
      }

      if (MessageBox.Show(message, NASResources.Error, button, MessageBoxImage.Error) == MessageBoxResult.OK)
      {
        okAction.Invoke();
      }
      else
      {
        cancelAction?.Invoke();
      }
    }

    public void Warning(string message, Action okAction = null, Action cancelAction = null)
    {
      var button = MessageBoxButton.OK;

      if (cancelAction != null)
      {
        button = MessageBoxButton.OKCancel;
      }

      if (MessageBox.Show(message, NASResources.Warning, button, MessageBoxImage.Warning) == MessageBoxResult.OK)
      {
        okAction.Invoke();
      }
      else
      {
        cancelAction?.Invoke();
      }
    }

    public void Information(string message)
    {
      MessageBox.Show(message, NASResources.Information, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public void Question(string message, Action yesAction, Action noAction, Action cancelAction = null)
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
          yesAction.Invoke();
          break;
        case MessageBoxResult.No:
          noAction.Invoke();
          break;
        default:
          cancelAction?.Invoke();
          break;
      }
    }
  }
}
