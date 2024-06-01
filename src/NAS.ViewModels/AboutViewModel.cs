using NAS.Models;
using NAS.Resources;
using NAS.ViewModels.Base;

namespace NAS.ViewModels
{
  public class AboutViewModel : DialogContentViewModel
  {
#pragma warning disable CA1822 // Mark members as static

    public override string Title => NASResources.About;

    public override DialogSize DialogSize => DialogSize.Fixed(550, 420);

    public string Version => Globals.Version.ToString(3);

    public string Copyright => Globals.Copyright;

    public string ApplicationName => Globals.ApplicationName;

    public string ApplicationLongName => $"{Globals.ApplicationName} ({Globals.ApplicationShortName})";

#pragma warning restore CA1822 // Mark members as static

    #region Overwritten Members

    public override IEnumerable<IButtonViewModel> Buttons { get; } = new List<ButtonViewModel> { ButtonViewModel.CreateOKButton() };

    #endregion
  }
}
