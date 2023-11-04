using NAS.Resources;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel
{
  public class GetTextViewModel : ViewModelBase, IValidatable
  {
    private readonly bool allowEmpty;

    public GetTextViewModel(string title, string label, string text = null, bool allowEmpty = false)
    {
      Title = title;
      Label = label;
      Text = text;
      this.allowEmpty = allowEmpty;
    }

    public string Title { get; }

    public string Label { get; }

    public string Text { get; set; }

    public string ErrorMessage { get; private set; }

    public bool HasErrors => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool Validate()
    {
      ErrorMessage = null;
      if (!allowEmpty && string.IsNullOrWhiteSpace(Text))
      {
        ErrorMessage = NASResources.MessageTextMayNotBeEmpty;
        UserNotificationService.Instance.Error(NASResources.MessageTextMayNotBeEmpty);
      }

      OnPropertyChanged(nameof(ErrorMessage));
      OnPropertyChanged(nameof(HasErrors));
      return !HasErrors;
    }
  }
}
