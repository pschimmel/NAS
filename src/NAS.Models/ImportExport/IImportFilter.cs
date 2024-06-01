using NAS.Models.Entities;

namespace NAS.Models.ImportExport
{
  public interface IImportFilter : IFilter
  {
    Schedule Import(string fileName);
  }
}
