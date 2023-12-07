using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NAS.Model.Entities;

namespace NAS.View.Controls
{
  /// <summary>
  /// Interaction logic for UserControlPERT.xaml
  /// </summary>
  public partial class UserControlPERT : UserControl
  {
    #region Constructors

    public UserControlPERT()
    {
      InitializeComponent();
      canvas.RequestCheckPosition += Canvas_RequestCheckPosition;
      Loaded += UserControlPERT_Loaded;
    }

    private void UserControlPERT_Loaded(object sender, RoutedEventArgs e)
    {
      Loaded -= UserControlPERT_Loaded;

      _ = Keyboard.Focus(this);
    }

    #endregion

    #region Public Properties

    public Layout Layout
    {
      get => canvas.Layout;
      set => canvas.Layout = value;
    }

    #endregion

    #region Mouse Interaction

    private void Canvas_RequestCheckPosition(object sender, CheckPositionEventArgs e)
    {
      var point = Mouse.GetPosition(scrollViewer);
      if (scrollViewer.VerticalOffset > 0 && point.Y <= 20)
      {
        scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - 20);
        e.Result = false;
      }
      if (scrollViewer.VerticalOffset < scrollViewer.ViewportHeight && point.Y >= scrollViewer.ViewportHeight - 20)
      {
        scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + 20);
        e.Result = false;
      }
      if (point.X < 0 || point.Y <= 20 || point.X > scrollViewer.ViewportWidth || point.Y > scrollViewer.ViewportHeight)
      {
        canvas.ClearTempItems();
        e.Result = false;
      }
    }

    #endregion
  }
}
