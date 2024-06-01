using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class SchedulePropertiesViewModel : ValidatingViewModel
  {
    #region Constructor

    public SchedulePropertiesViewModel(Schedule schedule)
    {
      Schedule = schedule;
    }

    #endregion

    #region Public Properties

    public override HelpTopic HelpTopicKey => HelpTopic.Properties;

    public Schedule Schedule { get; }

    #endregion

    #region Validation

    protected override ValidationResult OnValidating()
    {
      return string.IsNullOrWhiteSpace(Schedule.Name)
             ? ValidationResult.Error(NASResources.PleaseEnterName)
             : ValidationResult.OK();
    }

    #endregion
  }
}
