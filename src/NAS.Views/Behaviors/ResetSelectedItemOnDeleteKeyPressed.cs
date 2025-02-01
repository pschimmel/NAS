using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace NAS.Views.Behaviors
{
  public class ResetSelectedItemOnDeleteKeyPressed : Behavior<ComboBox>
  {
    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();
      AssociatedObject.PreviewKeyDown -= AssociatedObject_PreviewKeyDown;
    }

    private void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Delete)
      {
        AssociatedObject.SelectedItem = null;
      }
    }
  }
}
