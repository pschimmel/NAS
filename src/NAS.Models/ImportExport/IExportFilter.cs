using NAS.Models.Entities;

namespace NAS.Models.ImportExport
{
  public interface IExportFilter : IFilter
  {
    void Export(Schedule schedule, string fileName);
  }
}
