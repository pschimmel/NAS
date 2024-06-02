using NAS.Models.Scheduler;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class SchedulingSettingsViewModel : DialogContentViewModel
  {
    #region Constructor

    public SchedulingSettingsViewModel(string settingsString)
      : base()
    {
      Settings = SchedulingSettingsHelper.LoadSchedulingSettings(settingsString);
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.SchedulingSettings;

    public override string Icon => "Settings";

    #endregion

    #region Properties

    public SchedulingSettings Settings { get; }

    public override HelpTopic HelpTopicKey => HelpTopic.Calculate;

    public string GetScheduleSettingsString()
    {
      return SchedulingSettingsHelper.SaveSchedulingSettings(Settings);
    }

    #endregion

    #region Validation

    protected override ValidationResult OnValidating()
    {
      return ValidationResult.OK();
    }

    #endregion
  }
}
