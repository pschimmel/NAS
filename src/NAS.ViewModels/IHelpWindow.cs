using System;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public interface IHelpWindow
  {
    event EventHandler Closed;
    void SelectTopic(HelpTopic topic);
    void Show();
  }
}
