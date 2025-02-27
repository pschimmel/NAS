using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using NAS.Models.Entities;
using NAS.ViewModels;
using NAS.Views.Shapes;

namespace NAS.Views.Helpers
{
  public abstract class DiagramCanvasBase : Canvas
  {
    protected ScheduleViewModel VM => DataContext as ScheduleViewModel;

    protected Schedule Schedule => VM?.Schedule;

    public abstract Layout Layout { get; set; }

    public abstract void Refresh();

    protected virtual Effect SelectionEffect => new DropShadowEffect() { BlurRadius = 7, Color = Colors.Blue, ShadowDepth = 0, Opacity = 1 };

    protected void Select<T, U>(U item) where T : FrameworkElement, IActivityDiagram<U> where U : class
    {
      Dispatcher.BeginInvoke(new Action(() =>
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
