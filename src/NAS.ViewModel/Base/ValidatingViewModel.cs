namespace NAS.ViewModel.Base
{
  public abstract class ValidatingViewModel : ViewModelBase, IValidating
  {
    public event EventHandler<ValidationEventArgs> Validated;

    protected ValidatingViewModel()
      : base()
    { }

    public ValidationResult Validate()
    {
      var result = ValidateImpl();
      Validated?.Invoke(this, new ValidationEventArgs(result));
      return result;
    }

    protected virtual ValidationResult ValidateImpl()
    {
      return ValidationResult.OK();
    }
  }
}
