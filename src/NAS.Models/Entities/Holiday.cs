namespace NAS.Models.Entities
{
  public class Holiday : NASObject
  {
    private DateTime _date;

    public DateTime Date
    {
      get => _date;
      set
      {
        if (_date != value)
        {
          _date = value;
          OnPropertyChanged(nameof(Date));
        }
      }
    }
  }
}
