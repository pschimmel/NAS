
namespace NAS.Model.ImportExport
{
  public interface IFilter
  {
    string FilterName { get; }
    string FileExtension { get; }
    string Output { get; }
  }
}
