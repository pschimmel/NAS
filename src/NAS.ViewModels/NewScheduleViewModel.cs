using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;

namespace NAS.ViewModels
{
  public class NewScheduleViewModel : DialogContentViewModel
  {
    #region Constructor

    public NewScheduleViewModel()
      : base()
    {
      Schedule = new Schedule();
      Schedule.Validate();
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.NewSchedule;

    public override string Icon => "Gear";

    public override DialogSize DialogSize => DialogSize.Fixed(550, 400);

    // All buttons are part of the Wizard control
    public override IEnumerable<IButtonViewModel> Buttons => null;

    public override bool OnClosing(bool? dialogResult)
    {
      if (dialogResult == true)
      {
        Validate();
      }

      return CanFinish;
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

    public string ErrorMessage { get; private set; }

    public bool HasErrors { get; private set; }

    #endregion

    #region IValidatable

    private List<(Func<bool> Validate, string Message)> ValidationList()
    {
      return
      [
        (() => !string.IsNullOrWhiteSpace(Schedule.Name), NASResources.PleaseEnterName),
        (() => Schedule.StandardCalendar != null, NASResources.PleaseSelectCalendar),
        (() => Schedule.StartDate != DateTime.MinValue, NASResources.PleaseEnterStartDate),
        (() => !Schedule.EndDate.HasValue || Schedule.StartDate != DateTime.MinValue && Schedule.EndDate.Value > Schedule.StartDate, NASResources.FinishDateNotEarlierThanStartDate),
        (() => Schedule.DataDate >= Schedule.StartDate, NASResources.DataDateNotEarlierThanStartDate)
      ];
    }

    public void ValidateScheduleData()
    {
      HasErrors = false;
      ErrorMessage = null;

      foreach (var validationItem in ValidationList())
      {
        var result = validationItem.Validate();
        if (result == false)
        {
          HasErrors = true;
          ErrorMessage = validationItem.Message;
          break;
        }
      }

      OnPropertyChanged(nameof(LastPageTitle));
      OnPropertyChanged(nameof(LastPageDescription));
      OnPropertyChanged(nameof(CanFinish));
      OnPropertyChanged(nameof(HasErrors));
      OnPropertyChanged(nameof(ErrorMessage));
    }

    #endregion
  }
}
