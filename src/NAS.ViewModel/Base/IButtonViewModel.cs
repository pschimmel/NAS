using System;
using System.ComponentModel;
using System.Windows.Input;

namespace NAS.ViewModel.Base
{
  public interface IButtonViewModel
  {
    event EventHandler<CancelEventArgs> CommandExecuting;
    event EventHandler CommandExecuted;
    bool ClosesDialog { get; set; }
    ICommand Command { get; }
    bool IsCancel { get; set; }
    bool IsDefault { get; set; }
    string Text { get; }
  }
}
