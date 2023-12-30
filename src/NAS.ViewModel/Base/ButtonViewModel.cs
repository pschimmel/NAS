using System.ComponentModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Resources;

namespace NAS.ViewModel.Base
{
  public class ButtonViewModel : IButtonViewModel
  {
    public event EventHandler<CancelEventArgs> CommandExecuting;
    public event EventHandler CommandExecuted;
    private readonly ActionCommand<object> _command;

    public ButtonViewModel(string text, Action executeDelegate = null, Func<bool> canExecuteDelegate = null)
    {
      Text = text;
      _command = new ActionCommand
      (
        x =>
        {
          var resultArgs= new CancelEventArgs();
          CommandExecuting?.Invoke(this, resultArgs);

          if (resultArgs.Cancel)
          {
            return;
          }

          executeDelegate?.Invoke();

          if (ClosesDialog)
          {
            CommandExecuted?.Invoke(this, EventArgs.Empty);
          }
        },
        x => canExecuteDelegate?.Invoke() ?? true
      );
    }

    public string Text { get; }

    public ICommand Command => _command;

    public bool IsDefault { get; set; }

    public bool IsCancel { get; set; }

    public bool ClosesDialog { get; set; } = true;

    public void RaiseCanExecuteChanged()
    {
      _command.RaiseCanExecuteChanged();
    }

    public static ButtonViewModel CreateOKButton(Action action = null, Func<bool> canExecuteAction = null)
    {
      return new ButtonViewModel(NASResources.OK, action, canExecuteAction) { IsDefault = true };
    }

    public static ButtonViewModel CreateCancelButton(Action action = null)
    {
      return new ButtonViewModel(NASResources.Cancel, action) { IsCancel = true };
    }
  }
}
