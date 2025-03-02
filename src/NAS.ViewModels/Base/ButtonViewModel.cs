using System.ComponentModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Resources;

namespace NAS.ViewModels.Base
{
  public class ButtonViewModel : IButtonViewModel
  {
    public event EventHandler<CancelEventArgs> CommandExecuting;
    public event EventHandler CommandExecuted;

    private readonly ActionCommand<object> _command;

    private ButtonViewModel(string text,
                            Action executeDelegate,
                            Func<bool> canExecuteDelegate,
                            bool isDefault,
                            bool isCancel,
                            bool closesDialog = true)
    {
      Text = text;
      _command = new ActionCommand
      (
        x =>
        {
          var resultArgs = new CancelEventArgs();
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
        x => canExecuteDelegate.Invoke()
      );
      IsDefault = isDefault;
      IsCancel = isCancel;
      ClosesDialog = closesDialog;
    }

    public string Text { get; }

    public ICommand Command => _command;

    public bool IsDefault { get; }

    public bool IsCancel { get; }

    public bool ClosesDialog { get; } = true;

    public void RaiseCanExecuteChanged()
    {
      _command.RaiseCanExecuteChanged();
    }

    public static ButtonViewModel CreateOKButton(Action action = null, Func<bool> canExecuteAction = null)
    {
      return new ButtonViewModel(NASResources.OK, action ?? new Action(() => { }), canExecuteAction ?? new Func<bool>(() => true), true, false);
    }

    public static ButtonViewModel CreateCloseButton(Action action = null, Func<bool> canExecuteAction = null)
    {
      return new ButtonViewModel(NASResources.Close, action ?? new Action(() => { }), canExecuteAction ?? new Func<bool>(() => true), true, true);
    }

    public static ButtonViewModel CreateCancelButton(Action action = null)
    {
      return new ButtonViewModel(NASResources.Cancel, action ?? new Action(() => { }), new Func<bool>(() => true), false, true);
    }

    public static ButtonViewModel CreateButton(string text, Action action, Func<bool> canExecuteAction = null)
    {
      return new ButtonViewModel(text, action, canExecuteAction ?? new Func<bool>(() => true), false, false, false);
    }

    public static ButtonViewModel CreateAcceptButton(string text, Action action, Func<bool> canExecuteAction = null)
    {
      return new ButtonViewModel(text, action, canExecuteAction ?? new Func<bool>(() => true), false, false, true);
    }
  }
}
