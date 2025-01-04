using System.Diagnostics;
using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class EditResourceViewModel : DialogContentViewModel
  {
    #region Fields

    public Resource _resource;
    private string _unit;

    #endregion

    #region Constructor

    public EditResourceViewModel(Resource resource)
    {
      _resource = resource;
      Name = resource.Name;
      CostsPerUnit = resource.CostsPerUnit;
      Limit = resource.Limit;

      switch (_resource)
      {
        case MaterialResource materialResource:
          _unit = materialResource.Unit;
          break;
        case WorkResource:
          _unit = NASResources.Hours;
          break;
        case CalendarResource:
          _unit = NASResources.CalendarDay;
          break;
      }
    }

    #endregion

    #region Overwritten Members

    public override string Title
    {
      get
      {
        if (IsWorkResource)
        {
          return NASResources.WorkResource;
        }
        else if (IsCalendarResource)
        {
          return NASResources.CalendarResource;
        }
        else
        {
          Debug.Assert(IsMaterialResource);
          return NASResources.MaterialResource;
        }
      }
    }

    public override string Icon
    {
      get
      {
        if (IsWorkResource)
        {
          return "Resources";
        }
        else if (IsCalendarResource)
        {
          return "Calendar";
        }
        else
        {
          Debug.Assert(IsMaterialResource);
          return "MaterialResource";
        }
      }
    }

    public override DialogSize DialogSize => DialogSize.Fixed(350, 200);

    public override HelpTopic HelpTopicKey => HelpTopic.Resources;

    #endregion

    #region Properties

    public Resource Resource => _resource;

    public bool CanEditUnit => _resource is MaterialResource;

    public bool IsMaterialResource => _resource is MaterialResource;

    public bool IsCalendarResource => _resource is CalendarResource;

    public bool IsWorkResource => _resource is WorkResource;

    public string Name { get; set; }

    public string Unit
    {
      get => _unit;
      set
      {
        if (_unit != value)
        {
          _unit = value;
          OnPropertyChanged(nameof(Unit));
        }
      }
    }

    public decimal CostsPerUnit { get; set; }

    public double? Limit { get; set; }

    #endregion

    #region Validation

    protected override ValidationResult OnValidating()
    {
      return string.IsNullOrWhiteSpace(Name)
             ? ValidationResult.Error(NASResources.PleaseEnterName)
             : ValidationResult.OK();
    }

    #endregion

    #region Apply

    protected override void OnApply()
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
