using System.ComponentModel;
using System.Windows.Input;

namespace NAS.ViewModels.Base
{
  public interface IButtonViewModel
  {
    event EventHandler<CancelEventArgs> CommandExecuting;
    event EventHandler CommandExecuted;

    bool ClosesDialog { get; }

    ICommand Command { get; }

    bool IsCancel { get; }

    bool IsDefault { get; }

    string Text { get; }
  }
}
