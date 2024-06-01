using NAS.Models.Entities;

namespace NAS.Models.ImportExport
{
  public class Persistency
  {
    public static Schedule Load(string fileName)
    {
      var filter = new NASFilter();
      var schedule = filter.Import(fileName);
      schedule.CreatedDate = DateTime.Now;
      schedule.CreatedBy = Globals.UserName;
      schedule.FileName = fileName;
      return schedule;
    }

    public static void Save(Schedule schedule, string fileName)
    {
      if (!schedule.CreatedDate.HasValue)
      {
        schedule.CreatedDate = DateTime.Now;
      }

      if (string.IsNullOrWhiteSpace(schedule.CreatedBy))
      {
        schedule.CreatedBy = Globals.UserName;
      }

      schedule.ModifiedBy = Globals.UserName;
      schedule.ModifiedDate = DateTime.Now;
      schedule.FileName = fileName;

      var filter = new NASFilter();
      filter.Export(schedule, fileName);
    }
  }
}
