using NAS.Models.Entities;
using NAS.Models.Enums;

namespace NAS.ViewModels.Base
{
  public interface IVisibleLayoutViewModel : IDisposable
  {
    bool IsVisible { get; set; }
    Layout Layout { get; }
    string Name { get; }
    LayoutType LayoutType { get; }
  }
}
