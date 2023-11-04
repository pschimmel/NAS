using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NAS.ViewModel.Base
{
  public abstract class ValidateItemViewModelBase : ES.Tools.Core.MVVM.ViewModel, INotifyDataErrorInfo
  {
    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    private readonly Dictionary<string, List<string>> propertyErrors = new Dictionary<string, List<string>>();

    public IEnumerable GetErrors(string propertyName)
    {
      if (propertyName == null)
      {
        throw new ArgumentNullException(nameof(propertyName));
      }

      propertyErrors.TryGetValue(propertyName, out var errors);
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
      propertyErrors[propertyName] = new List<string> { message };
      OnErrorsChanged(propertyName);
    }

    protected void AddError(string propertyName, string message)
    {
      if (!propertyErrors.ContainsKey(propertyName))
      {
        propertyErrors[propertyName] = new List<string>();
      }

      propertyErrors[propertyName].Add(message);
      OnErrorsChanged(propertyName);
    }

    protected void ClearErrors(string propertyName)
    {
      propertyErrors.Remove(propertyName);
      OnErrorsChanged(propertyName);
    }

    protected void ClearAllErrors()
    {
      string[] propertyNames = propertyErrors.Keys.ToArray();
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
