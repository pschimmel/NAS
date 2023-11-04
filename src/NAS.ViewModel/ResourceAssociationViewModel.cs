using NAS.Model.Entities;
using NAS.Resources;
using NAS.ViewModel.Base;

namespace NAS.ViewModel
{
  public class ResourceAssociationViewModel : ViewModelBase, IValidatable
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

    private string errorMessage = null;

    public string ErrorMessage
    {
      get => errorMessage;
      set
      {
        errorMessage = value;
        OnPropertyChanged(nameof(ErrorMessage));
        HasErrors = !string.IsNullOrWhiteSpace(value);
        OnPropertyChanged(nameof(HasErrors));
      }
    }

    public bool HasErrors { get; private set; } = false;

    public bool Validate()
    {
      return !HasErrors;
    }

    #endregion
  }
}
