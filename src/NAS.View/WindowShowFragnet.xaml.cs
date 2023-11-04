using System.Windows;
using ES.Tools.Core.MVVM;
using NAS.View.Gantt;
using NAS.ViewModel;

namespace NAS
{
  /// <summary>
  /// Interaction logic for WindowShowFragnet.xaml
  /// </summary>
  public partial class WindowShowFragnet : IView
  {
    public WindowShowFragnet()
    {
      InitializeComponent();
    }

    public IViewModel ViewModel
    {
      get => DataContext as IViewModel;
      set
      {
        DataContext = value;
        if (value is ShowFragnetViewModel fvm)
        {
          var canvas = new StandaloneGanttCanvas() { Layout = fvm.Layout, DataContext = value };
          fvm.Canvas = canvas;
          canvas.Refresh();
        }
        else if (value is ShowSchedulesComparisonViewModel svm)
        {
          var canvas = new StandaloneGanttCanvas() { Layout = svm.Layout, DataContext = value };
          svm.Canvas = canvas;
          canvas.Refresh();
        }
      }
    }

    private void buttonOK_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
    }
  }
}
