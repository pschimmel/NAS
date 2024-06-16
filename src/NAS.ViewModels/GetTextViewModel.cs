using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class GetTextViewModel : DialogContentViewModel
  {
    #region Fields

    private readonly bool _allowEmpty;

    #endregion

    #region Constructor

    public GetTextViewModel(string title, string label, string text = null, bool allowEmpty = false)
    {
      Title = title;
      Label = label;
      Text = text;
      _allowEmpty = allowEmpty;
    }

    #endregion 

    #region Properties

    public override string Title { get; }

    public string Label { get; }

    public string Text { get; set; }

    #endregion

    #region Validation

    protected override ValidationResult OnValidating()
    {
      if (!_allowEmpty && string.IsNullOrWhiteSpace(Text))
      {
        UserNotificationService.Instance.Error(NASResources.MessageTextMayNotBeEmpty);
        return ValidationResult.Error(NASResources.MessageTextMayNotBeEmpty);
      }

      return ValidationResult.OK();
    }

    #endregion
  }
}
