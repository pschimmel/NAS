using NAS.Model.Entities;

namespace NAS.ViewModel.Base
{
  public interface IPrintableCanvas
  {
    Layout Layout { get; set; }
    void Refresh();
    object DataContext { get; set; }
  }
}
