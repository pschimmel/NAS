using System.Diagnostics;
using NAS.Models.Entities;
using NAS.ViewModels.Base;
using NAS.Views.Controls;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for ShowFragnetView.xaml
  /// </summary>
  public partial class ShowFragnetView : IDialogContentView
  {
    public ShowFragnetView()
    {
      InitializeComponent();
    }

    public IDialogContentViewModel ViewModel
    {
      get => DataContext as IDialogContentViewModel;
      set
      {
        DataContext = value;
        if (value is ILayoutViewModel lvm)
        {
          IPrintableCanvas canvas;

          if (lvm.Layout is GanttLayout)
          {
            canvas = new StandaloneGanttCanvas() { DataContext = value };
          }
          else
          {
            Debug.Assert(lvm.Layout is PERTLayout);
            canvas = new StandalonePERTCanvas() { DataContext = value };
          }
          canvas.DataContext = lvm.Schedule;
          canvas.Layout = lvm.Layout;
          lvm.Canvas = canvas;
          canvas.Refresh();
        }
      }
    }
  }
}
