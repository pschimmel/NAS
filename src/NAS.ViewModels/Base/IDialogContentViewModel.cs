using ES.Tools.Core.MVVM;

namespace NAS.ViewModels.Base
{
  public interface IDialogContentViewModel : IViewModel, IApplyable
  {
    string Title { get; }

    string Icon { get; }

    DialogSize DialogSize { get; }

    IEnumerable<IButtonViewModel> Buttons { get; }

    bool OnClosing(bool? dialogResult);
  }
}