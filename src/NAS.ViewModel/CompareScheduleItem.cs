using NAS.Model.Entities;

namespace NAS.ViewModel
{
  public class CompareScheduleItem
  {
    public CompareScheduleItem(Fragnet fragnet)
    {
      Fragnet = fragnet;
    }

    public Fragnet Fragnet { get; private set; }

    public string Name => Fragnet.ToString();

    public bool IsChecked { get; set; }
  }
}
