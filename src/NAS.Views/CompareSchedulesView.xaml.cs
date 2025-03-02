using NAS.ViewModels.Base;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for CompareSchedulesView.xaml
  /// </summary>
  public partial class CompareSchedulesView : IDialogContentView
  {
    public CompareSchedulesView()
    {
      InitializeComponent();
    }

    public IDialogContentViewModel ViewModel
    {
      get => DataContext as IDialogContentViewModel;
      set => DataContext = value;
    }
  }
}
