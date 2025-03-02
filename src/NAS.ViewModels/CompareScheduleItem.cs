using NAS.Models.Entities;

namespace NAS.ViewModels
{
  public class CompareScheduleItem
  {
    public CompareScheduleItem(Fragnet fragnet)
    {
      Fragnet = fragnet;
    }

    public Fragnet Fragnet { get; private set; }

    public string Name => Fragnet.Name;

    public bool IsChecked { get; set; }
  }
}
