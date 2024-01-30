using ES.Tools.Core.MVVM;

namespace NAS.ViewModel.Base
{
  public sealed class DialogViewModel : ViewModelBase, IViewModel
  {
    public event EventHandler RequestOK;
    public event EventHandler RequestCancel;

    private readonly List<IButtonViewModel> _buttons;

    public DialogViewModel(IDialogContentViewModel contentViewModel)
      : base()
    {
      ContentViewModel = contentViewModel;
      _buttons = [];

      foreach (var buttonVM in contentViewModel.Buttons)
      {
        _buttons.Add(buttonVM);

        if (buttonVM.IsCancel)
        {
          buttonVM.CommandExecuting += Button_Cancel;
        }
        else if (buttonVM.ClosesDialog)
        {
          buttonVM.CommandExecuted += Button_OK;
        }
      }
    }

    public string Title => ContentViewModel.Title;

    public string Icon => ContentViewModel.Icon;

    public IDialogContentViewModel ContentViewModel { get; }

    public DialogSize DialogSize => ContentViewModel.DialogSize;

    public IEnumerable<IButtonViewModel> Buttons => _buttons;

    private void Button_OK(object sender, EventArgs e)
    {
      RequestOK?.Invoke(this, EventArgs.Empty);
    }

    private void Button_Cancel(object sender, EventArgs e)
    {
      RequestCancel?.Invoke(this, EventArgs.Empty);
    }
  }
}
