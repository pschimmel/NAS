using System.IO;
using System.Xml;
using NAS.Model.Entities;
using NAS.Model.Enums;
using NAS.Resources;

namespace NAS.Model.ImportExport
{
  internal class MSProjectFilter : IImportFilter
  {
    //private ScheduleController controller;
    private Schedule schedule;
    private readonly Dictionary<int, Calendar> calendars = [];
    private readonly Dictionary<string, Activity> tasks = [];
    private readonly Dictionary<int, Resource> resources = [];

    public string FileExtension => ".xml";

    public string FilterName => NASResources.MSProjectFiles;

    public string Output => string.Empty;

    public Schedule Import(/*ScheduleController controller, */string fileName)
    {
      schedule = new Schedule(); //controller.AddEmptySchedule();
      if (!File.Exists(fileName))
      {
        throw new FileNotFoundException("File " + fileName + " not found!");
      }

      if (!fileName.ToLower().EndsWith(FileExtension.ToLower()))
      {
        throw new Exception("Wrong file extension!");
      }

      //this.controller = new ScheduleController();
      var doc = new XmlDocument();
      using (var reader = XmlReader.Create(fileName))
      {
        doc.Load(reader);
      }
      if (!doc.DocumentElement.HasAttribute("xmlns") || doc.DocumentElement.GetAttribute("xmlns") != "http://schemas.microsoft.com/project")
      {
        throw new Exception("Wrong file type!");
      }

      XmlNode documentNode = doc.DocumentElement;
      SetCustomFields(documentNode);
      GetProjectData(documentNode);
      foreach (XmlNode node in documentNode.ChildNodes)
      {
        switch (node.Name)
        {
          case "Calendars":
            GetCalendars(node);
            break;
          case "Tasks":
            GetActivities(node);
            break;
          case "Resources":
            GetResources(node);
            break;
          case "Assignments":
            GetAssignments(node);
            break;
        }
      }
      return schedule;
    }

    private static void SetCustomFields(XmlNode documentNode)
    {
      foreach (XmlNode node in documentNode.ChildNodes)
      {
        if (node.Name == "ExtendedAttributes")
        {
          foreach (XmlNode extendedAttributeNode in node.ChildNodes)
          {
            if (extendedAttributeNode.Name == "ExtendedAttribute")
            {
              SetCustomFieldId(extendedAttributeNode);
            }
          }
        }
      }
    }

    private static void SetCustomFieldId(XmlNode parentNode)
    {
      foreach (XmlNode attributeFieldNode in parentNode.ChildNodes)
      {
        if (attributeFieldNode.Name == "FieldID")
        {
          //int fieldID = Convert.ToInt32(attributeFieldNode.InnerText); //???
        }
        else if (attributeFieldNode.Name == "Alias")
        {
          //for (int i = 0; i < fieldList.Length; i++) {
          //  if (fieldList[i] == attributeFieldNode.InnerText) {
          //    extendedAttribute.Add(fieldID, attributeFieldNode.InnerText);
          //    fieldID = -1;
          //  }
          //}
        }
      }
    }

    private void GetProjectData(XmlNode parentNode)
    {
      // Read ID, _name, _start and end dates of project
      foreach (XmlNode node in parentNode.ChildNodes)
      {
        switch (node.Name)
        {
          case "Name":
            schedule.Description = Path.GetFileNameWithoutExtension(node.InnerText);
            break;
          case "Title":
            schedule.Name = XmlStringToString(Path.GetFileNameWithoutExtension(node.InnerText), 45);
            break;
          case "StartDate":
            schedule.StartDate = XmlStringToDateTime(node.InnerText);
            break;
          case "FinishDate":
            schedule.EndDate = XmlStringToDateTime(node.InnerText);
            break;
          case "LastSaved":
            schedule.DataDate = XmlStringToDateTime(node.InnerText);
            break;
        }
      }
      //controller.SaveChanges();
    }

    private void GetCalendars(XmlNode parentNode)
    {
      foreach (XmlNode node in parentNode.ChildNodes)
      {
        if (node.Name == "Calendar" && node.FirstChild.Name == "UID" && int.TryParse(node.FirstChild.InnerText, out int id))
        {
          var calendar = new Calendar(); //controller.AddCalendar(schedule);
          calendars.Add(id, calendar);
          foreach (XmlNode calendarNode in node.ChildNodes)
          {
            switch (calendarNode.Name)
            {
              case "Name":
                calendar.Name = XmlStringToString(calendarNode.InnerText, 45);
                break;
              case "WeekDays":
                GetWorkDays(calendarNode, calendar);
                break;
            }
          }
          //controller.SaveChanges();
        }
      }
    }

    private static void GetWorkDays(XmlNode parentNode, Calendar calendar)
    {
      if (calendar == null || parentNode.Name != "WeekDays")
      {
        return;
      }

      foreach (XmlNode node in parentNode.ChildNodes)
      {
        if (node.Name == "WeekDay" && node.FirstChild.Name == "DayType" && node.FirstChild.NextSibling.Name == "DayWorking")
        {
          int dayOfWeek = Convert.ToInt32(node.FirstChild.InnerText);
          if (node.FirstChild.NextSibling.InnerText == "0")
          {
            switch (dayOfWeek)
            {
              case 1:
                calendar.Sunday = true;
                break;
              case 2:
                calendar.Monday = true;
                break;
              case 3:
                calendar.Tuesday = true;
                break;
              case 4:
                calendar.Wednesday = true;
                break;
              case 5:
                calendar.Thursday = true;
                break;
              case 6:
                calendar.Friday = true;
                break;
              case 7:
                calendar.Saturday = true;
                break;
            }
          }
          else
          {
            switch (dayOfWeek)
            {
              case 1:
                calendar.Sunday = true;
                break;
              case 2:
                calendar.Monday = true;
                break;
              case 3:
                calendar.Tuesday = true;
                break;
              case 4:
                calendar.Wednesday = true;
                break;
              case 5:
                calendar.Thursday = true;
                break;
              case 6:
                calendar.Friday = true;
                break;
              case 7:
                calendar.Saturday = true;
                break;
            }
          }
        }
      }
    }

    private void GetActivities(XmlNode parentNode)
    {
      foreach (XmlNode node in parentNode.ChildNodes)
      {
        if (node.Name == "Task" && node.FirstChild.Name == "UID")
        {
          var activity = new Activity(schedule);
          tasks.Add(node.FirstChild.InnerText, activity);
          foreach (XmlNode actNode in node.ChildNodes)
          {
            switch (actNode.Name)
            {
              case "ID":
                activity.Number = XmlStringToString(actNode.InnerText, 45);
                break;
              case "Name":
                activity.Name = XmlStringToString(actNode.InnerText, 255);
                break;
              case "CalendarUID":
                int id = Convert.ToInt32(actNode.InnerText);
                activity.Calendar = !calendars.TryGetValue(id, out var value) ? activity.Schedule.StandardCalendar : value;
                break;
              case "EarlyStart":
                activity.EarlyStartDate = XmlStringToDateTime(actNode.InnerText);
                break;
              case "LateStart":
                activity.LateStartDate = XmlStringToDateTime(actNode.InnerText);
                break;
              case "EarlyFinish":
                activity.EarlyFinishDate = XmlStringToDateTime(actNode.InnerText);
                break;
              case "LateFinish":
                activity.LateFinishDate = XmlStringToDateTime(actNode.InnerText);
                break;
              case "RemainingDuration":
                activity.RemainingDuration = XmlStringToTimeSpan(actNode.InnerText).Days;
                break;
              case "PredecessorLink":
                schedule.AddRelationship(GetRelationship(activity, actNode));
                break;
              case "PercentComplete":
                activity.PercentComplete = Convert.ToInt32(actNode.InnerText);
                break;
              case "ActualStart":
                activity.ActualStartDate = XmlStringToDateTime(actNode.InnerText);
                break;
              case "ConstraintType":
                switch (actNode.InnerText)
                {
                  //case "0":
                  //  activity.Constraint = Constraint.ConstraintType.None; // As Early as possible
                  //  activity.ConstraintDate = activity.EarlyStartDate;
                  //  break;
                  //case "1":
                  //  activity.Constraint = Constraint.ConstraintType.ZeroTotalFloat; // As Late as possible
                  //  activity.ConstraintDate = activity.LateStartDate;
                  //  break;
                  case "2":
                    activity.Constraint = ConstraintType.StartOn; // Must Start On
                    break;
                  case "3":
                    activity.Constraint = ConstraintType.EndOn; // Must End On
                    break;
                  case "4":
                    activity.Constraint = ConstraintType.StartOnOrLater; // Start not earlier as
                    break;
                  //case "5":
                  //  activity.Constraint = ConstraintType.StartOnOrEarlier; // Start not later as
                  //  break;
                  //case "6":
                  //  activity.Constraint = ConstraintType.EndOnOrLater; // End not earlier as
                  //  break;
                  case "7":
                    activity.Constraint = ConstraintType.StartOnOrLater; // End not later as
                    break;
                }
                break;
              case "ConstraintDate":
                activity.ConstraintDate = XmlStringToDateTime(actNode.InnerText);
                break;
                //case "ExtendedAttribute":
                //  ApplyExtendedAttributes(actNode, activity);
                //  break;
            }
          }
          //controller.SaveChanges();
        }
      }
    }

    //private static void ApplyExtendedAttributes(XmlNode parentNode, Activity activity)
    //{
    //int id = -1;
    //foreach (XmlNode node in parentNode.ChildNodes) {
    //  if (node.Name == "FieldID") {
    //    id = Convert.ToInt32(node.InnerText);
    //  }
    //  else if (node.Name == "Value" && id >= 0) {
    //    string s = "";
    //    if (extendedAttribute.TryGetValue(id, out s)) {
    //      switch (s) {
    //        case "FOW":
    //          activity.FeatureOfWork = node.InnerText;
    //          break;
    //        case "CATW":
    //          activity.CategoryOfWork = node.InnerText;
    //          break;
    //        case "PHAS":
    //          activity.Phase = node.InnerText;
    //          break;
    //        case "BIDI":
    //          activity.BidItem = node.InnerText;
    //          break;
    //        case "MODF":
    //          activity.Modification = node.InnerText;
    //          break;
    //        case "WRKP":
    //          int workers = 0;
    //          int.TryParse(node.InnerText, out workers);
    //          activity.WorkersPerDay = workers;
    //          break;
    //        case "AREA":
    //          activity.Area = node.InnerText;
    //          break;
    //        case "RESP":
    //          activity.Responsibility = node.InnerText;
    //          break;
    //      }
    //    }
    //    id = -1;
    //  }
    //}
    //}

    private Relationship GetRelationship(Activity activity2, XmlNode parentNode)
    {
      if (parentNode.Name != "PredecessorLink" || parentNode.FirstChild.Name != "PredecessorUID")
      {
        return null;
      }

      var activity1 = tasks[parentNode.FirstChild.InnerText];
      var relationship = new Relationship(activity1, activity2);

      foreach (XmlNode node in parentNode.ChildNodes)
      {
        switch (node.Name)
        {
          case "Type":
            switch (node.InnerText)
            {
              case "0":
                relationship.RelationshipType = RelationshipType.FinishFinish;
                break;
              case "1":
                relationship.RelationshipType = RelationshipType.FinishStart;
                break;
              //case "2":
              //  relationship.RelationshipType = RelationshipType.StartFinish;
              //  break;
              case "3":
                relationship.RelationshipType = RelationshipType.StartStart;
                break;
            }
            break;
          case "LinkLag":
            relationship.Lag = Convert.ToInt32(node.InnerText);
            break;
        }
      }
      return relationship;
    }

    private void GetResources(XmlNode parentNode)
    {
      foreach (XmlNode node in parentNode.ChildNodes)
      {
        if (node.Name == "Resource" && node.FirstChild.Name == "UID" && int.TryParse(node.FirstChild.InnerText, out int id))
        {
          var resource = new MaterialResource(); // controller.AddMaterialResource(schedule);
          resources.Add(id, resource);
          foreach (XmlNode resourceNode in node.ChildNodes)
          {
            switch (resourceNode.Name)
            {
              case "Name":
                resource.Name = XmlStringToString(resourceNode.InnerText, 45);
                break;
              case "StandardRate":
                resource.CostsPerUnit = Convert.ToDecimal(resourceNode.InnerText);
                break;
            }
          }
          //controller.SaveChanges();
        }
      }
    }

    private void GetAssignments(XmlNode parentNode)
    {
      foreach (XmlNode node in parentNode.ChildNodes)
      {
        if (node.Name == "Assignment" &&
            node.FirstChild.Name == "UID" &&
            int.TryParse(node.FirstChild.InnerText, out int id) &&
            node.FirstChild.NextSibling.Name == "TaskUID" &&
            tasks.TryGetValue(node.FirstChild.NextSibling.InnerText, out Activity activity) &&
            node.FirstChild.NextSibling.NextSibling.Name == "ResourceUID" &&
            int.TryParse(node.FirstChild.NextSibling.NextSibling.InnerText, out int resourceID) &&
            resources.TryGetValue(resourceID, out var resource))
        {
          var assignment = new ResourceAssociation(activity, resource);
          foreach (XmlNode resourceNode in node.ChildNodes)
          {
            switch (resourceNode.Name)
            {
              case "Units":
                assignment.UnitsPerDay = double.Parse(resourceNode.InnerText);
                break;
            }
          }
        }
      }
    }

    private static string XmlStringToString(string xml, int max)
    {
      return max > xml.Length ? xml : xml.Substring(0, max);
    }

    private static DateTime XmlStringToDateTime(string xmlDate)
    {
      int.TryParse(xmlDate.AsSpan(0, 4), out int year);
      int.TryParse(xmlDate.AsSpan(5, 2), out int month);
      int.TryParse(xmlDate.AsSpan(8, 2), out int day);
      int.TryParse(xmlDate.AsSpan(11, 2), out int hour);
      int.TryParse(xmlDate.AsSpan(14, 2), out int minute);
      int.TryParse(xmlDate.AsSpan(17, 2), out int second);
      year = Math.Max(year, 1900);
      month = Math.Max(month, 1);
      month = Math.Min(month, 12);
      day = Math.Max(day, 1);
      day = Math.Min(day, DateTime.DaysInMonth(year, month));
      hour = Math.Max(hour, 0);
      hour = Math.Min(hour, 23);
      minute = Math.Max(minute, 0);
      minute = Math.Min(minute, 59);
      second = Math.Max(second, 0);
      second = Math.Min(second, 59);
      return new DateTime(year, month, day, hour, minute, second);
    }

    private static TimeSpan XmlStringToTimeSpan(string xmlTimeSpan)
    {
      int idx2;
      int idx = xmlTimeSpan.IndexOf('T');
      idx2 = xmlTimeSpan.IndexOf('H');
      int.TryParse(xmlTimeSpan.AsSpan(idx, idx2 - idx), out int hours);
      idx = idx2;
      idx2 = xmlTimeSpan.IndexOf('M');
      int.TryParse(xmlTimeSpan.AsSpan(idx, idx2 - idx), out int minutes);
      idx = idx2;
      idx2 = xmlTimeSpan.IndexOf('S');
      int.TryParse(xmlTimeSpan.AsSpan(idx, idx2 - idx), out int seconds);
      hours = Math.Max(hours, 0);
      hours = Math.Min(hours, 24);
      minutes = Math.Max(minutes, 0);
      minutes = Math.Min(minutes, 59);
      seconds = Math.Max(seconds, 0);
      seconds = Math.Min(seconds, 59);
      return new TimeSpan(hours, minutes, seconds);
    }
  }
}
