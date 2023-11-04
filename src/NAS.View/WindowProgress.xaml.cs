using System.Windows;
using ES.WPF.General;

namespace NAS
{
  /// <summary>
  /// Interaction logic for WindowProgress.xaml
  /// </summary>
  public partial class WindowProgress : Window
  {
    public WindowProgress(double value, string text)
    {
      InitializeComponent();
      progressBar.Value = value;
      textBlock.Text = text;
    }

    public void SetProgress(double value)
    {
      DispatcherWrapper.Default.BeginInvoke(() => progressBar.Value = value);
    }

    public void SetProgress(double value, string text)
    {
      DispatcherWrapper.Default.BeginInvoke(() =>
      {
        progressBar.Value = value;
        textBlock.Text = text;
      });
    }
  }
}
