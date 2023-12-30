namespace NAS.ViewModel.Base
{
  public abstract class DialogContentViewModel : ViewModelBase, IDialogContentViewModel
  {
    protected DialogContentViewModel(string title, string icon, DialogSize dialogSize = null)
      : base()
    {
      Title = title;
      Icon = icon;
      DialogSize = dialogSize ?? DialogSize.Auto;
    }

    public string Title { get; }

    public string Icon { get; }

    public DialogSize DialogSize { get; }

    public virtual IEnumerable<IButtonViewModel> Buttons => new List<IButtonViewModel>
    {
      ButtonViewModel.CreateCancelButton(),
      ButtonViewModel.CreateOKButton()
    };

    public virtual bool OnClosing(bool? dialogResult)
    {
      return true;
    }
  }
}
