using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class EditPropertiesViewModel : DialogContentViewModel
  {
    #region Fields

    private readonly Schedule _schedule;

    #endregion

    #region Constructor

    public EditPropertiesViewModel(Schedule schedule)
    {
      _schedule = schedule;
      Name = schedule.Name;
      Description = schedule.Description;
      StartDate = schedule.StartDate;
      EndDate = schedule.EndDate;
      DataDate = schedule.DataDate;
      StandardCalendar = schedule.StandardCalendar;
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.ProjectProperties;

    public override string Icon => "Options";

    public override DialogSize DialogSize => DialogSize.Fixed(560, 250);

    public override HelpTopic HelpTopicKey => HelpTopic.Properties;

    #endregion

    #region Properties

    public string Name { get; set; }

    public string Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime DataDate { get; set; }

    public List<Calendar> Calendars => _schedule.Calendars.ToList();

    public Calendar StandardCalendar { get; set; }

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
      base.OnApply();

      _schedule.Name = Name;
      _schedule.Description = Description;
      _schedule.StartDate = StartDate;
      _schedule.EndDate = EndDate;
      _schedule.DataDate = DataDate;
      _schedule.StandardCalendar = StandardCalendar;
    }

    #endregion
  }
}
