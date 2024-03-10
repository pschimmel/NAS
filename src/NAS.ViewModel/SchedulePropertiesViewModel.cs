using NAS.Model.Entities;
using NAS.Resources;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel
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

    protected override ValidationResult ValidateImpl()
    {
      return string.IsNullOrWhiteSpace(Schedule.Name)
             ? ValidationResult.Error(NASResources.PleaseEnterName)
             : ValidationResult.OK();
    }

    #endregion
  }
}
