using System.Windows;
using ES.Tools.Core.MVVM;
using Xceed.Wpf.Toolkit;

namespace NAS
{
  /// <summary>
  /// Interaction logic for WindowNewScheduleWizard.xaml
  /// </summary>
  public partial class WindowNewScheduleWizard : IView
  {
    public WindowNewScheduleWizard()
    {
      InitializeComponent();
    }

    public IViewModel ViewModel
    {
      get => DataContext as IViewModel;
      set => DataContext = value;
    }

    private void Wizard_PageChanged(object sender, RoutedEventArgs e)
    {
      if ((e.OriginalSource as Wizard).CurrentPage == LastPage)
      {
        if (DataContext is IValidatingViewModel)
        {
          (DataContext as IValidatingViewModel).Validate();
        }
      }
    }
  }
}
