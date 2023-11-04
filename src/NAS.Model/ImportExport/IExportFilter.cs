using NAS.Model.Entities;

namespace NAS.Model.ImportExport
{
  public interface IExportFilter : IFilter
  {
    void Export(Schedule schedule, string fileName);
  }
}
