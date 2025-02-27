﻿using System.Diagnostics;
using System.Windows.Controls;
using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.Resources;
using NAS.ViewModels.Base;

namespace NAS.ViewModels
{
  public class ShowSchedulesComparisonViewModel : ViewModelBase
  {
    private readonly ScheduleViewModel vm;
    private readonly Layout layout;

    public ShowSchedulesComparisonViewModel(Schedule schedule, Schedule baseline)
      : base()
    {
      vm = new ScheduleViewModel(schedule);
      layout = new GanttLayout();
      Debug.Assert(baseline != null);

      if (baseline != null)
      {
        layout.ActivityColumns.Add(new ActivityColumn(ActivityProperty.Number));
        layout.ActivityColumns.Add(new ActivityColumn(ActivityProperty.Name));
        layout.ActivityColumns.Add(new ActivityColumn(ActivityProperty.StartDate));
        layout.ActivityColumns.Add(new ActivityColumn(ActivityProperty.FinishDate));
        layout.VisibleBaselines.Add(new VisibleBaseline(baseline));
        Title = NASResources.ProjectComparison;
      }
    }

    #region Properties

    public Canvas Canvas { get; set; }

    public Layout Layout { get; private set; }

    public string Title { get; private set; }

    public double Zoom
    {
      get => vm.Zoom * 4d;
      set => vm.Zoom = value / 4d;
    }

    #endregion

    #region IDisposable

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (disposing)
      {
        vm.Dispose();
      }
    }

    #endregion
  }
}
