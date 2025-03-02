using System.Diagnostics;
using NAS.ViewModels;
using NAS.ViewModels.Base;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for CompareResultsView.xaml
  /// </summary>
  public partial class CompareResultsView : IDialogContentView
  {
    public CompareResultsView()
    {
      InitializeComponent();
    }

    public IDialogContentViewModel ViewModel
    {
      get => DataContext as IDialogContentViewModel;
      set
      {
        if (DataContext is CompareResultsViewModel)
          (DataContext as CompareResultsViewModel).OnPrintRequested -= PrintRequested;

        Debug.Assert(value is CompareResultsViewModel);
        DataContext = value;

        if (DataContext is CompareResultsViewModel)
          (DataContext as CompareResultsViewModel).OnPrintRequested += PrintRequested;
      }
    }

    private void PrintRequested(object sender, EventArgs e)
    {
      ComparisonDocument.Print();
    }
  }
}
