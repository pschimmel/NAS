using NAS.Model.Entities;

namespace NAS.Model.ImportExport
{
  public interface IImportFilter : IFilter
  {
    Schedule Import(string fileName);
  }
}
