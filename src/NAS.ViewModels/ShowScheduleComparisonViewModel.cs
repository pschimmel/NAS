using System.Diagnostics;
using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.Resources;
using NAS.ViewModels.Base;

namespace NAS.ViewModels
{
  public class ShowSchedulesComparisonViewModel : ViewModelBase, ILayoutViewModel
  {
    private readonly Layout _layout;

    public ShowSchedulesComparisonViewModel(Schedule schedule, Schedule baseline)
      : base()
    {
      Schedule = new ScheduleViewModel(schedule);
      _layout = new GanttLayout();
      Debug.Assert(baseline != null);

      if (baseline != null)
      {
        _layout.ActivityColumns.Add(new ActivityColumn(ActivityProperty.Number));
        _layout.ActivityColumns.Add(new ActivityColumn(ActivityProperty.Name));
        _layout.ActivityColumns.Add(new ActivityColumn(ActivityProperty.StartDate));
        _layout.ActivityColumns.Add(new ActivityColumn(ActivityProperty.FinishDate));
        _layout.VisibleBaselines.Add(new VisibleBaseline(baseline));
        Title = NASResources.ProjectComparison;
      }
    }

    #region Properties

    public IPrintableCanvas Canvas { get; set; }

    public Layout Layout { get; }

    public ScheduleViewModel Schedule { get; }

    public string Title { get; }

    public double Zoom
    {
      get => Schedule.Zoom * 4d;
      set => Schedule.Zoom = value / 4d;
    }

    #endregion

    #region IDisposable

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (disposing)
      {
        Schedule.Dispose();
      }
    }

    #endregion
  }
}
