using System;
using System.Collections.Generic;
using NAS.Model.Enums;
using NAS.Resources;
using NAS.ViewModel.Base;
using NAS.Model.Settings;

namespace NAS.ViewModel
{
  public class SettingsViewModel : DialogViewModel
  {
    #region Fields

    private Themes _selectedTheme;

    #endregion

    #region Constructor

    public SettingsViewModel(Action cancelAction)
      : base(NASResources.Settings, "Gear", DialogSize.Fixed(300, 300))
    {
      var settings = SettingsController.Settings;
      _selectedTheme = settings.Theme;
      ShowInstantHelpOnStartUp = settings.ShowInstantHelpOnStartUp;
      AutoCheckForUpdates = settings.AutoCheckForUpdates;
      Buttons = new List<IButtonViewModel> { ButtonViewModel.CreateCancelButton(cancelAction), ButtonViewModel.CreateOKButton(() =>
      {
        settings.Theme = SelectedTheme;
        settings.AutoCheckForUpdates = AutoCheckForUpdates;
        settings.ShowInstantHelpOnStartUp = ShowInstantHelpOnStartUp;
      }) };
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
