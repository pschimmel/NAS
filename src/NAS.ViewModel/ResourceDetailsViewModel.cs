using NAS.Model.Entities;
using NAS.Resources;
using NAS.ViewModel.Base;

namespace NAS.ViewModel
{
  public class ResourceDetailsViewModel : ValidatingViewModel, IApplyable
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

    public Resource Resource => _resource;

    public bool CanEditUnit => _resource is MaterialResource;

    public bool IsMaterialResource => _resource is MaterialResource;

    public bool IsCalendarResource => _resource is CalendarResource;

    public bool IsWorkResource => _resource is WorkResource;

    public string Name { get; set; }

    public string Unit { get; set; }

    public decimal CostsPerUnit { get; set; }

    public double? Limit { get; set; }

    #endregion

    #region Validation

    protected override ValidationResult OnValidating()
    {
      return string.IsNullOrWhiteSpace(_resource.Name)
             ? ValidationResult.Error(NASResources.PleaseEnterName)
             : ValidationResult.OK();
    }

    #endregion

    #region Apply

    public void Apply()
    {
      if (Validate().IsOK)
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
