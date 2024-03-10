namespace NAS.ViewModel.Base
{
  public class ValidationEventArgs : EventArgs
  {
    public ValidationEventArgs(ValidationResult result)
    {
      Result = result;
    }

    public ValidationResult Result { get; }
  }
}