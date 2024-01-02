using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NAS.ViewModel.Base
{
  public abstract class ValidateItemViewModelBase : ES.Tools.Core.MVVM.ViewModel, INotifyDataErrorInfo
  {
    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    private readonly Dictionary<string, List<string>> propertyErrors = [];

    public IEnumerable GetErrors(string propertyName)
    {
      ArgumentNullException.ThrowIfNull(propertyName);
      _ = propertyErrors.TryGetValue(propertyName, out var errors);
      return errors;
    }

    public bool HasErrors => propertyErrors.Any(x => x.Value.Count > 0);

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      base.OnPropertyChanged(propertyName);
      Validate(propertyName);
    }

    protected abstract void Validate(string propertyName);

    protected void Validate(string propertyName, Func<bool> validation, string errorMessage)
    {
      if (validation.Invoke())
      {
        ClearErrors(propertyName);
      }
      else
      {
        SetError(propertyName, errorMessage);
      }
    }

    protected void SetError(string propertyName, string message)
    {
      propertyErrors[propertyName] = [message];
      OnErrorsChanged(propertyName);
    }

    protected void AddError(string propertyName, string message)
    {
      if (!propertyErrors.TryGetValue(propertyName, out var propertyErrorList))
      {
        propertyErrorList = ([]);
        propertyErrors[propertyName] = propertyErrorList;
      }

      propertyErrorList.Add(message);
      OnErrorsChanged(propertyName);
    }

    protected void ClearErrors(string propertyName)
    {
      _ = propertyErrors.Remove(propertyName);
      OnErrorsChanged(propertyName);
    }

    protected void ClearAllErrors()
    {
      string[] propertyNames = [.. propertyErrors.Keys];
      propertyErrors.Clear();
      foreach (string propertyName in propertyNames)
      {
        OnErrorsChanged(propertyName);
      }
    }

    protected void OnErrorsChanged(string propertyName)
    {
      ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }
  }
}
