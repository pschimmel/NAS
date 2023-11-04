using System.Windows;
using System.Windows.Documents;
using NAS.ViewModel;
using NAS.ViewModel.Helpers;

namespace NAS
{
  /// <summary>
  /// Interaction logic for WindowInstantHelp.xaml
  /// </summary>
  public partial class WindowInstantHelp : IHelpWindow
  {
    public WindowInstantHelp()
    {
      InitializeComponent();
    }

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);
      Left = SystemParameters.WorkArea.Width - Width;
      Top = SystemParameters.WorkArea.Height - Height;
    }

    public void SelectTopic(HelpTopic topic)
    {
      if (TryFindResource(topic) is FlowDocument x)
      {
        reader.Document = x;
      }
    }

    private void buttonPrint_Click(object sender, RoutedEventArgs e)
    {
      reader.Print();
    }
  }
}
