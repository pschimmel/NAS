using System.IO;
using System.Xml;
using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.Resources;

namespace NAS.Models.ImportExport
{
  public class NASFilter : IImportFilter, IExportFilter
  {
    #region Properties

    public string FilterName => NASResources.NASFiles;

    public string FileExtension => ".nas";

    public string Output => string.Empty;

    #endregion

    #region Import

    public Schedule Import(string fileName)
    {
      var schedule = new Schedule();

      if (!File.Exists(fileName))
      {
        throw new FileNotFoundException("File " + fileName + " not found!");
      }

      if (!string.Equals(Path.GetExtension(fileName), FileExtension, StringComparison.InvariantCultureIgnoreCase))
      {
        throw new Exception("Wrong file extension!");
      }

      var xml = new XmlDocument();
      xml.Load(fileName);
      if (xml.FirstChild.Name == "Project")
      {
        var projectNode = xml.FirstChild;
        ReadSchedule(schedule, projectNode);
      }
      return schedule;
    }

    private static void ReadSchedule(Schedule schedule, XmlNode projectNode)
    {
      foreach (XmlNode node in projectNode.ChildNodes)
      {
        if (node.Name == "Baselines")
        { // Read Baselines first
          foreach (XmlNode baselineNode in node.ChildNodes)
          {
            if (baselineNode.Name == "Baseline")
            {
              var b = new Schedule();
              ReadSchedule(b, baselineNode);
              schedule.Baselines.Add(b);
            }
          }
        }
      }
      foreach (XmlNode node in projectNode.ChildNodes)
      {
        if (node.Name == "Name")
        {
          schedule.Name = node.InnerText;
        }
        else if (node.Name == "Description")
        {
          schedule.Description = node.InnerText;
        }
        else if (node.Name == "StartDate" && node.GetDateTime().HasValue)
        {
          schedule.StartDate = node.GetDateTime().Value;
        }
        else if (node.Name == "EndDate" && node.GetDateTime().HasValue)
        {
          schedule.EndDate = node.GetDateTime().Value;
        }
        else if (node.Name == "DataDate" && node.GetDateTime().HasValue)
        {
          schedule.DataDate = node.GetDateTime().Value;
        }
        else if (node.Name == "Scheduling Settings")
        {
          // Read Scheduling Settings
          foreach (XmlNode settingsNode in node.ChildNodes)
          {
            if (settingsNode.Name == "CriticalPath" && settingsNode.TryGetEnum(out CriticalPathDefinition enumValue))
            {
              schedule.SchedulingSettings.CriticalPath = enumValue;
            }
          }
        }
        else if (node.Name == "Calendars")
        {
          // Read Calendars
          foreach (XmlNode calendarNode in node.ChildNodes)
          {
            if (calendarNode.Name == "Calendar")
            {
              var calendar = new Calendar();
              schedule.Calendars.Add(calendar);
              foreach (XmlNode exceptionSubNode in calendarNode.ChildNodes)
              {
                if (exceptionSubNode.Name == "Name")
                {
                  calendar.Name = exceptionSubNode.InnerText;
                }
                else if (exceptionSubNode.Name == "Sunday" && exceptionSubNode.GetBoolean().HasValue)
                {
                  calendar.Sunday = exceptionSubNode.GetBoolean().Value;
                }
                else if (exceptionSubNode.Name == "Monday" && exceptionSubNode.GetBoolean().HasValue)
                {
                  calendar.Monday = exceptionSubNode.GetBoolean().Value;
                }
                else if (exceptionSubNode.Name == "Tuesday" && exceptionSubNode.GetBoolean().HasValue)
                {
                  calendar.Tuesday = exceptionSubNode.GetBoolean().Value;
                }
                else if (exceptionSubNode.Name == "Wednesday" && exceptionSubNode.GetBoolean().HasValue)
                {
                  calendar.Wednesday = exceptionSubNode.GetBoolean().Value;
                }
                else if (exceptionSubNode.Name == "Thursday" && exceptionSubNode.GetBoolean().HasValue)
                {
                  calendar.Thursday = exceptionSubNode.GetBoolean().Value;
                }
                else if (exceptionSubNode.Name == "Friday" && exceptionSubNode.GetBoolean().HasValue)
                {
                  calendar.Friday = exceptionSubNode.GetBoolean().Value;
                }
                else if (exceptionSubNode.Name == "Saturday" && exceptionSubNode.GetBoolean().HasValue)
                {
                  calendar.Saturday = exceptionSubNode.GetBoolean().Value;
                }
                else if (exceptionSubNode.Name == "Holidays")
                {
                  foreach (XmlNode exceptionNode in exceptionSubNode.ChildNodes)
                  {
                    if (exceptionNode.Name == "Holiday" && exceptionNode.GetDateTime().HasValue)
                    {
                      calendar.Holidays.Add(new Holiday() { Date = exceptionNode.GetDateTime().Value });
                    }
                  }
                }
              }
            }
          }
        }
        else if (node.Name == "StandardCalendar" && node.GetInteger().HasValue && node.GetInteger().Value >= 0 && node.GetInteger().Value < schedule.Calendars.Count)
        {
          schedule.StandardCalendar = schedule.Calendars.ToList()[node.GetInteger().Value];
        }
        else if (node.Name == "CustomAttributes1")
        { // Read CustomAttributes
          foreach (XmlNode attributeNode in node.ChildNodes)
          {
            if (attributeNode.Name == "CustomAttribute")
            {
              var customAttribute = new CustomAttribute();
              schedule.CustomAttributes1.Add(customAttribute);
              foreach (XmlNode attributeSubNode in attributeNode.ChildNodes)
              {
                if (attributeSubNode.Name == "Name")
                {
                  customAttribute.Name = attributeSubNode.InnerText;
                }
              }
            }
          }
        }
        else if (node.Name == "CustomAttributes2")
        { // Read CustomAttributes
          foreach (XmlNode attributeNode in node.ChildNodes)
          {
            if (attributeNode.Name == "CustomAttribute")
            {
              var customAttribute = new CustomAttribute();
              schedule.CustomAttributes2.Add(customAttribute);
              foreach (XmlNode attributeSubNode in attributeNode.ChildNodes)
              {
                if (attributeSubNode.Name == "Name")
                {
                  customAttribute.Name = attributeSubNode.InnerText;
                }
              }
            }
          }
        }
        else if (node.Name == "CustomAttributes3")
        { // Read CustomAttributes
          foreach (XmlNode attributeNode in node.ChildNodes)
          {
            if (attributeNode.Name == "CustomAttribute")
            {
              var customAttribute = new CustomAttribute();
              schedule.CustomAttributes3.Add(customAttribute);
              foreach (XmlNode attributeSubNode in attributeNode.ChildNodes)
              {
                if (attributeSubNode.Name == "Name")
                {
                  customAttribute.Name = attributeSubNode.InnerText;
                }
              }
            }
          }
        }
        else if (node.Name == "Fragnets")
        { // Read Fragnets
          foreach (XmlNode fragnetNode in node.ChildNodes)
          {
            if (fragnetNode.Name == "Fragnet")
            {
              var fragnet = new Fragnet();
              schedule.Fragnets.Add(fragnet);
              foreach (XmlNode fragnetSubNode in fragnetNode.ChildNodes)
              {
                if (fragnetSubNode.Name == "ID")
                {
                  fragnet.Number = fragnetSubNode.InnerText;
                }
                else if (fragnetSubNode.Name == "Name")
                {
                  fragnet.Name = fragnetSubNode.InnerText;
                }
                else if (fragnetSubNode.Name == "Description")
                {
                  fragnet.Description = fragnetSubNode.InnerText;
                }
                else if (fragnetSubNode.Name == "Approved")
                {
                  fragnet.Approved = fragnetSubNode.GetDateTime();
                }
                else if (fragnetSubNode.Name == "Identified" && fragnetSubNode.GetDateTime().HasValue)
                {
                  fragnet.Identified = fragnetSubNode.GetDateTime().Value;
                }
                else if (fragnetSubNode.Name == "IsDisputable" && fragnetSubNode.GetBoolean().HasValue)
                {
                  fragnet.IsDisputable = fragnetSubNode.GetBoolean().Value;
                }
                else if (fragnetSubNode.Name == "Submitted")
                {
                  fragnet.Submitted = fragnetSubNode.GetDateTime();
                }
                else if (fragnetSubNode.Name == "IsVisible" && fragnetSubNode.GetBoolean().HasValue)
                {
                  fragnet.IsVisible = fragnetSubNode.GetBoolean().Value;
                }
              }
            }
          }
        }
        else if (node.Name == "WBS")
        {
          var wbs = new WBSItem();
          ReadWBSItems(node, wbs);
          schedule.WBSItem = wbs;
        }
        else if (node.Name == "Resources")
        { // Read Resources
          foreach (XmlNode resourceNode in node.ChildNodes)
          {
            Resource resource = null;
            if (resourceNode.Name == "MaterialResource")
            {
              resource = new MaterialResource();
            }
            else if (resourceNode.Name == "WorkResource")
            {
              resource = new WorkResource();
            }
            else if (resourceNode.Name == "CalendarResource")
            {
              resource = new CalendarResource();
            }

            if (resource != null)
            {
              schedule.Resources.Add(resource);
              foreach (XmlNode resourceSubNode in resourceNode.ChildNodes)
              {
                if (resourceSubNode.Name == "Name")
                {
                  resource.Name = resourceSubNode.InnerText;
                }
                else if (resourceSubNode.Name == "Unit" && resource is MaterialResource)
                {
                  (resource as MaterialResource).Unit = resourceSubNode.InnerText;
                }
                else if (resourceSubNode.Name == "Limit")
                {
                  resource.Limit = resourceSubNode.GetDouble();
                }
                else if (resourceSubNode.Name == "CostsPerUnit" && resourceSubNode.GetDecimal().HasValue)
                {
                  resource.CostsPerUnit = resourceSubNode.GetDecimal().Value;
                }
              }
            }
          }
        }
        else if (node.Name == "Activities")
        { // Read Activities
          foreach (XmlNode activityNode in node.ChildNodes)
          {
            Activity activity = null;
            if (activityNode.Name is "Activity" or "Milestone")
            {
              activity = activityNode.Name == "Activity" ? Activity.NewActivity(schedule) : Activity.NewMilestone(schedule);
              schedule.AddActivity(activity);
              foreach (XmlNode activitySubNode in activityNode.ChildNodes)
              {
                if (activitySubNode.Name == "ID")
                {
                  activity.Number = activitySubNode.InnerText;
                }
                else if (activitySubNode.Name == "Name")
                {
                  activity.Name = activitySubNode.InnerText;
                }
                else if (activitySubNode.Name == "EarlyStartDate" && activitySubNode.GetDateTime().HasValue)
                {
                  activity.EarlyStartDate = activitySubNode.GetDateTime().Value;
                }
                else if (activitySubNode.Name == "LateStartDate" && activitySubNode.GetDateTime().HasValue)
                {
                  activity.LateStartDate = activitySubNode.GetDateTime().Value;
                }
                else if (activitySubNode.Name == "OriginalDuration" && activitySubNode.GetInteger().HasValue)
                {
                  activity.OriginalDuration = activitySubNode.GetInteger().Value;
                }
                else if (activitySubNode.Name == "Fragnet" && schedule.Fragnets.Any(x => x.Number == activitySubNode.InnerText))
                {
                  activity.Fragnet = schedule.Fragnets.First(x => x.Number == activitySubNode.InnerText);
                }
                else if (activitySubNode.Name == "Calendar" && activitySubNode.GetInteger().HasValue && activitySubNode.GetInteger().Value >= 0 && activitySubNode.GetInteger().Value < schedule.Calendars.Count)
                {
                  activity.Calendar = schedule.Calendars.ToList()[activitySubNode.GetInteger().Value];
                }
                else if (activitySubNode.Name == "CustomAttribute1" && activitySubNode.GetInteger().HasValue && activitySubNode.GetInteger().Value >= 0 && activitySubNode.GetInteger().Value < schedule.CustomAttributes1.Count)
                {
                  activity.CustomAttribute1 = schedule.CustomAttributes1.ToList()[activitySubNode.GetInteger().Value];
                }
                else if (activitySubNode.Name == "CustomAttribute2" && activitySubNode.GetInteger().HasValue && activitySubNode.GetInteger().Value >= 0 && activitySubNode.GetInteger().Value < schedule.CustomAttributes2.Count)
                {
                  activity.CustomAttribute2 = schedule.CustomAttributes2.ToList()[activitySubNode.GetInteger().Value];
                }
                else if (activitySubNode.Name == "CustomAttribute3" && activitySubNode.GetInteger().HasValue && activitySubNode.GetInteger().Value >= 0 && activitySubNode.GetInteger().Value < schedule.CustomAttributes3.Count)
                {
                  activity.CustomAttribute3 = schedule.CustomAttributes3.ToList()[activitySubNode.GetInteger().Value];
                }
                else if (activitySubNode.Name == "ActualStartDate")
                {
                  activity.ActualStartDate = activitySubNode.GetDateTime();
                }
                else if (activitySubNode.Name == "ActualFinishDate")
                {
                  activity.ActualFinishDate = activitySubNode.GetDateTime();
                }
                else if (activitySubNode.Name == "PercentComplete" && activitySubNode.GetDouble().HasValue)
                {
                  activity.PercentComplete = activitySubNode.GetDouble().Value;
                }
                else if (activitySubNode.Name == "TotalFloat" && activitySubNode.GetInteger().HasValue)
                {
                  activity.TotalFloat = activitySubNode.GetInteger().Value;
                }
                else if (activitySubNode.Name == "FreeFloat" && activitySubNode.GetInteger().HasValue)
                {
                  activity.FreeFloat = activitySubNode.GetInteger().Value;
                }
                else if (activitySubNode.Name == "WBS" && schedule.WBSItem != null)
                {
                  var item = FindWBSItem(schedule.WBSItem, activitySubNode.InnerText);
                  activity.WBSItem = item;
                }
                else if (activitySubNode.Name == "Constraints")
                {
                  foreach (XmlNode constraintNode in activitySubNode.ChildNodes)
                  {
                    if (constraintNode.Name == "Constraint")
                    {
                      foreach (XmlNode constraintSubNode in constraintNode.ChildNodes)
                      {
                        if (constraintSubNode.Name == "ConstraintType")
                        {
                          if (Enum.TryParse<ConstraintType>(constraintSubNode.InnerText, out var t))
                          {
                            activity.Constraint = t;
                          }
                        }
                        else if (constraintSubNode.Name == "ConstraintDate")
                        {
                          activity.ConstraintDate = constraintSubNode.GetDateTime();
                        }
                      }
                    }
                  }
                }
                else if (activitySubNode.Name == "Resources")
                {
                  foreach (XmlNode resourceNode in activitySubNode.ChildNodes)
                  {
                    if (resourceNode.Name == "Resource")
                    {
                      ResourceAssignment ra = null;
                      foreach (XmlNode resourceSubNode in resourceNode.ChildNodes)
                      {
                        if (resourceSubNode.Name == "ResourceID" && resourceSubNode.GetInteger().HasValue && resourceSubNode.GetInteger().Value >= 0 && resourceSubNode.GetInteger().Value < schedule.Resources.Count)
                        {
                          ra = new ResourceAssignment(activity, schedule.Resources.ToList()[resourceSubNode.GetInteger().Value]);
                        }
                      }
                      if (ra != null)
                      {
                        activity.ResourceAssignments.Add(ra);
                        foreach (XmlNode resourceSubNode in resourceNode.ChildNodes)
                        {
                          if (resourceSubNode.Name == "FixedCosts" && resourceSubNode.GetDecimal().HasValue)
                          {
                            ra.FixedCosts = resourceSubNode.GetDecimal().Value;
                          }
                          else if (resourceSubNode.Name == "UnitsPerDay" && resourceSubNode.GetDouble().HasValue)
                          {
                            ra.UnitsPerDay = resourceSubNode.GetDouble().Value;
                          }
                          else if (resourceSubNode.Name == "Budget" && resourceSubNode.GetDecimal().HasValue)
                          {
                            ra.Budget = resourceSubNode.GetDecimal().Value;
                          }
                        }
                      }
                    }
                  }
                }
                else if (activitySubNode.Name == "Distortions")
                {
                  foreach (XmlNode distortionNode in activitySubNode.ChildNodes)
                  {
                    Distortion distortion = null;
                    if (distortionNode.Name == "Delay")
                    {
                      distortion = new Delay(activity);
                      foreach (XmlNode distortionSubNode in distortionNode.ChildNodes)
                      {
                        if (distortionSubNode.Name == "Days" && distortionSubNode.GetInteger().HasValue)
                        {
                          (distortion as Delay).Days = distortionSubNode.GetInteger().Value;
                        }
                      }
                    }
                    else if (distortionNode.Name == "Interruption")
                    {
                      distortion = new Interruption(activity);
                      foreach (XmlNode distortionSubNode in distortionNode.ChildNodes)
                      {
                        if (distortionSubNode.Name == "Days" && distortionSubNode.GetInteger().HasValue)
                        {
                          (distortion as Interruption).Days = distortionSubNode.GetInteger().Value;
                        }
                        else if (distortionSubNode.Name == "Days" && distortionSubNode.GetDateTime().HasValue)
                        {
                          (distortion as Interruption).Start = distortionSubNode.GetDateTime().Value;
                        }
                      }
                    }
                    else if (distortionNode.Name == "Inhibition")
                    {
                      distortion = new Inhibition(activity);
                      foreach (XmlNode distortionSubNode in distortionNode.ChildNodes)
                      {
                        if (distortionSubNode.Name == "Percent" && distortionSubNode.GetDouble().HasValue)
                        {
                          (distortion as Inhibition).Percent = distortionSubNode.GetDouble().Value;
                        }
                      }
                    }
                    else if (distortionNode.Name == "Extension")
                    {
                      distortion = new Extension(activity);
                      foreach (XmlNode distortionSubNode in distortionNode.ChildNodes)
                      {
                        if (distortionSubNode.Name == "Days" && distortionSubNode.GetInteger().HasValue)
                        {
                          (distortion as Extension).Days = distortionSubNode.GetInteger().Value;
                        }
                      }
                    }
                    else if (distortionNode.Name == "Reduction")
                    {
                      distortion = new Reduction(activity);
                      foreach (XmlNode distortionSubNode in distortionNode.ChildNodes)
                      {
                        if (distortionSubNode.Name == "Days" && distortionSubNode.GetInteger().HasValue)
                        {
                          (distortion as Reduction).Days = distortionSubNode.GetInteger().Value;
                        }
                      }
                    }
                    if (distortion != null)
                    {
                      foreach (XmlNode distortionSubNode in distortionNode.ChildNodes)
                      {
                        if (distortionSubNode.Name == "Description")
                        {
                          distortion.Description = distortionSubNode.InnerText;
                        }
                        else if (distortionSubNode.Name == "Fragnet" && distortionSubNode.GetInteger().HasValue && distortionSubNode.GetInteger().Value >= 0)
                        {
                          distortion.Fragnet = schedule.Fragnets.ToList()[distortionSubNode.GetInteger().Value];
                        }
                      }
                      activity.Distortions.Add(distortion);
                    }
                  }
                }
              }
            }
            activity.Calendar ??= schedule.StandardCalendar;
          }
        }
        else if (node.Name == "Relationships")
        { // Read Relationships
          foreach (XmlNode relationshipNode in node.ChildNodes)
          {
            Activity activity1 = null;
            Activity activity2 = null;
            int lag = 0;
            var type = RelationshipType.FinishStart;
            if (relationshipNode.Name == "Relationship")
            {
              foreach (XmlNode relationshipSubNode in relationshipNode.ChildNodes)
              {
                if (relationshipSubNode.Name == "Activity1")
                {
                  activity1 = schedule.Activities.First(x => x.Number == relationshipSubNode.InnerText);
                }
                else if (relationshipSubNode.Name == "Activity2")
                {
                  activity2 = schedule.Activities.First(x => x.Number == relationshipSubNode.InnerText);
                }
                else if (relationshipSubNode.Name == "Lag" && relationshipSubNode.GetInteger().HasValue)
                {
                  lag = relationshipSubNode.GetInteger().Value;
                }
                else if (relationshipSubNode.Name == "DependencyType")
                {
                  Enum.TryParse(relationshipSubNode.InnerText, out type);
                }
              }
            }
            if (activity1 != null && activity2 != null)
            {
              var r = schedule.AddRelationship(activity1, activity2);
              r.Lag = lag;
              r.RelationshipType = type;
            }
          }
        }
        else if (node.Name == "PERTDefinitions")
        { // Read PERT Definitions
          foreach (XmlNode pertNode in node.ChildNodes)
          {
            if (pertNode.Name == "PERTDefinition")
            {
              var d = new PERTDefinition(schedule);
              schedule.PERTDefinitions.Add(d);
              foreach (XmlNode pertSubNode in pertNode.ChildNodes)
              {
                if (pertSubNode.Name == "Name")
                {
                  d.Name = pertSubNode.InnerText;
                }
                else if (pertSubNode.Name == "Width" && pertSubNode.GetDouble().HasValue)
                {
                  d.Width = pertSubNode.GetDouble().Value;
                }
                else if (pertSubNode.Name == "Height" && pertSubNode.GetDouble().HasValue)
                {
                  d.Height = pertSubNode.GetDouble().Value;
                }
                else if (pertSubNode.Name == "FontSize" && pertSubNode.GetDouble().HasValue)
                {
                  d.FontSize = pertSubNode.GetDouble().Value;
                }
                else if (pertSubNode.Name == "SpacingX" && pertSubNode.GetDouble().HasValue)
                {
                  d.SpacingX = pertSubNode.GetDouble().Value;
                }
                else if (pertSubNode.Name == "SpacingY" && pertSubNode.GetDouble().HasValue)
                {
                  d.SpacingY = pertSubNode.GetDouble().Value;
                }
                else if (pertSubNode.Name == "Rows")
                {
                  foreach (XmlNode rowNode in pertSubNode.ChildNodes)
                  {
                    if (rowNode.Name == "Row")
                    {
                      var r = new RowDefinition();
                      d.RowDefinitions.Add(r);
                      foreach (XmlNode rowSubNode in rowNode.ChildNodes)
                      {
                        if (rowSubNode.Name == "Height" && rowSubNode.GetDouble().HasValue)
                        {
                          r.Height = rowSubNode.GetDouble().Value;
                        }
                      }
                    }
                  }
                }
                else if (pertSubNode.Name == "Columns")
                {
                  foreach (XmlNode colNode in pertSubNode.ChildNodes)
                  {
                    if (colNode.Name == "Column")
                    {
                      var r = new ColumnDefinition();
                      d.ColumnDefinitions.Add(r);
                      foreach (XmlNode colSubNode in colNode.ChildNodes)
                      {
                        if (colSubNode.Name == "Width" && colSubNode.GetDouble().HasValue)
                        {
                          r.Width = colSubNode.GetDouble().Value;
                        }
                      }
                    }
                  }
                }
                else if (pertSubNode.Name == "Items")
                {
                  foreach (XmlNode itemNode in pertSubNode.ChildNodes)
                  {
                    if (itemNode.Name == "Item")
                    {
                      var i = new PERTDataItem();
                      d.Items.Add(i);
                      foreach (XmlNode itemSubNode in itemNode.ChildNodes)
                      {
                        if (itemSubNode.Name == "Row" && itemSubNode.GetInteger().HasValue)
                        {
                          i.Row = itemSubNode.GetInteger().Value;
                        }
                        else if (itemSubNode.Name == "Column" && itemSubNode.GetInteger().HasValue)
                        {
                          i.Column = itemSubNode.GetInteger().Value;
                        }
                        else if (itemSubNode.Name == "RowSpan" && itemSubNode.GetInteger().HasValue)
                        {
                          i.RowSpan = itemSubNode.GetInteger().Value;
                        }
                        else if (itemSubNode.Name == "ColumnSpan" && itemSubNode.GetInteger().HasValue)
                        {
                          i.ColumnSpan = itemSubNode.GetInteger().Value;
                        }
                        else if (itemSubNode.Name == "HorizontalAlignment" && itemSubNode.GetEnum<HorizontalAlignment>().HasValue)
                        {
                          i.HorizontalAlignment = itemSubNode.GetEnum<HorizontalAlignment>().Value;
                        }
                        else if (itemSubNode.Name == "VerticalAlignment" && itemSubNode.GetEnum<VerticalAlignment>().HasValue)
                        {
                          i.VerticalAlignment = itemSubNode.GetEnum<VerticalAlignment>().Value;
                        }
                        else if (itemSubNode.Name == "Property" && itemSubNode.GetEnum<ActivityProperty>().HasValue)
                        {
                          i.Property = itemSubNode.GetEnum<ActivityProperty>().Value;
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
        else if (node.Name == "Layouts")
        { // Read Layouts
          foreach (XmlNode layoutNode in node.ChildNodes)
          {
            if (layoutNode.Name == "Layout")
            {
              Layout l = null;

              foreach (XmlNode layoutSubNode in layoutNode.ChildNodes)
              {
                if (layoutSubNode.Name == "IsPert" && layoutSubNode.GetBoolean().HasValue)
                {
                  l = new PERTLayout();
                  break;
                }
              }

              l ??= new GanttLayout();

              schedule.Layouts.Add(l);

              GanttLayout ganttLayout = l as GanttLayout;
              PERTLayout pertLayout = l as PERTLayout;

              foreach (XmlNode layoutSubNode in layoutNode.ChildNodes)
              {
                if (layoutSubNode.Name == "Name")
                {
                  l.Name = layoutSubNode.InnerText;
                }
                else if (layoutSubNode.Name == "ActivityStandardColor")
                {
                  l.ActivityStandardColor = layoutSubNode.GetColor();
                }
                else if (layoutSubNode.Name == "ActivityCriticalColor")
                {
                  l.ActivityCriticalColor = layoutSubNode.GetColor();
                }
                else if (layoutSubNode.Name == "ActivityDoneColor")
                {
                  l.ActivityDoneColor = layoutSubNode.GetColor();
                }
                else if (layoutSubNode.Name == "MilestoneStandardColor")
                {
                  l.MilestoneStandardColor = layoutSubNode.GetColor();
                }
                else if (layoutSubNode.Name == "MilestoneCriticalColor")
                {
                  l.MilestoneCriticalColor = layoutSubNode.GetColor();
                }
                else if (layoutSubNode.Name == "MilestoneDoneColor")
                {
                  l.MilestoneDoneColor = layoutSubNode.GetColor();
                }
                else if (layoutSubNode.Name == "BaselineColor" && schedule.VisibleBaselines.Count > 0)
                {
                  schedule.VisibleBaselines.First().Color = layoutSubNode.GetColor();
                }
                else if (layoutSubNode.Name == "PERTDefinition" && layoutSubNode.GetInteger().HasValue && pertLayout != null)
                {
                  pertLayout.PERTDefinition = schedule.PERTDefinitions.ToList()[layoutSubNode.GetInteger().Value];
                }
                else if (layoutSubNode.Name == "VisibleBaselines")
                {
                  foreach (XmlNode baselineNode in layoutSubNode.ChildNodes)
                  {
                    if (baselineNode.Name == "VisibleBaseline")
                    {
                      Schedule baseline = null;
                      string color = null;
                      foreach (XmlNode baselineSubNode in baselineNode.ChildNodes)
                      {
                        if (baselineSubNode.Name == "Color")
                        {
                          color = baselineSubNode.GetColor();
                        }
                        else if (baselineSubNode.Name == "Project" && baselineSubNode.GetInteger().HasValue)
                        {
                          baseline = schedule.Baselines.ToList()[baselineSubNode.GetInteger().Value];
                        }
                      }
                      if (baseline != null)
                      {
                        var v = new VisibleBaseline(schedule)
                        {
                          Color = color
                        };
                        l.VisibleBaselines.Add(v);
                      }
                    }
                  }
                }
                else if (layoutSubNode.Name == "ShowBaseline" && layoutSubNode.GetBoolean().HasValue && schedule.Baselines.Count > 0)
                {
                  var v = new VisibleBaseline(schedule.Baselines.First());
                  l.VisibleBaselines.Add(v);
                }
                else if (layoutSubNode.Name == "ShowRelationships" && layoutSubNode.GetBoolean().HasValue)
                {
                  l.ShowRelationships = layoutSubNode.GetBoolean().Value;
                }
                else if (layoutSubNode.Name == "ShowFloat" && layoutSubNode.GetBoolean().HasValue)
                {
                  l.ShowFloat = layoutSubNode.GetBoolean().Value;
                }
                else if (layoutSubNode.Name == "VisibleColumns")
                {
                  foreach (XmlNode columnNode in layoutSubNode.ChildNodes)
                  {
                    if (columnNode.Name == "ActivityColumn")
                    {
                      var a = ActivityProperty.None;
                      double? width = null;
                      foreach (XmlNode columnSubNode in columnNode.ChildNodes)
                      {
                        if (columnSubNode.Name == "Property")
                        {
                          Enum.TryParse(columnSubNode.InnerText, out a);
                        }
                        else if (columnSubNode.Name == "ColumnWidth")
                        {
                          width = columnSubNode.GetDouble();
                        }

                        if (a != ActivityProperty.None)
                        {
                          var col = new ActivityColumn(a)
                          {
                            ColumnWidth = width
                          };
                          l.ActivityColumns.Add(col);
                        }
                      }
                    }
                  }
                }
                else if (layoutSubNode.Name == "SortingDefinitions")
                {
                  foreach (XmlNode sortingDefinitionNode in layoutSubNode.ChildNodes)
                  {
                    if (sortingDefinitionNode.Name == "SortingDefinition")
                    {
                      var a = ActivityProperty.None;
                      var s = SortDirection.Ascending;
                      foreach (XmlNode sortingDefinitionSubNode in sortingDefinitionNode.ChildNodes)
                      {
                        if (sortingDefinitionSubNode.Name == "Property")
                        {
                          Enum.TryParse(sortingDefinitionSubNode.InnerText, out a);
                        }
                        else if (sortingDefinitionSubNode.Name == "Direction")
                        {
                          Enum.TryParse(sortingDefinitionSubNode.InnerText, out s);
                        }
                      }
                      if (a != ActivityProperty.None)
                      {
                        var def = new SortingDefinition(a)
                        {
                          Direction = s
                        };
                        l.SortingDefinitions.Add(def);
                      }
                    }
                  }
                }
                else if (layoutSubNode.Name == "GroupingDefinitions")
                {
                  foreach (XmlNode groupingDefinitionNode in layoutSubNode.ChildNodes)
                  {
                    if (groupingDefinitionNode.Name == "GroupingDefinition")
                    {
                      var a = ActivityProperty.None;
                      string c = "Black";
                      foreach (XmlNode groupingDefinitionSubNode in groupingDefinitionNode.ChildNodes)
                      {
                        if (groupingDefinitionSubNode.Name == "Property")
                        {
                          Enum.TryParse(groupingDefinitionSubNode.InnerText, out a);
                        }
                        else if (groupingDefinitionSubNode.Name == "Color")
                        {
                          c = groupingDefinitionSubNode.GetColor();
                        }
                      }
                      if (a != ActivityProperty.None)
                      {
                        var def = new GroupingDefinition(a)
                        {
                          Color = c
                        };
                        l.GroupingDefinitions.Add(def);
                      }
                    }
                  }
                }
                else if (layoutSubNode.Name == "LeftText" && ganttLayout != null)
                {
                  var property = ActivityProperty.None;
                  if (Enum.TryParse(layoutSubNode.InnerText, out property))
                  {
                    ganttLayout.LeftText = property;
                  }
                }
                else if (layoutSubNode.Name == "CenterText" && ganttLayout != null)
                {
                  var property = ActivityProperty.None;
                  if (Enum.TryParse(layoutSubNode.InnerText, out property))
                  {
                    ganttLayout.CenterText = property;
                  }
                }
                else if (layoutSubNode.Name == "RightText" && ganttLayout != null)
                {
                  var property = ActivityProperty.None;
                  if (Enum.TryParse(layoutSubNode.InnerText, out property))
                  {
                    ganttLayout.RightText = property;
                  }
                }
                else if (layoutSubNode.Name == "FilterCombination")
                {
                  var property = FilterCombinationType.And;
                  if (Enum.TryParse(layoutSubNode.InnerText, out property))
                  {
                    l.FilterCombination = property;
                  }
                }
                else if (layoutSubNode.Name == "Filters")
                {
                  foreach (XmlNode filterNode in layoutSubNode.ChildNodes)
                  {
                    if (filterNode.Name == "FilterDefinition")
                    {
                      var property = ActivityProperty.None;
                      var relation = FilterRelation.EqualTo;
                      string obj = null;
                      foreach (XmlNode filterSubNode in filterNode.ChildNodes)
                      {
                        if (filterSubNode.Name == "Property")
                        {
                          Enum.TryParse(filterSubNode.InnerText, out property);
                        }
                        else if (filterSubNode.Name == "Relation")
                        {
                          Enum.TryParse(filterSubNode.InnerText, out relation);
                        }
                        else if (filterSubNode.Name == "Object")
                        {
                          obj = filterSubNode.InnerText;
                        }
                      }
                      var f = new FilterDefinition(property)
                      {
                        Relation = relation,
                        ObjectString = obj
                      };
                      l.FilterDefinitions.Add(f);
                    }
                  }
                }
                else if (layoutSubNode.Name == "VisibleResources")
                {
                  foreach (XmlNode resourceNode in layoutSubNode.ChildNodes)
                  {
                    if (resourceNode.Name == "VisibleResource")
                    {
                      Resource r = null;
                      bool b1 = false, b2 = false, b3 = false, b4 = true;
                      foreach (XmlNode resourceSubNode in resourceNode.ChildNodes)
                      {
                        if (resourceSubNode.Name == "ID" && resourceSubNode.GetInteger().HasValue && resourceSubNode.GetInteger().Value >= 0 && resourceSubNode.GetInteger().Value < schedule.Resources.Count)
                        {
                          r = schedule.Resources.ToList()[resourceSubNode.GetInteger().Value];
                        }
                        else if (resourceSubNode.Name == "ShowBudget" && resourceSubNode.GetBoolean().HasValue)
                        {
                          b1 = resourceSubNode.GetBoolean().Value;
                        }
                        else if (resourceSubNode.Name == "ShowActualCosts" && resourceSubNode.GetBoolean().HasValue)
                        {
                          b2 = resourceSubNode.GetBoolean().Value;
                        }
                        else if (resourceSubNode.Name == "ShowPlannedCosts" && resourceSubNode.GetBoolean().HasValue)
                        {
                          b3 = resourceSubNode.GetBoolean().Value;
                        }
                        else if (resourceSubNode.Name == "ShowResourceAllocation" && resourceSubNode.GetBoolean().HasValue)
                        {
                          b4 = resourceSubNode.GetBoolean().Value;
                        }
                      }
                      if (r != null)
                      {
                        var v = new VisibleResource(l, r);
                        l.VisibleResources.Add(v);
                        v.ShowBudget = b1;
                        v.ShowActualCosts = b2;
                        v.ShowPlannedCosts = b3;
                        v.ShowResourceAllocation = b4;
                      }
                    }
                  }
                }
              }
            }
          }
        }
        else if (node.Name == "ActiveLayout" && node.GetInteger().HasValue)
        { // Read Current Layout
          schedule.ActiveLayout = schedule.Layouts.ToList()[node.GetInteger().Value];
        }
      }
    }

    private static void ReadWBSItems(XmlNode parentNode, WBSItem parent)
    {
      foreach (XmlNode node in parentNode.ChildNodes)
      {
        if (node.Name == "Number")
        {
          parent.Number = node.InnerText;
        }
        else if (node.Name == "Name")
        {
          parent.Name = node.InnerText;
        }
        else if (node.Name == "Items")
        {
          foreach (XmlNode childNode in node.ChildNodes)
          {
            if (childNode.Name == "WBSItem")
            {
              var item = new WBSItem(parent);
              parent.Children.Add(item);
              ReadWBSItems(childNode, item);
            }
          }
        }
      }
    }

    #endregion

    #region Export

    public void Export(Schedule schedule, string fileName)
    {
      var xml = new XmlDocument();
      xml.CreateXmlDeclaration("1.0", "iso-8859-1", "yes");
      // Add general schedule data
      var projectElement = xml.CreateElement("Project");
      xml.AppendChild(projectElement);
      WriteSchedule(schedule, xml, projectElement);
      xml.Save(fileName);
    }

    private static void WriteSchedule(Schedule schedule, XmlDocument xml, XmlElement projectElement)
    {
      var applicationName = xml.CreateAttribute("Application");
      applicationName.InnerText = "NAS";
      projectElement.Attributes.Append(applicationName);
      var applicationVersion = xml.CreateAttribute("Version");
      applicationVersion.InnerText = "1.0";
      projectElement.Attributes.Append(applicationVersion);
      var saveDate = xml.CreateAttribute("SaveDate");
      saveDate.InnerText = DateTime.Now.ToString("yyyy-MM-dd");
      projectElement.Attributes.Append(saveDate);
      projectElement.AppendTextChild("Name", schedule.Name);
      projectElement.AppendTextChild("Description", schedule.Description);
      projectElement.AppendTextChild("StartDate", schedule.StartDate);
      projectElement.AppendTextChild("EndDate", schedule.EndDate);
      projectElement.AppendTextChild("DataDate", schedule.DataDate);

      // Scheduling Settings
      var settingsElement = xml.CreateElement("SchedulingSettings");
      projectElement.AppendChild(settingsElement);
      settingsElement.AppendTextChild("CriticalPath", schedule.SchedulingSettings.CriticalPath);

      // Add Calendars
      var calendars = new List<Calendar>(schedule.Calendars);
      if (calendars.Count > 0)
      {
        var calendarsElement = xml.CreateElement("Calendars");
        projectElement.AppendChild(calendarsElement);
        for (int i = 0; i < calendars.Count; i++)
        {
          var calendarElement = xml.CreateElement("Calendar");
          calendarsElement.AppendChild(calendarElement);
          calendarElement.AppendTextChild("ID", i);
          calendarElement.AppendTextChild("Name", calendars[i].Name);
          calendarElement.AppendTextChild("Sunday", calendars[i].Sunday);
          calendarElement.AppendTextChild("Monday", calendars[i].Monday);
          calendarElement.AppendTextChild("Tuesday", calendars[i].Tuesday);
          calendarElement.AppendTextChild("Wednesday", calendars[i].Wednesday);
          calendarElement.AppendTextChild("Thursday", calendars[i].Thursday);
          calendarElement.AppendTextChild("Friday", calendars[i].Friday);
          calendarElement.AppendTextChild("Saturday", calendars[i].Saturday);
          if (calendars[i].Holidays != null && calendars[i].Holidays.Count > 0)
          {
            var holidayElement = xml.CreateElement("Holidays");
            calendarElement.AppendChild(holidayElement);
            foreach (var exception in calendars[i].Holidays)
            {
              holidayElement.AppendTextChild("Holiday", exception.Date);
            }
          }
        }
        if (schedule.StandardCalendar != null)
        {
          projectElement.AppendTextChild("StandardCalendar", calendars.IndexOf(schedule.StandardCalendar));
        }
      }

      // Add Fragnets
      var fragnets = new List<Fragnet>(schedule.Fragnets);
      if (fragnets.Count > 0)
      {
        var fragnetsElement = xml.CreateElement("Fragnets");
        projectElement.AppendChild(fragnetsElement);
        for (int i = 0; i < fragnets.Count; i++)
        {
          var fragnetElement = xml.CreateElement("Fragnet");
          fragnetsElement.AppendChild(fragnetElement);
          fragnetElement.AppendTextChild("ID", fragnets[i].Number);
          fragnetElement.AppendTextChild("Name", fragnets[i].Name);
          fragnetElement.AppendTextChild("Description", fragnets[i].Description);
          fragnetElement.AppendTextChild("IsVisible", fragnets[i].IsVisible);
          fragnetElement.AppendTextChild("Approved", fragnets[i].Approved);
          fragnetElement.AppendTextChild("Identified", fragnets[i].Identified);
          fragnetElement.AppendTextChild("IsDisputable", fragnets[i].IsDisputable);
          fragnetElement.AppendTextChild("Submitted", fragnets[i].Submitted);
        }
      }

      // Add WBS
      var wbsElement = xml.CreateElement("WBS");
      projectElement.AppendChild(wbsElement);
      if (schedule.WBSItem != null)
      {
        wbsElement.AppendTextChild("Number", schedule.WBSItem.Number);
        wbsElement.AppendTextChild("Name", schedule.WBSItem.Name);
        WriteWBSItems(wbsElement, schedule.WBSItem.Children);
      }

      // Add Custom Attributes
      var customAttributes1 = new List<CustomAttribute>(schedule.CustomAttributes1);
      if (customAttributes1.Count > 0)
      {
        var attributes1Element = xml.CreateElement("CustomAttributes1");
        projectElement.AppendChild(attributes1Element);
        for (int i = 0; i < customAttributes1.Count; i++)
        {
          var attributeElement = xml.CreateElement("CustomAttribute");
          attributes1Element.AppendChild(attributeElement);
          attributeElement.AppendTextChild("ID", i);
          attributeElement.AppendTextChild("Name", customAttributes1[i].Name);
        }
      }
      var customAttributes2 = new List<CustomAttribute>(schedule.CustomAttributes2);
      if (customAttributes2.Count > 0)
      {
        var attributes2Element = xml.CreateElement("CustomAttributes2");
        projectElement.AppendChild(attributes2Element);
        for (int i = 0; i < customAttributes2.Count; i++)
        {
          var attributeElement = xml.CreateElement("CustomAttribute");
          attributes2Element.AppendChild(attributeElement);
          attributeElement.AppendTextChild("ID", i);
          attributeElement.AppendTextChild("Name", customAttributes2[i].Name);
        }
      }
      var customAttributes3 = new List<CustomAttribute>(schedule.CustomAttributes3);
      if (customAttributes3.Count > 0)
      {
        var attributes3Element = xml.CreateElement("CustomAttributes3");
        projectElement.AppendChild(attributes3Element);
        for (int i = 0; i < customAttributes3.Count; i++)
        {
          var attributeElement = xml.CreateElement("CustomAttribute");
          attributes3Element.AppendChild(attributeElement);
          attributeElement.AppendTextChild("ID", i);
          attributeElement.AppendTextChild("Name", customAttributes3[i].Name);
        }
      }
      // Add Resources
      var resources = new List<Resource>(schedule.Resources);
      if (resources.Count > 0)
      {
        var resourcesElement = xml.CreateElement("Resources");
        projectElement.AppendChild(resourcesElement);
        for (int i = 0; i < resources.Count; i++)
        {
          XmlElement resourceElement = null;
          if (resources[i] is MaterialResource)
          {
            resourceElement = xml.CreateElement("MaterialResource");
            resourceElement.AppendTextChild("Unit", (resources[i] as MaterialResource).Unit);
          }
          else if (resources[i] is WorkResource)
          {
            resourceElement = xml.CreateElement("WorkResource");
          }
          else if (resources[i] is CalendarResource)
          {
            resourceElement = xml.CreateElement("CalendarResource");
          }
          if (resourceElement != null)
          {
            resourcesElement.AppendChild(resourceElement);
            resourceElement.AppendTextChild("Name", resources[i].Name);
            resourceElement.AppendTextChild("CostsPerUnit", resources[i].CostsPerUnit);
            resourceElement.AppendTextChild("Limit", resources[i].Limit);
          }
        }
      }
      // Add Activities
      var activities = new List<Activity>(schedule.Activities);
      var activitiesElement = xml.CreateElement("Activities");
      projectElement.AppendChild(activitiesElement);
      for (int i = 0; i < activities.Count; i++)
      {
        var a = activities[i];
        XmlElement activityElement;
        if (a.ActivityType == ActivityType.Milestone)
        {
          activityElement = xml.CreateElement("Milestone");
        }
        else
        {
          activityElement = xml.CreateElement("Activity");
          activityElement.AppendTextChild("OriginalDuration", a.OriginalDuration);
        }
        activitiesElement.AppendChild(activityElement);
        activitiesElement.AppendTextChild("ID", a.Number);
        activitiesElement.AppendTextChild("Name", a.Name);
        activitiesElement.AppendTextChild("EarlyStartDate", a.EarlyStartDate);
        activitiesElement.AppendTextChild("LateStartDate", a.LateStartDate);
        if (a.Fragnet != null)
        {
          activitiesElement.AppendTextChild("Fragnet", a.Fragnet.Number);
        }

        if (a.CustomAttribute1 != null)
        {
          activitiesElement.AppendTextChild("CustomAttribute1", customAttributes1.IndexOf(a.CustomAttribute1));
        }

        if (a.CustomAttribute1 != null)
        {
          activitiesElement.AppendTextChild("CustomAttribute2", customAttributes2.IndexOf(a.CustomAttribute2));
        }

        if (a.CustomAttribute1 != null)
        {
          activitiesElement.AppendTextChild("CustomAttribute3", customAttributes3.IndexOf(a.CustomAttribute3));
        }

        if (a.Calendar != null)
        {
          activitiesElement.AppendTextChild("Calendar", calendars.IndexOf(a.Calendar));
        }

        if (a.ActualStartDate.HasValue)
        {
          activitiesElement.AppendTextChild("ActualStartDate", a.ActualStartDate.Value);
        }

        if (a.ActualFinishDate.HasValue)
        {
          activitiesElement.AppendTextChild("ActualFinishDate", a.ActualFinishDate.Value);
        }

        activitiesElement.AppendTextChild("PercentComplete", a.PercentComplete);
        activitiesElement.AppendTextChild("TotalFloat", a.TotalFloat);
        activitiesElement.AppendTextChild("FreeFloat", a.FreeFloat);
        if (a.WBSItem != null)
        {
          activitiesElement.AppendTextChild("WBS", a.WBSItem);
        }
        // Add Constraints
        if (a.Constraint != ConstraintType.None)
        {
          var constraintsElement = xml.CreateElement("Constraints");
          activityElement.AppendChild(constraintsElement);
          var constraintElement = xml.CreateElement("Constraint");
          constraintsElement.AppendChild(constraintElement);
          constraintElement.AppendTextChild("ConstraintType", a.Constraint);
          constraintElement.AppendTextChild("ConstraintDate", a.ConstraintDate);
        }
        // Add Resources
        var resourceAssignments = new List<ResourceAssignment>(a.ResourceAssignments);
        if (resourceAssignments.Count > 0)
        {
          var resourcesElement = xml.CreateElement("Resources");
          activityElement.AppendChild(resourcesElement);
          for (int j = 0; j < resourceAssignments.Count; j++)
          {
            var resourceElement = xml.CreateElement("Resource");
            resourcesElement.AppendChild(resourceElement);
            resourceElement.AppendTextChild("ResourceID", resources.IndexOf(resourceAssignments[j].Resource));
            resourceElement.AppendTextChild("Budget", resourceAssignments[j].Budget);
            resourceElement.AppendTextChild("FixedCosts", resourceAssignments[j].FixedCosts);
            resourceElement.AppendTextChild("UnitsPerDay", resourceAssignments[j].UnitsPerDay);
          }
        }
        // Add Distortions
        var distortions = new List<Distortion>(a.Distortions);
        if (distortions.Count > 0)
        {
          var distortionsElement = xml.CreateElement("Distortions");
          activityElement.AppendChild(distortionsElement);
          foreach (var dis in distortions)
          {
            XmlElement distortionElement = null;
            if (dis is Delay)
            {
              distortionElement = xml.CreateElement("Delay");
              distortionElement.AppendTextChild("Days", (dis as Delay).Days);
            }
            else if (dis is Interruption)
            {
              distortionElement = xml.CreateElement("Interruption");
              distortionElement.AppendTextChild("Days", (dis as Interruption).Days);
              distortionElement.AppendTextChild("Start", (dis as Interruption).Start);
            }
            else if (dis is Inhibition)
            {
              distortionElement = xml.CreateElement("Inhibition");
              distortionElement.AppendTextChild("Percent", (dis as Inhibition).Percent);
            }
            else if (dis is Extension)
            {
              distortionElement = xml.CreateElement("Extension");
              distortionElement.AppendTextChild("Days", (dis as Extension).Days);
            }
            else if (dis is Reduction)
            {
              distortionElement = xml.CreateElement("Reduction");
              distortionElement.AppendTextChild("Days", (dis as Reduction).Days);
            }
            distortionElement.AppendTextChild("Description", dis.Description);
            if (dis.Fragnet != null)
            {
              distortionElement.AppendTextChild("Fragnet", fragnets.IndexOf(dis.Fragnet));
            }

            distortionsElement.AppendChild(distortionElement);
          }
        }
      }
      // Add relationships
      var relationships = new List<Relationship>(schedule.Relationships);
      var relationshipsElement = xml.CreateElement("Relationships");
      projectElement.AppendChild(relationshipsElement);
      for (int i = 0; i < relationships.Count; i++)
      {
        var relationshipElement = xml.CreateElement("Relationship");
        relationshipsElement.AppendChild(relationshipElement);
        relationshipElement.AppendTextChild("Activity1", relationships[i].Activity1.Number);
        relationshipElement.AppendTextChild("Activity2", relationships[i].Activity2.Number);
        relationshipElement.AppendTextChild("DependencyType", relationships[i].RelationshipType);
        relationshipElement.AppendTextChild("Lag", relationships[i].Lag);
      }
      // Add BaseLines
      var baselines = new List<Schedule>();
      if (schedule.Baselines.Count > 0)
      {
        var baseLinesElement = xml.CreateElement("Baselines");
        projectElement.AppendChild(baseLinesElement);
        foreach (var baseline in schedule.Baselines)
        {
          var baseLineElement = xml.CreateElement("Baseline");
          baseLinesElement.AppendChild(baseLineElement);
          WriteSchedule(baseline, xml, baseLineElement);
          baselines.Add(baseline);
        }
      }
      // Add PERT Definitions
      var perts = new List<PERTDefinition>(schedule.PERTDefinitions);
      if (perts.Count > 0)
      {
        var pertsElement = xml.CreateElement("PERTDefinitions");
        projectElement.AppendChild(pertsElement);
        for (int i = 0; i < perts.Count; i++)
        {
          if (perts[i] != null)
          {
            var pertElement = xml.CreateElement("PERTDefinition");
            pertsElement.AppendChild(pertElement);
            pertElement.AppendTextChild("Name", perts[i].Name);
            pertElement.AppendTextChild("Width", perts[i].Width);
            pertElement.AppendTextChild("Height", perts[i].Height);
            pertElement.AppendTextChild("FontSize", perts[i].FontSize);
            pertElement.AppendTextChild("SpacingX", perts[i].SpacingX);
            pertElement.AppendTextChild("SpacingY", perts[i].SpacingY);
            var rows = new List<RowDefinition>(perts[i].RowDefinitions.OrderBy(x => x.Sort));
            if (rows.Count > 0)
            {
              var rowsElement = xml.CreateElement("Rows");
              pertElement.AppendChild(rowsElement);
              for (int j = 0; j < rows.Count; j++)
              {
                var rowElement = xml.CreateElement("Row");
                rowsElement.AppendChild(rowElement);
                rowElement.AppendTextChild("Sort", j);
                rowElement.AppendTextChild("Height", rows[j].Height);
              }
            }
            var cols = new List<ColumnDefinition>(perts[i].ColumnDefinitions.OrderBy(x => x.Sort));
            if (cols.Count > 0)
            {
              var colsElement = xml.CreateElement("Columns");
              pertElement.AppendChild(colsElement);
              for (int j = 0; j < cols.Count; j++)
              {
                var colElement = xml.CreateElement("Column");
                colsElement.AppendChild(colElement);
                colElement.AppendTextChild("Sort", j);
                colElement.AppendTextChild("Width", cols[j].Width);
              }
            }
            var items = new List<PERTDataItem>(perts[i].Items);
            if (items.Count > 0)
            {
              var itemsElement = xml.CreateElement("Items");
              pertElement.AppendChild(itemsElement);
              for (int j = 0; j < items.Count; j++)
              {
                var itemElement = xml.CreateElement("Item");
                itemsElement.AppendChild(itemElement);
                itemElement.AppendTextChild("Row", items[j].Row);
                itemElement.AppendTextChild("Column", items[j].Column);
                itemElement.AppendTextChild("RowSpan", items[j].RowSpan);
                itemElement.AppendTextChild("ColumnSpan", items[j].ColumnSpan);
                itemElement.AppendTextChild("HorizontalAlignment", items[j].HorizontalAlignment);
                itemElement.AppendTextChild("VerticalAlignment", items[j].VerticalAlignment);
                itemElement.AppendTextChild("Property", items[j].Property);
              }
            }
          }
        }
      }
      // Add layouts
      var layouts = new List<Layout>(schedule.Layouts);
      if (layouts.Count > 0)
      {
        var layoutsElement = xml.CreateElement("Layouts");
        projectElement.AppendChild(layoutsElement);
        for (int i = 0; i < layouts.Count; i++)
        {
          if (layouts[i] != null)
          {
            var layout = layouts[i];
            var ganttLayout = layout as GanttLayout;
            var pertLayout = layout as PERTLayout;
            var layoutElement = xml.CreateElement("Layout");
            layoutsElement.AppendChild(layoutElement);
            layoutElement.AppendTextChild("ID", i);
            layoutElement.AppendTextChild("Name", layout.Name);
            layoutElement.AppendTextChild("ActivityStandardColor", layout.ActivityStandardColor);
            layoutElement.AppendTextChild("ActivityCriticalColor", layout.ActivityCriticalColor);
            layoutElement.AppendTextChild("ActivityDoneColor", layout.ActivityDoneColor);
            layoutElement.AppendTextChild("MilestoneStandardColor", layout.MilestoneStandardColor);
            layoutElement.AppendTextChild("MilestoneCriticalColor", layout.MilestoneCriticalColor);
            layoutElement.AppendTextChild("MilestoneDoneColor", layout.MilestoneDoneColor);
            layoutElement.AppendTextChild("DataDateColor", layout.DataDateColor);
            if (layout.VisibleBaselines.Count > 0)
            {
              layoutElement.AppendTextChild("BaselineColor", layout.VisibleBaselines.First().Color);
              layoutElement.AppendTextChild("ShowBaseline", true);
            }
            layoutElement.AppendTextChild("ShowRelationships", layout.ShowRelationships);
            layoutElement.AppendTextChild("ShowFloat", layout.ShowFloat);
            var visibleColumnsElement = xml.CreateElement("VisibleColumns");
            layoutElement.AppendChild(visibleColumnsElement);
            foreach (var activityColumn in layout.ActivityColumns)
            {
              var activityColumnElement = xml.CreateElement("ActivityColumn");
              visibleColumnsElement.AppendChild(activityColumnElement);
              activityColumnElement.AppendTextChild("Property", activityColumn.Property);
              if (activityColumn.ColumnWidth.HasValue)
              {
                activityColumnElement.AppendTextChild("ColumnWidth", activityColumn.ColumnWidth);
              }
            }
            var sortingDefinitionsElement = xml.CreateElement("SortingDefinitions");
            layoutElement.AppendChild(sortingDefinitionsElement);
            foreach (var sortingDefinition in layout.SortingDefinitions)
            {
              var sortingDefinitionElement = xml.CreateElement("SortingDefinition");
              sortingDefinitionsElement.AppendChild(sortingDefinitionElement);
              sortingDefinitionElement.AppendTextChild("Property", sortingDefinition.Property);
              sortingDefinitionElement.AppendTextChild("Direction", sortingDefinition.Direction);
            }
            var groupingDefinitionsElement = xml.CreateElement("GroupingDefinitions");
            layoutElement.AppendChild(groupingDefinitionsElement);
            foreach (var groupingDefinition in layout.GroupingDefinitions)
            {
              var groupingDefinitionElement = xml.CreateElement("GroupingDefinition");
              groupingDefinitionsElement.AppendChild(groupingDefinitionElement);
              groupingDefinitionElement.AppendTextChild("Property", groupingDefinition.Property);
              groupingDefinitionElement.AppendTextChild("Color", groupingDefinition.Color);
            }
            if (ganttLayout != null)
            {
              layoutElement.AppendTextChild("LeftText", ganttLayout.LeftText);
              layoutElement.AppendTextChild("CenterText", ganttLayout.CenterText);
              layoutElement.AppendTextChild("RightText", ganttLayout.RightText);
            }
            layoutElement.AppendTextChild("FilterCombination", layout.FilterCombination);
            layoutElement.AppendTextChild("IsPert", layout.LayoutType == LayoutType.PERT);
            if (pertLayout != null)
            {
              layoutElement.AppendTextChild("PERTDefinition", perts.IndexOf(pertLayout.PERTDefinition));
            }
            var filtersElement = xml.CreateElement("Filters");
            layoutElement.AppendChild(filtersElement);
            foreach (var filter in layout.FilterDefinitions)
            {
              var filterElement = xml.CreateElement("FilterDefinition");
              filtersElement.AppendChild(filterElement);
              filterElement.AppendTextChild("Property", filter.Property);
              filterElement.AppendTextChild("Relation", filter.Relation);
              filterElement.AppendTextChild("Object", filter.ObjectString);
            }
            var resourcesElement = xml.CreateElement("VisibleResources");
            layoutElement.AppendChild(resourcesElement);
            foreach (var resource in layout.VisibleResources)
            {
              var resourceElement = xml.CreateElement("VisibleResource");
              resourcesElement.AppendChild(resourceElement);
              resourceElement.AppendTextChild("ID", resources.IndexOf(resource.Resource));
              resourceElement.AppendTextChild("ShowBudget", resource.ShowBudget);
              resourceElement.AppendTextChild("ShowActualCosts", resource.ShowActualCosts);
              resourceElement.AppendTextChild("ShowPlannedCosts", resource.ShowPlannedCosts);
              resourceElement.AppendTextChild("ShowResourceAllocation", resource.ShowResourceAllocation);
            }
            var baselinesElement = xml.CreateElement("VisibleBaselines");
            layoutElement.AppendChild(baselinesElement);
            foreach (var baseline in layout.VisibleBaselines)
            {
              var baselineElement = xml.CreateElement("VisibleBaseline");
              resourcesElement.AppendChild(baselineElement);
              baselineElement.AppendTextChild("ID", baselines.IndexOf(baseline.Schedule));
              baselineElement.AppendTextChild("Color", baseline.Color);
            }
          }
        }
      }
      // Add Current Layout
      if (schedule.ActiveLayout != null)
      {
        projectElement.AppendTextChild("ActiveLayout", layouts.IndexOf(schedule.ActiveLayout));
      }
    }

    private static void WriteWBSItems(XmlElement parentElement, IEnumerable<WBSItem> items)
    {
      if (items != null && items.Any())
      {
        var xml = parentElement.OwnerDocument;
        var itemsElement = xml.CreateElement("Items");
        parentElement.AppendChild(itemsElement);
        foreach (var item in items)
        {
          var itemElement = xml.CreateElement("WBSItem");
          itemsElement.AppendChild(itemElement);
          itemElement.AppendTextChild("Number", item.Number);
          itemElement.AppendTextChild("Name", item.Name);
          WriteWBSItems(itemElement, item.Children);
        }
      }
    }

    #endregion

    #region Private Members

    private static WBSItem FindWBSItem(WBSItem parent, string number)
    {
      if (parent.Number == number)
      {
        return parent;
      }

      foreach (var item in parent.Children)
      {
        if (FindWBSItem(item, number) != null)
        {
          return item;
        }
      }
      return null;
    }

    #endregion
  }
}
