using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using NAS.Model.Entities;
using NAS.View.Shapes;

namespace NAS.View.Helpers
{
  public abstract class DiagramCanvasBase : Canvas
  {
    public abstract Layout Layout { get; set; }

    public abstract void Refresh();

    protected virtual Effect SelectionEffect => new DropShadowEffect() { BlurRadius = 7, Color = Colors.Blue, ShadowDepth = 0, Opacity = 1 };

    protected void Select<T, U>(U item) where T : FrameworkElement, IActivityDiagram<U> where U : class
    {
      _ = Dispatcher.BeginInvoke(new Action(() =>
      {
        Children.OfType<T>().ToList().ForEach(x => x.Effect = null);
        if (item != null)
        {
          var shape = Children.OfType<T>().FirstOrDefault(x => x.Item == item);
          if (shape != null)
          {
            shape.Effect = SelectionEffect;
          }
        }
      }), DispatcherPriority.DataBind);
    }
  }
}
