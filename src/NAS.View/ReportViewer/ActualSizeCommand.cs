using System.Windows.Input;

namespace NAS.ReportViewer
{
  public static class ActualSizeCommand
  {
    public static RoutedCommand ActualSize { get; private set; }

    static ActualSizeCommand()
    {
      ActualSize = new RoutedUICommand("Actual size", "ActualSize", typeof(ActualSizeCommand));
    }
  }
}
