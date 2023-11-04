using System.Windows;
using NAS.Helpers;
using NAS.ViewModel;

namespace NAS
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      var vm = new MainViewModel();
      vm.ShowFastReport += (s, e) => FastReportHelper.ShowFastReport(e.Item.Schedule, e.Item.FileName, e.Item.Language);
      vm.EditFastReport += (s, e) => FastReportHelper.EditFastReport(e.Item.Schedule, e.Item.FileName, e.Item.Language);
    }
  }
}
