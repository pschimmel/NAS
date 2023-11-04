using System;
using ES.Tools.Core.MVVM;

namespace NAS.ViewModel.Helpers
{
  public static class ViewModelExtensions
  {
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
