using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NAS.ReportViewer
{
  /// <summary>
  /// Interaction logic for UserControlPrintPreview.xaml
  /// </summary>
  public partial class UserControlPrintPreview : DocumentViewer
  {
    public UserControlPrintPreview()
    {
      InitializeComponent();
    }

    public ICommand PageSettingsCommand
    {
      get => (ICommand)GetValue(PageSettingsCommandProperty);
      set => SetValue(PageSettingsCommandProperty, value);
    }

    public static DependencyProperty PageSettingsCommandProperty = DependencyProperty.Register("PageSettingsCommand", typeof(ICommand), typeof(UserControlPrintPreview));

    public ICommand LayoutSettingsCommand
    {
      get => (ICommand)GetValue(LayoutSettingsCommandProperty);
      set => SetValue(LayoutSettingsCommandProperty, value);
    }

    public static DependencyProperty LayoutSettingsCommandProperty = DependencyProperty.Register("LayoutSettingsCommand", typeof(ICommand), typeof(UserControlPrintPreview));

    public ICommand PrintCommand
    {
      get => (ICommand)GetValue(PrintCommandProperty);
      set => SetValue(PrintCommandProperty, value);
    }

    public static DependencyProperty PrintCommandProperty = DependencyProperty.Register("PrintCommand", typeof(ICommand), typeof(UserControlPrintPreview));

    protected override void OnPrintCommand()
    {
      if (PrintCommand != null && PrintCommand.CanExecute(null))
      {
        PrintCommand.Execute(null);
      }
    }

    #region Workaround for known zoom bug in control template of document viewer

    private void ActualSize_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      e.Handled = true;
      NavigationCommands.Zoom.Execute(100.0, this);
    }

    #endregion
  }
}
