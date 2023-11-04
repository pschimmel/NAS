using System;

namespace NAS.Model.Entities
{
  public class Holiday : NASObject
  {
    private DateTime date;

    public DateTime Date
    {
      get => date;
      set
      {
        if (date != value)
        {
          date = value;
          OnPropertyChanged(nameof(Date));
        }
      }
    }

    public virtual Calendar Calendar { get; set; }
  }
}
