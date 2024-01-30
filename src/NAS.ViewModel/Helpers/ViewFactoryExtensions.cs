using ES.Tools.Core.MVVM;
using NAS.ViewModel.Base;

namespace NAS.ViewModel.Helpers
{
  public static class ViewFactoryExtensions
  {
    private static readonly Dictionary<Type, Type> _dialogs = [];

    public static bool? ShowDialog(this ViewFactory factory, IDialogContentViewModel viewModel)
    {
      var dialogVM = new DialogViewModel(viewModel);
      var view = factory.CreateView(dialogVM);

      return view.ShowDialog();
    }

    public static IDialogContentView CreateDialogContentView(this ViewFactory _, IDialogContentViewModel viewModel)
    {
      Type type = viewModel.GetType();
      Type viewType = _dialogs[type] ?? throw new InvalidOperationException("Unknown View for ViewModel object");
      var view = (IDialogContentView)Activator.CreateInstance(viewType);
      view.ViewModel = viewModel;
      return view;
    }

    public static void RegisterDialog<TViewModel, TView>(this ViewFactory _) where TViewModel : IDialogContentViewModel
                                                                             where TView : IDialogContentView
    {
      _dialogs[typeof(TViewModel)] = typeof(TView);
    }

    public static bool? ShowDialog(this ViewFactory factory, IViewModel vm)
    {
      if (vm == null)
      {
        throw new ArgumentException("ViewModel cannot be null.", nameof(vm));
      }

      var view = factory.CreateView(vm);
      return view.ShowDialog();
    }
  }
}
