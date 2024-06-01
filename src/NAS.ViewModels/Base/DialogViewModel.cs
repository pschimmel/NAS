using ES.Tools.Core.MVVM;

namespace NAS.ViewModels.Base
{
  public sealed class DialogViewModel : ViewModelBase, IViewModel
  {
    #region Events

    public event EventHandler RequestOK;
    public event EventHandler RequestCancel;

    #endregion

    #region Fields

    private readonly List<IButtonViewModel> _buttons;

    #endregion

    #region Constructor

    public DialogViewModel(IDialogContentViewModel contentViewModel)
      : base()
    {
      HasErrors = false;
      ContentViewModel = contentViewModel;
      _buttons = [];

      foreach (var buttonVM in contentViewModel.Buttons ?? Enumerable.Empty<ButtonViewModel>())
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

      if (contentViewModel is IValidatable validatableVM)
      {
        validatableVM.Validated += ValidatableVM_Validated;
      }
    }

    #endregion

    #region Properties

    public string Title => ContentViewModel.Title;

    public string Icon => ContentViewModel.Icon;

    public IDialogContentViewModel ContentViewModel { get; }

    public DialogSize DialogSize => ContentViewModel.DialogSize;

    public bool HasErrors { get; private set; }

    public string ErrorMessages { get; private set; }

    #endregion

    #region Validation

    private void ValidatableVM_Validated(object sender, ValidationEventArgs e)
    {
      HasErrors = !e.Result.IsOK;
      ErrorMessages = e.Result.Message;
      OnPropertyChanged(nameof(HasErrors));
      OnPropertyChanged(nameof(ErrorMessages));
    }

    #endregion

    #region Buttons

    public IReadOnlyList<IButtonViewModel> Buttons => _buttons;

    private void Button_OK(object sender, EventArgs e)
    {
      if (ContentViewModel is IValidatable validatable)
      {
        var result = validatable.Validate();
        if (!result.IsOK)
        {
          return;
        }
      }

      if (ContentViewModel is IApplyable applyable)
      {
        applyable.Apply();
      }

      RequestOK?.Invoke(this, EventArgs.Empty);
    }

    private void Button_Cancel(object sender, EventArgs e)
    {
      RequestCancel?.Invoke(this, EventArgs.Empty);
    }

    #endregion
  }
}
