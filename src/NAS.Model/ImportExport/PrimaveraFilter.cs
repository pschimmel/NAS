using System.Diagnostics;
using System.IO;
using NAS.Model.Entities;
using NAS.Model.Enums;
using NAS.Resources;

namespace NAS.Model.ImportExport
{
  internal class PrimaveraFilter : IImportFilter
  {
    //private ScheduleController controller;
    private Schedule schedule;
    private readonly Dictionary<int, Calendar> calendars = [];
    private readonly Dictionary<int, Entities.Activity> activities = [];

    public string FilterName => NASResources.PrimaveraFiles;

    public string FileExtension => ".xer";

    public string Output { get; private set; } = string.Empty;

    public Schedule Import(/*ScheduleController controller, */string fileName)
    {
      ClearOutput();
      schedule = new Schedule(); //controller.AddEmptySchedule();
      if (!File.Exists(fileName))
      {
        throw new FileNotFoundException("File " + fileName + " not found!");
      }

      if (!fileName.ToLower().EndsWith(FileExtension.ToLower()))
      {
        throw new Exception("Wrong file extension!");
      }

      //this.controller = controller;
      string[] lines = File.ReadAllLines(fileName);
      var projectFragment = GetFragment(lines, SectionType.PROJECT);
      var wbsFragment = GetFragment(lines, SectionType.PROJWBS);
      var calendarFragment = GetFragment(lines, SectionType.CALENDAR);
      var activityFragment = GetFragment(lines, SectionType.TASK);
      var relationshipFragment = GetFragment(lines, SectionType.TASKPRED);
      ApplyProjectValues(projectFragment);
      ApplyWbsValues(wbsFragment);
      ApplyCalendarValues(calendarFragment);
      ApplyActivityValues(activityFragment);
      ApplyRelationshipValues(relationshipFragment);
      return schedule;
    }

    private void ApplyCalendarValues(Fragment calendarFragment)
    {
      for (int i = 0; i < calendarFragment.Count; i++)
      {
        int? id = calendarFragment.GetInt("clndr_id", i);
        if (id.HasValue)
        {
          var c = new Calendar();// controller.AddCalendar(schedule);
          c.Name = calendarFragment.GetItem("clndr_name", i);
          bool? standard = calendarFragment.GetBoolean("default_flag", i);
          if (standard == true)
          {
            c.IsStandard = true;
          }

          string data = calendarFragment.GetItem("clndr_data", i);
          ApplyCalendarData(c, data);
          calendars.Add(id.Value, c);
        }
      }
    }

    private void ApplyCalendarData(Calendar c, string data)
    {
      string s = GetNextItem(data);
      if (string.IsNullOrEmpty(s))
      {
        return;
      }

      if (s.StartsWith("0||CalendarData()"))
      {
        s = s.Remove(0, "0||CalendarData()".Length);
        ApplyCalendarData(c, s);
      }
      else if (s.StartsWith("0||DaysOfWeek()"))
      {
        s = s.Remove(0, "0||DaysOfWeek()".Length);
        ApplyDaysOfWeek(c, s);
      }
      else if (s.StartsWith("0||VIEW"))
      {
        s = s.Remove(0, "0||VIEW".Length);
        string s1 = GetNextItem(s);
        s = s.Remove(0, s1.Length);
        ApplyCalendarData(c, s);
      }
      else
      {
        ApplyCalendarData(c, s);
      }
    }

    private void ApplyDaysOfWeek(Calendar c, string data)
    {
      string s = GetNextItem(data);
      if (string.IsNullOrEmpty(s))
      {
        return;
      }

      for (int i = 1; i <= 7; i++)
      {
        if (s.StartsWith("0||" + i + "()"))
        {
          s = s.Remove(0, ("0||" + i + "()").Length);
          string s1 = GetNextItem(s);
          bool workday = !string.IsNullOrEmpty(s1);
          switch (i)
          {
            case 1: c.Sunday = workday; break;
            case 2: c.Monday = workday; break;
            case 3: c.Tuesday = workday; break;
            case 4: c.Wednesday = workday; break;
            case 5: c.Thursday = workday; break;
            case 6: c.Friday = workday; break;
            case 7: c.Saturday = workday; break;
          }
          ApplyDaysOfWeek(c, s1);
        }
        else
        {
          ApplyDaysOfWeek(c, s);
        }
      }
    }

    private static string GetNextItem(string s)
    {
      s = s.Trim();
      while (s.StartsWith(((char)127).ToString()))
      {
        s = s.Remove(0, 1);
        s = s.Trim();
      }

      if (s.StartsWith("("))
      {
        int brackets = 1;
        for (int i = 1; i < s.Length; i++)
        {
          if (s[i] == '(')
          {
            brackets++;
          }
          else if (s[i] == ')')
          {
            brackets--;
          }

          if (brackets == 0)
          {
            s = s.Substring(1, i - 1);
            while (s.StartsWith(((char)127).ToString()))
            {
              s = s.Remove(0, 1);
              s = s.Trim();
            }
            return s;
          }
        }
      }
      return "";
    }

    private void ApplyWbsValues(Fragment wbsFragment)
    {
      if (wbsFragment.Count == 0)
      {
        return;
      }

      schedule.Description = wbsFragment.GetItem("wbs_name", 0);
    }

    private void ApplyProjectValues(Fragment projectFragment)
    {
      if (projectFragment.Count == 0)
      {
        return;
      }

      schedule.Name = projectFragment.GetItem("proj_short_name", 0);
      var date = projectFragment.GetDateTime("plan_start_date", 0);
      if (date.HasValue)
      {
        schedule.StartDate = date.Value;
      }

      schedule.EndDate = projectFragment.GetDateTime("plan_end_date", 0);
      var date2 = projectFragment.GetDateTime("sum_data_date", 0);
      schedule.DataDate = date2 ?? schedule.StartDate;
    }

    private void ApplyActivityValues(Fragment taskFragment)
    {
      for (int i = 0; i < taskFragment.Count; i++)
      {
        try
        {
          int? id = taskFragment.GetInt("task_id", i);
          if (id.HasValue)
          {
            Model.Entities.Activity a = null;
            string type = taskFragment.GetItem("task_type", i);
            if (type == "TT_Mile" || type == "TT_FinMile")
            {
              a = Model.Entities.Activity.NewMilestone();
            }
            else if (type == "TT_Task")
            {
              a = Model.Entities.Activity.NewActivity();
            }
            else
            {
              continue;
            }

            a.Name = taskFragment.GetItem("task_name", i);
            a.Number = taskFragment.GetItem("task_code", i);
            int? calendarID = taskFragment.GetInt("clndr_id", i);
            if (calendarID.HasValue)
            {
              if (calendars.ContainsKey(calendarID.Value))
              {
                a.Calendar = calendars[calendarID.Value];
              }
              else
              {
                AddOutput($"Error while importing task {a.Name} ({a.Number}): Calendar ID {calendarID.Value} not found. Using default instead.");
              }
            }

            var date = taskFragment.GetDateTime("early_start_date", i);
            if (date.HasValue)
            {
              a.EarlyStartDate = date.Value;
            }

            var date2 = taskFragment.GetDateTime("early_end_date", i);
            if (date2.HasValue)
            {
              a.EarlyFinishDate = date2.Value;
            }

            var date3 = taskFragment.GetDateTime("late_start_date", i);
            if (date3.HasValue)
            {
              a.LateStartDate = date3.Value;
            }

            var date4 = taskFragment.GetDateTime("late_end_date", i);
            if (date4.HasValue)
            {
              a.LateStartDate = date4.Value;
            }

            a.ActualStartDate = taskFragment.GetDateTime("act_start_date", i);
            a.ActualStartDate = taskFragment.GetDateTime("act_end_date", i);
            a.ConstraintDate = taskFragment.GetDateTime("cstr_date", i);
            int? dur = taskFragment.GetInt("target_drtn_hr_cnt", i);
            if (dur.HasValue)
            {
              a.OriginalDuration = dur.Value / 8;
            }

            int? rem = taskFragment.GetInt("remain_drtn_hr_cnt", i);
            if (rem.HasValue)
            {
              a.RemainingDuration = rem.Value / 8;
            }

            activities.Add(id.Value, a);
          }
        }
        catch (Exception ex)
        {
          Debug.Fail($"Exception in line {i}: {ex.Message} of task fragment.");
          throw;
        }
      }
    }

    private void ApplyRelationshipValues(Fragment relationshipFragment)
    {
      for (int i = 0; i < relationshipFragment.Count; i++)
      {
        try
        {
          int? id1 = relationshipFragment.GetInt("pred_task_id", i);
          int? id2 = relationshipFragment.GetInt("task_id", i);
          if (id1.HasValue && id2.HasValue)
          {
            if (!activities.ContainsKey(id1.Value))
            {
              AddOutput($"Activity with ID {id1.Value} was not found in the list while adding a relationship to {id2.Value}. Relationship will be ignored.");
              continue;
            }
            else if (!activities.ContainsKey(id2.Value))
            {
              AddOutput($"Activity with ID {id2.Value} was not found in the list while adding a relationship to {id1.Value}. Relationship will be ignored.");
              continue;
            }

            var r = new Relationship(); //controller.AddRelationship(activities[id1.Value], activities[id2.Value]);
            string type = relationshipFragment.GetItem("pred_type", i);
            if (type == "PR_SS")
            {
              r.RelationshipType = RelationshipType.StartStart;
            }
            else if (type == "PR_FS")
            {
              r.RelationshipType = RelationshipType.FinishStart;
            }
            else if (type == "PR_FF")
            {
              r.RelationshipType = RelationshipType.FinishFinish;
            }

            int? lag = relationshipFragment.GetInt("lag_hr_cnt", i);
            if (lag.HasValue)
            {
              r.Lag = lag.Value / 8;
            }

            //var scheduler = new Scheduler(schedule);
            //if (!scheduler.CheckForLoops())
            //{
            //  controller.DeleteRelationship(r);
            //}
          }
        }
        catch (Exception ex)
        {
          Debug.Fail($"Exception in line {i}: {ex.Message} of relationship fragment.");
          throw;
        }
      }
    }

    private static Fragment GetFragment(string[] lines, SectionType type)
    {
      Fragment fragment = null;
      for (int i = 0; i < lines.Length; i++)
      {
        string[] line = lines[i].Split(new char[] { (char)9 });
        if (fragment == null)
        {
          if (line.Length >= 2 && line[0] == "%T" && line[1] == type.ToString())
          {
            string[] line2 = lines[i + 1].Split(new char[] { (char)9 });
            if (line2.Length > 0 && line2[0] == "%F")
            {
              fragment = new Fragment(line2.Skip(1).ToArray(), type);
              i++;
            }
          }
        }
        else
        {
          string[] lineX = lines[i].Split(new char[] { (char)9 });
          if (lineX.Length > 0 && (lineX[0] == "%E" || lineX[0] == "%T"))
          {
            return fragment;
          }
          else if (lineX.Length > 0 && lineX[0] == "%R")
          {
            fragment.AddLine(lineX.Skip(1).ToArray());
          }
        }
      }
      return fragment;
    }

    private enum SectionType
    {
      PROJECT, CALENDAR, TASK, TASKPRED, PROJWBS
    }

    private void ClearOutput()
    {
      Output = string.Empty;
    }

    private void AddOutput(string message)
    {
      if (!string.IsNullOrEmpty(Output))
      {
        Output += Environment.NewLine;
      }

      Output += message;
    }

    private class Fragment
    {
      private readonly List<List<string>> fragment = [];

      public Fragment(string[] headers, SectionType name)
      {
        Headers = new List<string>(headers);
        Name = name;
      }

      public SectionType Name { get; private set; }
      public List<string> Headers { get; private set; }

      public void AddLine(string[] line)
      {
        if (Headers.Count != line.Length)
        {
          throw new ApplicationException();
        }

        fragment.Add(new List<string>(line));
      }

      public string GetItem(string header, int idx)
      {
        int i = Headers.IndexOf(header);
        return fragment[idx][i];
      }

      public DateTime? GetDateTime(string header, int idx)
      {
        return DateTime.TryParse(GetItem(header, idx), out var d) ? d : null;
      }

      public bool? GetBoolean(string header, int idx)
      {
        string b = GetItem(header, idx);
        switch (b)
        {
          case "Y":
            return true;
          case "N":
            return false;
          default:
            return null;
        }
      }

      public int? GetInt(string header, int idx)
      {
        return int.TryParse(GetItem(header, idx), out int d) ? d : null;
      }

      public int Count => fragment.Count;
    }
  }
}
