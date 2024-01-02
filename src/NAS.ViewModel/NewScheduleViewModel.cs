using NAS.Model.Entities;
using NAS.Resources;
using NAS.ViewModel.Base;

namespace NAS.ViewModel
{
  public class NewScheduleViewModel : DialogContentViewModel
  {
    #region Fields

    private bool _hasErrors;

    #endregion

    #region Constructor

    public NewScheduleViewModel()
      : base()
    {
      Schedule = new Schedule();
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.NewSchedule;

    public override string Icon => "Gear";

    public override DialogSize DialogSize => DialogSize.Fixed(550, 350);

    public override IEnumerable<IButtonViewModel> Buttons
    {
      get => new List<ButtonViewModel>
      {
        // Buttons are part of the Wizard control
      };
    }

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

    public bool CanFinish => !_hasErrors;

    public string LastPageTitle => CanFinish ? NASResources.WizardTitleLastPage : NASResources.WizardTitleLastPageError;

    public string LastPageDescription => CanFinish ? NASResources.WizardDescriptionLastPage : NASResources.WizardDescriptionLastPageError;

    public string Error { get; private set; }

    #endregion

    #region IValidatable

    private List<(Func<bool> Validate, string Message)> ValidationList()
    {
      return new List<(Func<bool>, string)>
      {
        (() => !string.IsNullOrWhiteSpace(Schedule.Name), NASResources.PleaseEnterName),
        (() => Schedule.StandardCalendar != null, NASResources.PleaseSelectCalendar),
        (() => Schedule.StartDate != DateTime.MinValue, NASResources.PleaseEnterStartDate),
        (() => !Schedule.EndDate.HasValue || Schedule.StartDate != DateTime.MinValue && Schedule.EndDate.Value > Schedule.StartDate, NASResources.FinishDateNotEarlierThanStartDate),
        (() => Schedule.DataDate >= Schedule.StartDate, NASResources.DataDateNotEarlierThanStartDate)
      };
    }

    public void Validate()
    {
      _hasErrors = false;
      Error = null;

      foreach (var validationItem in ValidationList())
      {
        var result = validationItem.Validate();
        if (result == false)
        {
          _hasErrors = true;
          Error = validationItem.Message;
          break;
        }
      }

      OnPropertyChanged(nameof(LastPageTitle));
      OnPropertyChanged(nameof(LastPageDescription));
      OnPropertyChanged(nameof(CanFinish));
    }

    #endregion
  }
}
