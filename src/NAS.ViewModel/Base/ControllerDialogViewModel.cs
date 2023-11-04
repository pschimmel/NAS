using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NAS.ViewModel.Base
{
  public abstract class ControllerDialogViewModel : ViewModelBase
  {
    private IList<IButtonViewModel> buttons = new List<IButtonViewModel>();

    public event EventHandler CloseDialog;

    protected ControllerDialogViewModel(string title, string icon, DialogSize dialogSize)
      : base()
    {
      Title = title;
      Icon = icon;
      DialogSize = dialogSize;
    }

    public string Title { get; }

    public string Icon { get; }

    public DialogSize DialogSize { get; }

    public IList<IButtonViewModel> Buttons
    {
      get => buttons;
      protected set
      {
        if (buttons != null)
        {
          buttons.ToList().ForEach(x => x.CommandExecuting -= Button_CommandExecuting);
          buttons.ToList().ForEach(x => x.CommandExecuted -= Button_CommandExecuted);
        }
        buttons = value;
        if (buttons != null)
        {
          buttons.ToList().ForEach(x => x.CommandExecuting += Button_CommandExecuting);
          buttons.ToList().ForEach(x => x.CommandExecuted += Button_CommandExecuted);
        }
      }
    }

    private void Button_CommandExecuting(object sender, CancelEventArgs e)
    {
      if (sender is ButtonViewModel buttonVM && this is IValidatable vm && buttonVM.ClosesDialog && !buttonVM.IsCancel)
      {
        e.Cancel = !vm.Validate();
      }
    }

    private void Button_CommandExecuted(object sender, EventArgs e)
    {
      if (sender is ButtonViewModel buttonVM && buttonVM.ClosesDialog)
      {
        CloseDialog?.Invoke(this, EventArgs.Empty);
      }
    }
  }
}
