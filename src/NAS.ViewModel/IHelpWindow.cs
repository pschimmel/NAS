using System;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel
{
  public interface IHelpWindow
  {
    event EventHandler Closed;
    void SelectTopic(HelpTopic topic);
    void Show();
  }
}