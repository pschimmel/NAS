using System.Windows.Shapes;
using NAS.Model.Entities;

namespace NAS.View.Shapes
{
  public abstract class ActivityShapeBase : Shape, IActivityDiagram<Activity>
  {
    protected ActivityShapeBase(Activity activity)
    {
      Item = activity;
    }

    public Activity Item { get; private set; }
  }
}
