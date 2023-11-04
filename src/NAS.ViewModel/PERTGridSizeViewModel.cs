using NAS.ViewModel.Base;

namespace NAS.ViewModel
{
  public class PERTGridSizeViewModel : ViewModelBase
  {
    public PERTGridSizeViewModel(double? size)
    {
      AutoSize = size == null;

      if (size != null)
      {
        Size = size.Value;
      }
    }

    public double Size { get; set; } = 0;

    public bool AutoSize { get; set; }
  }
}
