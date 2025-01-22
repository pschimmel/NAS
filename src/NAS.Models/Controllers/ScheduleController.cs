using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.Resources;

namespace NAS.Models.Controllers
{
  public static class ScheduleController
  {
    public static void AddMinimumData(Schedule schedule)
    {
      if (schedule.Layouts.Count == 0)
      {
        var newLayout = new GanttLayout();
        newLayout.Name = NASResources.Layout + " 1";
        newLayout.IsCurrent = true;
        schedule.Layouts.Add(newLayout);
      }
      foreach (var layout in schedule.Layouts)
      {
        if (layout.ActivityColumns.Count == 0)
        {
          layout.ActivityColumns.Add(new ActivityColumn(ActivityProperty.Number));
          layout.ActivityColumns.Add(new ActivityColumn(ActivityProperty.Name));
          layout.ActivityColumns.Add(new ActivityColumn(ActivityProperty.OriginalDuration));
          layout.ActivityColumns.Add(new ActivityColumn(ActivityProperty.StartDate));
          layout.ActivityColumns.Add(new ActivityColumn(ActivityProperty.FinishDate));
          layout.ActivityColumns.Add(new ActivityColumn(ActivityProperty.PercentComplete));
          layout.ActivityColumns.Add(new ActivityColumn(ActivityProperty.TotalFloat));
        }
        if (layout.HeaderItems.Count == 0)
        {
          layout.HeaderItems.Add(new HeaderItem());
        }
        if (layout.FooterItems.Count == 0)
        {
          layout.FooterItems.Add(new FooterItem());
        }
        if (layout is PERTLayout pertLayout)
        {
          if (pertLayout.PERTDefinition == null)
          {
            if (schedule.PERTDefinitions.Count != 0)
            {
              pertLayout.PERTDefinition = schedule.PERTDefinitions.First();
            }
            else
            {
              var newDefinition = GetStandardPert();
              schedule.PERTDefinitions.Add(newDefinition);
              pertLayout.PERTDefinition = newDefinition;
            }
          }
        }
      }
      if (schedule.Calendars.Count == 0)
      {
        schedule.Calendars.Add(new Calendar() { Name = NASResources.StandardCalendar, IsStandard = true });
      }

      if (schedule.WBSItem == null)
      {
        var wbs = new WBSItem();
        wbs.Name = schedule.Name;
        wbs.Number = "1";
        schedule.WBSItem = wbs;
      }
    }

    public static PERTDefinition GetStandardPert()
    {
      var result = new PERTDefinition();
      result.Name = NASResources.Standard;
      result.ColumnDefinitions.Add(new ColumnDefinition());
      result.ColumnDefinitions.Add(new ColumnDefinition());
      result.ColumnDefinitions.Add(new ColumnDefinition());
      result.RowDefinitions.Add(new RowDefinition());
      result.RowDefinitions.Add(new RowDefinition());
      result.RowDefinitions.Add(new RowDefinition());
      result.Items.Add(new PERTDataItem() { Property = ActivityProperty.Number, Column = 0, Row = 0 });
      result.Items.Add(new PERTDataItem() { Property = ActivityProperty.Name, Column = 1, Row = 0, ColumnSpan = 2 });
      result.Items.Add(new PERTDataItem() { Property = ActivityProperty.EarlyStartDate, Column = 0, Row = 1 });
      result.Items.Add(new PERTDataItem() { Property = ActivityProperty.LateStartDate, Column = 0, Row = 2 });
      result.Items.Add(new PERTDataItem() { Property = ActivityProperty.EarlyFinishDate, Column = 2, Row = 1 });
      result.Items.Add(new PERTDataItem() { Property = ActivityProperty.LateFinishDate, Column = 2, Row = 2 });
      result.Items.Add(new PERTDataItem() { Property = ActivityProperty.OriginalDuration, Column = 1, Row = 1 });
      result.Items.Add(new PERTDataItem() { Property = ActivityProperty.TotalFloat, Column = 1, Row = 2 });
      return result;
    }

    public static void CheckValues(Schedule schedule)
    {
      if (schedule.DataDate == default)
      {
        schedule.DataDate = schedule.StartDate;
      }

      foreach (var activity in schedule.Activities)
      {
        activity.Calendar ??= schedule.StandardCalendar;

        if (activity.EarlyStartDate == default)
        {
          activity.EarlyStartDate = schedule.DataDate;
        }

        if (activity.EarlyFinishDate == default)
        {
          activity.EarlyFinishDate = activity.Calendar.GetEndDate(activity.EarlyStartDate, activity.RetardedDuration);
        }
        if (activity.LateStartDate == default)
        {
          activity.LateStartDate = schedule.DataDate;
        }

        if (activity.LateFinishDate == default)
        {
          activity.LateFinishDate = activity.Calendar.GetEndDate(activity.LateStartDate, activity.RetardedDuration);
        }
      }
    }
  }
}
