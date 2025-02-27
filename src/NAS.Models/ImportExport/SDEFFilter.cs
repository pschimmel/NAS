﻿using System.IO;
using System.Text;
using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.Resources;

namespace NAS.Models.ImportExport
{
  internal class SDEFFilter : IExportFilter
  {
    private Schedule schedule;
    private readonly Dictionary<Calendar, int> calendarIDs = [];
    private readonly Dictionary<Activity, int> activityIDs = [];

    public string FilterName => NASResources.SDEFFiles;

    public string FileExtension => ".txt";

    public string Output => string.Empty;

    public void Export(Schedule project, string fileName)
    {
      schedule = project;
      for (int i = 0; i < project.Calendars.Count; i++)
      {
        calendarIDs.Add(project.Calendars.ToList()[i], i + 1);
      }

      for (int i = 0; i < project.Activities.Count; i++)
      {
        activityIDs.Add(project.Activities.ToList()[i], i + 1);
      }
      // Open filestream
      using var sw = new StreamWriter(fileName);
      WriteStartToken(sw);
      WriteProjectData(sw);
      WriteCalendars(sw);
      WriteActivityData(sw);
      WritePredecessors(sw);
      WriteUnitCosts(sw);
      WriteProgress(sw);
      WriteEndToken(sw);
    }

    /// <summary>
    /// Writes _start token to filestream
    /// </summary>
    /// <param _name="writer">filestream</param>
    private static void WriteStartToken(StreamWriter sw)
    {
      sw.WriteLine("VOLM  1");
    }

    /// <summary>
    /// Writes project data to filestream
    /// </summary>
    /// <param _name="writer">filestream</param>
    private void WriteProjectData(StreamWriter sw)
    {
      if (schedule != null)
      {
        // PROJ 21OCT04 VILS Barracks Complex Phase I, Vilseck                Bilfinger Berger AG                  P Initia 22JUN04 23DEC05
        var sb = new StringBuilder("PROJ ");
        sb.Append(DateFormatter(schedule.DataDate)); //project.DataDate));
        sb.Append(' ');
        sb.Append(TextFormatter(schedule.Name, 4, TextAlignment.Left));
        sb.Append(' ');
        sb.Append(TextFormatter(schedule.Name, 48, TextAlignment.Left));
        sb.Append(' ');
        sb.Append(TextFormatter("" /*project.Contractor*/, 36, TextAlignment.Left));
        sb.Append(' ');
        //switch (project.Logic) {
        //  case Project.LogicType.Arrow:
        //    sb.Append("A");
        //    break;
        //  case Project.LogicType.Precedence:
        sb.Append('P');
        //    break;
        //}
        sb.Append(' ');
        sb.Append(TextFormatter(schedule.ID.ToString(), 6, TextAlignment.Left));
        sb.Append(' ');
        sb.Append(DateFormatter(schedule.StartDate));
        sb.Append(' ');
        sb.Append(DateFormatter(schedule.LastDay));
        sw.WriteLine(sb.ToString());
      }
    }

    /// <summary>
    /// Writes calendars to filestream
    /// </summary>
    /// <param _name="writer">filestream</param>
    private void WriteCalendars(StreamWriter sw)
    {
      if (schedule != null)
      {
        // CLDR 1 NYYYYYN 5 _days
        foreach (var cal in schedule.Calendars)
        {
          var sb = new StringBuilder("CLDR ");
          sb.Append(calendarIDs[cal]);
          sb.Append(' ');
          string workdays = "";
          int workdayCount = 0;
          for (int i = 0; i < 7; i++)
          {
            switch ((DayOfWeek)i)
            {
              case DayOfWeek.Monday:
                workdays += cal.Monday ? "Y" : "N";
                if (cal.Monday)
                {
                  workdayCount++;
                }

                break;
              case DayOfWeek.Tuesday:
                workdays += cal.Tuesday ? "Y" : "N";
                if (cal.Tuesday)
                {
                  workdayCount++;
                }

                break;
              case DayOfWeek.Wednesday:
                workdays += cal.Wednesday ? "Y" : "N";
                if (cal.Wednesday)
                {
                  workdayCount++;
                }

                break;
              case DayOfWeek.Thursday:
                workdays += cal.Thursday ? "Y" : "N";
                if (cal.Thursday)
                {
                  workdayCount++;
                }

                break;
              case DayOfWeek.Friday:
                workdays += cal.Friday ? "Y" : "N";
                if (cal.Friday)
                {
                  workdayCount++;
                }

                break;
              case DayOfWeek.Saturday:
                workdays += cal.Saturday ? "Y" : "N";
                if (cal.Saturday)
                {
                  workdayCount++;
                }

                break;
              case DayOfWeek.Sunday:
                workdays += cal.Sunday ? "Y" : "N";
                if (cal.Sunday)
                {
                  workdayCount++;
                }

                break;
            }
          }
          sb.Append(workdays);
          sb.Append(' ');
          sb.Append(workdayCount);
          sb.Append(" days");
          sw.WriteLine(sb.ToString());
        }
        // HOLI 1 06JAN   01NOV   11NOV   10JUN04 05JUL04 06SEP04 11OCT04 25NOV04 24DEC04 25DEC04 26DEC04 27DEC04 28DEC04 29DEC04 30DEC04
        foreach (var cal in schedule.Calendars)
        {
          if (cal.Holidays.Count > 0)
          {
            var sb = new StringBuilder("HOLI ");
            sb.Append(calendarIDs[cal]);
            sb.Append(' ');
            // Add repeating holidays
            int count = 0;
            // Add other holidays
            foreach (var holiday in cal.Holidays)
            {
              if (count > 15)
              {
                sw.WriteLine(sb.ToString());
                sb = new StringBuilder("HOLI ");
                sb.Append(calendarIDs[cal]);
                sb.Append(' ');
                count = 0;
              }
              if (count > 0)
              {
                sb.Append(' ');
              }

              sb.Append(DateFormatter(holiday.Date));
              count++;
            }
            sw.WriteLine(sb.ToString());
          }
        }
      }
    }

    /// <summary>
    /// Writes activities to filestream
    /// </summary>
    /// <param _name="writer">Filestream</param>
    private void WriteActivityData(StreamWriter sw)
    {
      if (schedule != null)
      {
        // ACTV 1.01.01-E  Field Office                   355            1     0 PRIM EXT         0001        SITE MOBILIZE: FIELD OFFICE
        foreach (var act in schedule.Activities)
        {
          var sb = new StringBuilder("ACTV ");
          sb.Append(TextFormatter(activityIDs[act].ToString(), 10, TextAlignment.Left));
          sb.Append(' ');
          sb.Append(TextFormatter(act.Name, 30, TextAlignment.Left));
          sb.Append(' ');
          sb.Append(TextFormatter(act.OriginalDuration.ToString(), 3, TextAlignment.Right));
          sb.Append(' ');
          if (act.Constraint == ConstraintType.None)
          {
            sb.Append("          ");
          }
          else
          {
            sb.Append(DateFormatter(act.ConstraintDate));
            sb.Append(' ');
            _ = act.Constraint switch
            {
              // EarlyStart
              ConstraintType.StartOnOrLater => sb.Append("ES"),
              // ConstraintType.EarlyFinish => sb.Append("EF");
              // ConstraintType.LateStart => sb.Append("LS");
              // LateFinish
              ConstraintType.EndOnOrEarlier => sb.Append("LF"),
              // ConstraintType.ExpectedFinish => sb.Append("XF");
              // MandatoryStart
              ConstraintType.StartOn => sb.Append("MS"),
              // MandatoryFinish
              ConstraintType.EndOn => sb.Append("MF"),
              // ConstraintType.ZeroFreeFloat => sb.Append("ZF");
              // ConstraintType.ZeroTotalFloat => sb.Append("ZT");
              _ => sb.Append("  "),
            };
          }
          sb.Append(' ');
          _ = act.Calendar == null ? sb.Append(' ') : sb.Append(calendarIDs[act.Calendar]);

          sb.Append(' ');
          //if (activity.HammockCode)
          //  sb.Append("Y");
          //else
          sb.Append(' ');
          sb.Append(' ');
          sb.Append(TextFormatter("" /*activity.WorkersPerDay.ToString()*/, 3, TextAlignment.Right));
          sb.Append(' ');
          sb.Append(TextFormatter("" /*activity.Responsibility*/, 4, TextAlignment.Left));
          sb.Append(' ');
          sb.Append(TextFormatter("" /*activity.Area*/, 4, TextAlignment.Left));
          sb.Append(' ');
          sb.Append(TextFormatter("" /*activity.Modification*/, 6, TextAlignment.Left));
          sb.Append(' ');
          sb.Append(TextFormatter("" /*activity.BidItem*/, 6, TextAlignment.Left));
          sb.Append(' ');
          sb.Append(TextFormatter("" /*activity.Phase*/, 2, TextAlignment.Left));
          sb.Append(' ');
          sb.Append(TextFormatter("" /*activity.CategoryOfWork*/, 1, TextAlignment.Left));
          sb.Append(' ');
          sb.Append(TextFormatter("" /*activity.FeatureOfWork*/, 30, TextAlignment.Left));
          sw.WriteLine(sb.ToString());
        }
      }
    }

    /// <summary>
    /// Writes predecessors to filestream
    /// </summary>
    /// <param _name="writer">Filestream</param>
    private void WritePredecessors(StreamWriter sw)
    {
      if (schedule != null)
      {
        // PRED 2.32.01-A  1.01.03-E  F   10
        foreach (var activity in schedule.Activities)
        {
          foreach (var relationship in schedule.GetPreceedingRelationships(activity))
          {
            var sb = new StringBuilder("PRED ");
            sb.Append(TextFormatter(activityIDs[activity].ToString(), 10, TextAlignment.Left));
            sb.Append(' ');
            sb.Append(TextFormatter(activityIDs[schedule.GetActivity(relationship.Activity1.ID)].ToString(), 10, TextAlignment.Left));
            sb.Append(' ');
            switch (relationship.RelationshipType)
            {
              // Check this
              case RelationshipType.StartStart:
                sb.Append("S ");
                break;
              //case RelationshipType.StartFinish:
              //  sb.Append("SF");
              //  break;
              case RelationshipType.FinishStart:
                sb.Append("C ");
                break;
              case RelationshipType.FinishFinish:
                sb.Append("F ");
                break;
            }
            sb.Append(' ');
            sb.Append(TextFormatter(relationship.Lag.ToString(), 3, TextAlignment.Right));
            sw.WriteLine(sb.ToString());
          }
        }
      }
    }

    /// <summary>
    /// Writes _unit cost information to filestream
    /// </summary>
    /// <param _name="writer">Filestream</param>
    private void WriteUnitCosts(StreamWriter writer)
    {
      if (schedule != null)
      {
        // UNIT       1030       15.0000       22.2000         .0000
        foreach (var activity in schedule.Activities)
        {
          var assignments = schedule.ResourceAssignments?.Where(x => x.Activity == activity) ?? Enumerable.Empty<ResourceAssignment>();
          foreach (var assignment in assignments)
          {
            var sb = new StringBuilder("UNIT ");
            sb.Append(TextFormatter(activityIDs[activity].ToString(), 10, TextAlignment.Left));
            sb.Append(' ');
            double totalQuantity = 0;
            if (assignment.Resource is MaterialResource)
            {
              totalQuantity += assignment.UnitsPerDay * Convert.ToDouble(activity.OriginalDuration);
            }
            else if (assignment.Resource is WorkResource)
            {
              totalQuantity += assignment.UnitsPerDay * Convert.ToDouble(activity.OriginalDuration);
            }
            else if (assignment.Resource is CalendarResource)
            {
              totalQuantity += assignment.UnitsPerDay * (activity.EarlyFinishDate - activity.EarlyStartDate).TotalDays;
            }

            sb.Append(CurrencyFormatter(totalQuantity));
            sb.Append(' ');
            sb.Append(CurrencyFormatter(Convert.ToDouble(assignment.Resource.CostsPerUnit)));
            sb.Append(' ');
            double actualQuantity = 0;
            if (assignment.Resource is MaterialResource)
            {
              actualQuantity += assignment.UnitsPerDay * Convert.ToDouble(activity.ActualDuration);
            }
            else if (assignment.Resource is WorkResource)
            {
              actualQuantity += assignment.UnitsPerDay * Convert.ToDouble(activity.ActualDuration);
            }
            else if (assignment.Resource is CalendarResource)
            {
              actualQuantity += assignment.UnitsPerDay * (activity.FinishDate - activity.StartDate).TotalDays;
            }

            sb.Append(CurrencyFormatter(actualQuantity));
            sb.Append(' ');
            if (assignment.Resource is WorkResource)
            {
              sb.Append("Work");
            }
            else if (assignment.Resource is CalendarResource)
            {
              sb.Append("Days");
            }
            else if (assignment.Resource is MaterialResource)
            {
              sb.Append((assignment.Resource as MaterialResource).Unit);
            }

            writer.WriteLine(sb.ToString());
          }
        }
      }
    }

    /// <summary>
    /// Writes progress information to filestream
    /// </summary>
    /// <param _name="writer">Filestream</param>
    private void WriteProgress(StreamWriter sw)
    {
      if (schedule != null)
      {
        // PROG 1.01.04-1                   74      2820.00          .00          .00 01FEB05 20MAY05 02SEP05 23DEC05 + 145
        foreach (var act in schedule.Activities)
        {
          if (act.IsStarted)
          {
            var sb = new StringBuilder("PROG ");
            sb.Append(TextFormatter(activityIDs[act].ToString(), 10, TextAlignment.Left));
            sb.Append(' ');
            sb.Append(DateFormatter(act.ActualStartDate));
            sb.Append(' ');
            sb.Append(DateFormatter(act.ActualFinishDate));
            sb.Append(' ');
            sb.Append(TextFormatter(act.RemainingDuration.ToString(), 3, TextAlignment.Right));
            sb.Append(' ');
            sb.Append(CurrencyFormatter(Convert.ToDouble(act.TotalPlannedCosts))); // Cost
            sb.Append(' ');
            sb.Append(CurrencyFormatter(Convert.ToDouble(act.TotalActualCosts))); // Cost to _date
            sb.Append(' ');
            sb.Append(CurrencyFormatter(0)); // StoredMaterial
            sb.Append(' ');
            sb.Append(DateFormatter(act.EarlyStartDate));
            sb.Append(' ');
            sb.Append(DateFormatter(act.EarlyFinishDate));
            sb.Append(' ');
            sb.Append(DateFormatter(act.LateStartDate));
            sb.Append(' ');
            sb.Append(DateFormatter(act.LateFinishDate));
            sb.Append(' ');
            _ = act.TotalFloat < 0 ? sb.Append('-') : sb.Append('+');

            sb.Append(TextFormatter(act.TotalFloat.ToString(), 3, TextAlignment.Right));
            sw.WriteLine(sb.ToString());
          }
        }
      }
    }

    /// <summary>
    /// Writes end token to filestream
    /// </summary>
    /// <param _name="writer">Filestream</param>
    private static void WriteEndToken(StreamWriter sw)
    {
      sw.WriteLine("END");
    }

    public enum TextAlignment
    {
      Left, Right
    }

    /// <summary>
    /// Formats a given string in a way that it can be used in a export SDEF file
    /// </summary>
    /// <param _name="text">Text to be formatted</param>
    /// <param _name="length">Exact space available for the given text element</param>
    /// <returns>Formatted text element</returns>
    public static string TextFormatter(string text, int length, TextAlignment alignment)
    {
      text ??= "";

      string result = text;
      if (result.Length > length)
      {
        result = result.Substring(0, length);
      }

      result = result.Trim();
      switch (alignment)
      {
        case TextAlignment.Left:
          result = result.PadRight(length);
          break;
        case TextAlignment.Right:
          result = result.PadLeft(length);
          break;
      }
      return result;
    }

    /// <summary>
    /// Formats a given currency amount in a way that it can be used in a SDEF export file
    /// </summary>
    /// <param _name="amount">Amount to be formatted</param>
    /// <returns>New formatted string</returns>
    public static string CurrencyFormatter(double amount)
    {
      amount = Math.Round(amount, 2);
      if (amount > 999999999.99)
      {
        amount = 999999999.99;
      }

      var ci = System.Globalization.CultureInfo.GetCultureInfo("en-US");
      string result = amount.ToString("0.00", ci.NumberFormat);
      return result.PadLeft(12);
    }

    /// <summary>
    /// Formats a given _date in a way that it can be used in a SDEF export file
    /// </summary>
    /// <param _name="_date">Date to be formatted</param>
    /// <returns>New formatted string</returns>
    public static string DateFormatter(DateTime? date)
    {
      if (date == null)
      {
        return "       ";
      }

      string day = date.Value.Day.ToString().PadLeft(2, '0');
      string month = "";
      switch (date.Value.Month)
      {
        case 1:
          month = "JAN";
          break;
        case 2:
          month = "FEB";
          break;
        case 3:
          month = "MAR";
          break;
        case 4:
          month = "APR";
          break;
        case 5:
          month = "MAY";
          break;
        case 6:
          month = "JUN";
          break;
        case 7:
          month = "JUL";
          break;
        case 8:
          month = "AUG";
          break;
        case 9:
          month = "SEP";
          break;
        case 10:
          month = "OCT";
          break;
        case 11:
          month = "NOV";
          break;
        case 12:
          month = "DEC";
          break;
      }
      string year = date.Value.Year.ToString();
      if (year.Length > 2)
      {
        year = year.Substring(2, 2);
      }

      return day + month + year;
    }
  }
}

