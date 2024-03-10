namespace NAS.ViewModel.Base
{
  public abstract class DialogContentViewModel : ValidatingViewModel, IDialogContentViewModel
  {
    protected DialogContentViewModel()
      : base()
    { }

    public abstract string Title { get; }

    public virtual string Icon => "Icon";

    public virtual DialogSize DialogSize => DialogSize.Auto;

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
