using NAS.Model.Entities;
using NAS.Resources;
using NAS.ViewModel.Base;

namespace NAS.ViewModel
{
  public class ResourceDetailsViewModel : ViewModelBase, IValidatable, IApplyable
  {
    #region Fields

    public Resource _resource;

    #endregion

    #region Constructor

    public ResourceDetailsViewModel(Resource resource)
    {
      _resource = resource;
      Name = resource.Name;
      CostsPerUnit = resource.CostsPerUnit;
      Limit = resource.Limit;

      if (_resource is MaterialResource materialResource)
      {
        Unit = materialResource.Unit;
      }
    }

    #endregion

    #region Public Properties

    public bool CanEditUnit => _resource is MaterialResource;

    public string Name { get; set; }

    public string Unit { get; set; }

    public decimal CostsPerUnit { get; set; }

    public double? Limit { get; set; }

    #endregion

    #region IValidatable Implementation

    public string ErrorMessage { get; set; } = null;

    public bool HasErrors => !string.IsNullOrWhiteSpace(ErrorMessage);

    private void AddError(string message)
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

    public bool Validate()
    {
      ErrorMessage = null;
      if (string.IsNullOrWhiteSpace(_resource.Name))
      {
        AddError(NASResources.PleaseEnterName);
      }

      return !HasErrors;
    }

    #endregion

    #region IApplyable Implementation

    public void Apply()
    {
      if (Validate())
      {
        _resource.Name = Name;
        _resource.CostsPerUnit = CostsPerUnit;
        _resource.Limit = Limit;
        if (_resource is MaterialResource materialResource)
        {
          materialResource.Unit = Unit;
        }
      }
    }

    #endregion
  }
}
