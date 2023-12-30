﻿using NAS.Model.Enums;
using NAS.Model.Settings;
using NAS.Resources;
using NAS.ViewModel.Base;

namespace NAS.ViewModel
{
  public class SettingsViewModel : DialogContentViewModel
  {
    #region Fields

    private Themes _selectedTheme;
    private readonly Action _cancelAction;

    #endregion

    #region Constructor

    public SettingsViewModel(Action cancelAction)
      : base(NASResources.Settings, "Gear", DialogSize.Fixed(300, 300))
    {
      _cancelAction = cancelAction;
      var settings = SettingsController.Settings;
      _selectedTheme = settings.Theme;
      ShowInstantHelpOnStartUp = settings.ShowInstantHelpOnStartUp;
      AutoCheckForUpdates = settings.AutoCheckForUpdates;
    }

    #endregion

    #region Overwritten Members

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

    public Themes SelectedTheme
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
