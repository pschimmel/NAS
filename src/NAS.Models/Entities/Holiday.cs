namespace NAS.Models.Entities
{
  public class Holiday : NASObject
  {
    public Holiday()
    { }

    private Holiday(Holiday other)
    {
      Date = other.Date;
    }

    public DateTime Date { get; set; }

    public Holiday Clone()
    {
      return new Holiday(this);
    }
  }
}
