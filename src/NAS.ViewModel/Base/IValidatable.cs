namespace NAS.ViewModel.Base
{
  public interface IValidatable
  {
    public event EventHandler<ValidationEventArgs> Validated;

    ValidationResult Validate();
  }
}
