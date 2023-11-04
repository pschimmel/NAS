using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ES.Tools.Core.MVVM;

namespace NAS.ViewModel.Base
{
  public abstract class DialogViewModel : ViewModelBase, IViewModel
  {
    public event EventHandler CloseDialog;

    private IList<IButtonViewModel> _buttons = new List<IButtonViewModel>();

    protected DialogViewModel(string title, string icon, DialogSize dialogSize = null)
      : base()
    {
      Title = title;
      Icon = icon;
      DialogSize = dialogSize ?? DialogSize.Auto;
      Buttons = new List<IButtonViewModel> { ButtonViewModel.CreateCancelButton(), ButtonViewModel.CreateOKButton() };
    }

    public string Title { get; }

    public string Icon { get; }

    public DialogSize DialogSize { get; }

    public IList<IButtonViewModel> Buttons
    {
      get => _buttons;
      protected set
      {
        if (_buttons != null)
        {
          _buttons.ToList().ForEach(x => x.CommandExecuting -= Button_CommandExecuting);
          _buttons.ToList().ForEach(x => x.CommandExecuted -= Button_CommandExecuted);
        }
        _buttons = value;
        if (_buttons != null)
        {
          _buttons.ToList().ForEach(x => x.CommandExecuting += Button_CommandExecuting);
          _buttons.ToList().ForEach(x => x.CommandExecuted += Button_CommandExecuted);
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
