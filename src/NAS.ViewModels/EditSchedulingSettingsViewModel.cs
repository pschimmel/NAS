using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class EditSchedulingSettingsViewModel : DialogContentViewModel
  {
    #region Fields

    private readonly Schedule _schedule;

    #endregion

    #region Constructor

    public EditSchedulingSettingsViewModel(Schedule schedule)
    {
      _schedule = schedule;
      var settings = schedule.SchedulingSettings;
      CriticalPathDefinition = settings.CriticalPath;
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.SchedulingSettings;

    public override string Icon => "Settings";

    public override DialogSize DialogSize => DialogSize.Auto;

    public override HelpTopic HelpTopicKey => HelpTopic.Calculate;

    #endregion

    #region Properties

    public static CriticalPathDefinition[] CriticalPathDefinitions => Enum.GetValues<CriticalPathDefinition>();

    public CriticalPathDefinition CriticalPathDefinition { get; set; }

    #endregion

    #region Validation

    protected override ValidationResult OnValidating()
    {
      return ValidationResult.OK();
    }

    #endregion

    #region Apply

    protected override void OnApply()
    {
      _schedule.SchedulingSettings.CriticalPath = CriticalPathDefinition;
    }

    #endregion
  }
}
