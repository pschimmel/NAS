namespace NAS.ViewModel.Base
{
  public abstract class ValidatingViewModelBase : ViewModelBase, IValidatable
  {
    protected ValidatingViewModelBase()
      : base()
    { }

    public string ErrorMessage { get; private set; } = null;

    public bool HasErrors => !string.IsNullOrWhiteSpace(ErrorMessage);

    public virtual bool Validate()
    {
      ResetErrors();

      foreach (var (Validation, Message) in ValidationList)
      {
        if (!Validation.Invoke())
        {
          AddError(Message);
        }
      }

      return !HasErrors;
    }

    protected void AddError(string message)
    {
      if (!string.IsNullOrWhiteSpace(message))
      {
        if (!string.IsNullOrWhiteSpace(ErrorMessage))
        {
          ErrorMessage += Environment.NewLine;
        }

        ErrorMessage += message;
        OnPropertyChanged(nameof(ErrorMessage));
        OnPropertyChanged(nameof(HasErrors));
      }
    }

    protected void ResetErrors()
    {
      ErrorMessage = null;
      OnPropertyChanged(nameof(ErrorMessage));
      OnPropertyChanged(nameof(HasErrors));
    }

    protected abstract IEnumerable<(Func<bool> Validation, string Message)> ValidationList { get; }
  }
}
