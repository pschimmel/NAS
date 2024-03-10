using NAS.Resources;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel
{
  public class GetTextViewModel : ValidatingViewModel
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

    public string Title { get; }

    public string Label { get; }

    public string Text { get; set; }

    #endregion

    #region Validation

    protected override ValidationResult ValidateImpl()
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
