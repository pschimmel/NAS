namespace NAS.ViewModel.Base
{
  public abstract class ValidatingViewModel : ViewModelBase, IValidatable
  {
    public event EventHandler<ValidationEventArgs> Validated;

    protected ValidatingViewModel()
      : base()
    { }

    public ValidationResult Validate()
    {
      var result = OnValidating();
      Validated?.Invoke(this, new ValidationEventArgs(result));
      return result;
    }

    protected virtual ValidationResult OnValidating()
    {
      return ValidationResult.OK();
    }
  }
}
