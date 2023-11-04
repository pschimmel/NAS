using System.Drawing;
using NAS.ViewModel.Base;

namespace NAS.ViewModel
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
