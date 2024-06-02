using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class ResourceAssignmentViewModel : DialogContentViewModel
  {
    #region Fields

    private readonly ResourceAssignment _resourceAssignment;

    #endregion

    #region Constructor

    public ResourceAssignmentViewModel(ResourceAssignment resourceAssignment)
    {
      _resourceAssignment = resourceAssignment;

      Unit = resourceAssignment.Resource switch
      {
        WorkResource _ => NASResources.Hours,
        CalendarResource _ => NASResources.CalendarDay,
        MaterialResource _ => (resourceAssignment.Resource as MaterialResource).Unit,
        _ => throw new ArgumentException("Unknown resource type.")
      };
      Budget = _resourceAssignment.Budget;
      FixedCosts = _resourceAssignment.FixedCosts;
      CostsPerUnit = _resourceAssignment.Resource.CostsPerUnit;
      UnitsPerDay = _resourceAssignment.UnitsPerDay;
    }

    #region Overwritten Members

    public override string Title => NASResources.ResourceAssignment;

    public override string Icon => "Resources";

    public override DialogSize DialogSize => DialogSize.Fixed(400, 300);

    public override HelpTopic HelpTopicKey => HelpTopic.Resources;

    #endregion

    #endregion

    #region Properties

    public string Name => _resourceAssignment.Resource.Name;

    public string Unit { get; }

    public decimal Budget { get; set; }

    public decimal FixedCosts { get; set; }

    public decimal CostsPerUnit { get; }

    public double UnitsPerDay { get; set; }

    #endregion

    #region Apply
    protected override void OnApply()
    {
      if (Validate().IsOK)
      {
        _resourceAssignment.Budget = Budget;
        _resourceAssignment.FixedCosts = FixedCosts;
        _resourceAssignment.UnitsPerDay = UnitsPerDay;
      }
    }

    #endregion
  }
}
