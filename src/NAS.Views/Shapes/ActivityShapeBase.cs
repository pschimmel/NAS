using System.Windows.Shapes;
using NAS.ViewModels;

namespace NAS.Views.Shapes
{
  public abstract class ActivityShapeBase : Shape, IActivityDiagram<ActivityViewModel>
  {
    protected ActivityShapeBase(ActivityViewModel activity)
    {
      Item = activity;
    }

    public ActivityViewModel Item { get; private set; }
  }
}
