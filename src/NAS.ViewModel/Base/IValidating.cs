namespace NAS.ViewModel.Base
{
  public interface IValidating
  {
    public event EventHandler<ValidationEventArgs> Validated;

    ValidationResult Validate();
  }
}
