namespace NAS.ViewModel.Base
{
  public interface IValidatable
  {
    string ErrorMessage { get; }

    bool HasErrors { get; }

    bool Validate();
  }
}
