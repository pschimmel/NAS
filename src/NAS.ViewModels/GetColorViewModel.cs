using System.Drawing;
using NAS.ViewModels.Base;

namespace NAS.ViewModels
{
  public class GetColorViewModel : ViewModelBase
  {
    public GetColorViewModel(Color? color)
    {
      Color = color;
    }

    public Color? Color { get; set; }
  }
}
