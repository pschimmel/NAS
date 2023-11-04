using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using NAS.Model.Entities;
using NAS.Model.Enums;
using NAS.ViewModel.Base;

namespace NAS.ViewModel
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

      _layout = new Layout();
      _layout.FilterCombination = FilterCombinationType.Or;
      _layout.FilterDefinitions.Add(new FilterDefinition { Property = ActivityProperty.Fragnet, Relation = FilterRelation.EqualTo, ObjectString = fragnet.Guid.ToString() });
      foreach (var a in _scheduleVM.Schedule.Activities)
      {
        if (a.Distortions != null)
        {
          foreach (var d in a.Distortions.Where(x => x.Fragnet == fragnet))
          {
            _layout.FilterDefinitions.Add(new FilterDefinition { Property = ActivityProperty.Number, Relation = FilterRelation.EqualTo, ObjectString = a.ID.ToString() });
          }
        }
      }
      _layout.ActivityColumns.Add(new ActivityColumn { Property = ActivityProperty.Number });
      _layout.ActivityColumns.Add(new ActivityColumn { Property = ActivityProperty.Name });
      _layout.ActivityColumns.Add(new ActivityColumn { Property = ActivityProperty.StartDate });
      _layout.ActivityColumns.Add(new ActivityColumn { Property = ActivityProperty.FinishDate });
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
