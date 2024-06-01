using NAS.Models.Entities;

namespace NAS.ViewModels.Base
{
  public interface IPrintableCanvas
  {
    Layout Layout { get; set; }
    void Refresh();
    object DataContext { get; set; }
  }
}
