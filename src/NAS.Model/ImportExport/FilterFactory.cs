namespace NAS.Model.ImportExport
{
  public static class FilterFactory
  {
    private static IEnumerable<IFilter> Filters => new IFilter[]
    {
      new NASFilter(),       // Always use this first
      new PrimaveraFilter(),
      new MSProjectFilter(),
      new SDEFFilter()
    };

    public static IEnumerable<IImportFilter> ImportFilters => Filters.OfType<IImportFilter>();

    public static IEnumerable<IExportFilter> ExportFilters => Filters.OfType<IExportFilter>();
  }
}
