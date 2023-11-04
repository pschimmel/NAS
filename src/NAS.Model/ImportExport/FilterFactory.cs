using System.Collections.Generic;
using System.Linq;

namespace NAS.Model.ImportExport
{
  public static class FilterFactory
  {
    public static IEnumerable<IFilter> Filters => new IFilter[] { new NASFilter(), new PrimaveraFilter(), new MSProjectFilter(), new SDEFFilter() };
    public static IEnumerable<IImportFilter> ImportFilters => Filters.OfType<IImportFilter>();
    public static IEnumerable<IExportFilter> ExportFilters => Filters.OfType<IExportFilter>();
  }
}
