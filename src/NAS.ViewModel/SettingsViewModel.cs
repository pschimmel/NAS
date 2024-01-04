using NAS.Model.Enums;
using NAS.Model.Settings;
using NAS.Resources;
using NAS.ViewModel.Base;

namespace NAS.ViewModel
{
  public class SettingsViewModel : DialogContentViewModel
  {
    #region Fields

    private Theme _selectedTheme;
    private readonly Action _cancelAction;

    #endregion

    #region Constructor

    public SettingsViewModel(Action cancelAction)
      : base()
    {
      _cancelAction = cancelAction;
      var settings = SettingsController.Settings;
      _selectedTheme = settings.Theme;
      ShowInstantHelpOnStartUp = settings.ShowInstantHelpOnStartUp;
      AutoCheckForUpdates = settings.AutoCheckForUpdates;
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.Settings;

    public override string Icon => "Gear";

    public override DialogSize DialogSize => DialogSize.Fixed(300, 300);

    public override IEnumerable<IButtonViewModel> Buttons
    {
      get => new List<ButtonViewModel>
                 {
                   ButtonViewModel.CreateCancelButton(_cancelAction),
                   ButtonViewModel.CreateOKButton(() =>
                   {
                     var settings = SettingsController.Settings;
                     settings.Theme = SelectedTheme;
                     settings.AutoCheckForUpdates = AutoCheckForUpdates;
                     settings.ShowInstantHelpOnStartUp = ShowInstantHelpOnStartUp;
                     SettingsController.Save();
                   })
                 };
    }

    #endregion

    #region Public Members

    public Theme SelectedTheme
    {
      get => _selectedTheme;
      set
      {
        if (_selectedTheme != value)
        {
          _selectedTheme = value;
          OnPropertyChanged(nameof(SelectedTheme));
        }
      }
    }

    public bool ShowInstantHelpOnStartUp { get; set; }

    public bool AutoCheckForUpdates { get; set; }

    #endregion
  }
}
