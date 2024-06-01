using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;

namespace NAS.ViewModels
{
  public class ResourceAssociationViewModel : ValidatingViewModel
  {
    #region Constructor

    public ResourceAssociationViewModel(ResourceAssociation resourceAssociation)
    {
      ResourceAssociation = resourceAssociation;
      Unit = resourceAssociation.Resource switch
      {
        WorkResource _ => NASResources.Hours,
        CalendarResource _ => NASResources.CalendarDay,
        _ => (resourceAssociation.Resource as MaterialResource).Unit,
      };
    }

    #endregion

    #region Public Properties

    public ResourceAssociation ResourceAssociation { get; private set; }

    public string Unit { get; private set; }

    #endregion

    #region Validation

    protected override ValidationResult OnValidating()
    {
      return ValidationResult.OK();
    }

    #endregion
  }
}
