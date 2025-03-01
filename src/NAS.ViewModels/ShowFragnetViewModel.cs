using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class ShowFragnetViewModel : DialogContentViewModel, ILayoutViewModel
  {
    #region Fields


    #endregion

    #region Constructor

    public ShowFragnetViewModel(Schedule schedule, Fragnet fragnet)
      : base()
    {
      ArgumentNullException.ThrowIfNull(schedule);
      ArgumentNullException.ThrowIfNull(fragnet);

      Schedule = new ScheduleViewModel(schedule);

      Layout = new GanttLayout
      {
        FilterCombination = FilterCombinationType.Or
      };

      Layout.FilterDefinitions.Add(new FilterDefinition(ActivityProperty.Fragnet) { Relation = FilterRelation.EqualTo, ObjectString = fragnet.ID.ToString() });

      foreach (var a in Schedule.Schedule.Activities)
      {
        if (a.Distortions != null)
        {
          foreach (var d in a.Distortions.Where(x => x.Fragnet == fragnet))
          {
            Layout.FilterDefinitions.Add(new FilterDefinition(ActivityProperty.Number) { Relation = FilterRelation.EqualTo, ObjectString = a.ID.ToString() });
          }
        }
      }

      Layout.ActivityColumns.Add(new ActivityColumn(ActivityProperty.Number));
      Layout.ActivityColumns.Add(new ActivityColumn(ActivityProperty.Name));
      Layout.ActivityColumns.Add(new ActivityColumn(ActivityProperty.StartDate));
      Layout.ActivityColumns.Add(new ActivityColumn(ActivityProperty.FinishDate));
      Title = "Fragnet " + fragnet.Name;
    }

    #endregion

    #region Overwritten Members

    public override string Title { get; }

    public override string Icon => "Chart";

    public override DialogSize DialogSize => DialogSize.Fixed(800, 600);

    public override HelpTopic HelpTopicKey => HelpTopic.Fragnets;

    public override IEnumerable<IButtonViewModel> Buttons => new List<IButtonViewModel> { ButtonViewModel.CreateCloseButton() };

    #endregion

    #region Properties

    public IPrintableCanvas Canvas { get; set; }

    public Layout Layout { get; }

    public ScheduleViewModel Schedule { get; }

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
