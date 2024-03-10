using NAS.Model.Entities;
using NAS.Resources;
using NAS.ViewModel.Base;

namespace NAS.ViewModel
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

    protected override ValidationResult ValidateImpl()
    {
      return ValidationResult.OK();
    }

    #endregion
  }
}
