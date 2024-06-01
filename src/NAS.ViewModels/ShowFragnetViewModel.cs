using System.Windows.Controls;
using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.ViewModels.Base;

namespace NAS.ViewModels
{
  public class ShowFragnetViewModel : ViewModelBase
  {
    private readonly ScheduleViewModel _scheduleVM;
    private readonly Layout _layout;

    public ShowFragnetViewModel(Schedule schedule, Fragnet fragnet)
      : base()
    {
      if (schedule == null)
      {
        throw new ArgumentNullException(nameof(schedule), "Argument mustn't be null");
      }

      if (fragnet == null)
      {
        throw new ArgumentNullException(nameof(fragnet), "Argument mustn't be null");
      }

      _scheduleVM = new ScheduleViewModel(schedule);

      _layout = new GanttLayout();
      _layout.FilterCombination = FilterCombinationType.Or;
      _layout.FilterDefinitions.Add(new FilterDefinition(ActivityProperty.Fragnet) { Relation = FilterRelation.EqualTo, ObjectString = fragnet.ID.ToString() });
      foreach (var a in _scheduleVM.Schedule.Activities)
      {
        if (a.Distortions != null)
        {
          foreach (var d in a.Distortions.Where(x => x.Fragnet == fragnet))
          {
            _layout.FilterDefinitions.Add(new FilterDefinition(ActivityProperty.Number) { Relation = FilterRelation.EqualTo, ObjectString = a.ID.ToString() });
          }
        }
      }
      _layout.ActivityColumns.Add(new ActivityColumn(ActivityProperty.Number));
      _layout.ActivityColumns.Add(new ActivityColumn(ActivityProperty.Name));
      _layout.ActivityColumns.Add(new ActivityColumn(ActivityProperty.StartDate));
      _layout.ActivityColumns.Add(new ActivityColumn(ActivityProperty.FinishDate));
      Title = "Fragnet " + fragnet;
    }

    #region Public Properties

    public Canvas Canvas { get; set; }

    public Layout Layout { get; private set; }

    public string Title { get; private set; }

    public double Zoom
    {
      get => _scheduleVM.Zoom * 4d;
      set => _scheduleVM.Zoom = value / 4d;
    }

    #endregion

    #region IDisposable

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (disposing)
      {
        _scheduleVM.Dispose();
      }
    }

    #endregion
  }
}
