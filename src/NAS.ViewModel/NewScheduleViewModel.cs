using NAS.Model.Entities;
using NAS.Resources;
using NAS.ViewModel.Base;

namespace NAS.ViewModel
{
  public class NewScheduleViewModel : ValidatingViewModelBase, IValidatable
  {
    #region Constructor

    public NewScheduleViewModel()
      : base()
    {
      Schedule = new Schedule();
    }

    #endregion

    #region Public Properties

    public Schedule Schedule { get; }

    public string ProjectName
    {
      get => Schedule.Name;
      set
      {
        if (Schedule.Name != value)
        {
          Schedule.Name = value;
          OnPropertyChanged(nameof(ProjectName));
        }
      }
    }

    public bool CanFinish => !HasErrors;

    public string LastPageTitle => CanFinish ? NASResources.WizardTitleLastPage : NASResources.WizardTitleLastPageError;

    public string LastPageDescription => CanFinish ? NASResources.WizardDescriptionLastPage : NASResources.WizardDescriptionLastPageError;

    #endregion

    #region IValidatingViewModel

    protected override IEnumerable<(Func<bool>, string)> ValidationList =>
      new List<(Func<bool>, string)>
      {
        (() => !string.IsNullOrWhiteSpace(Schedule.Name), NASResources.PleaseEnterName),
        (() => Schedule.StandardCalendar != null, NASResources.PleaseSelectCalendar),
        (() => Schedule.StartDate != DateTime.MinValue, NASResources.PleaseEnterStartDate),
        (() => !Schedule.EndDate.HasValue || Schedule.StartDate != DateTime.MinValue && Schedule.EndDate.Value > Schedule.StartDate, NASResources.FinishDateNotEarlierThanStartDate),
        (() => Schedule.DataDate >= Schedule.StartDate, NASResources.DataDateNotEarlierThanStartDate)
      };

    public override bool Validate()
    {
      bool result = base.Validate();
      OnPropertyChanged(nameof(LastPageTitle));
      OnPropertyChanged(nameof(LastPageDescription));
      OnPropertyChanged(nameof(CanFinish));
      return result;
    }

    #endregion
  }
}
