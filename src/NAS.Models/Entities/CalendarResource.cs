namespace NAS.Models.Entities
{
  public class CalendarResource : Resource
  {
    public CalendarResource()
    { }

    protected CalendarResource(CalendarResource other)
      : base(other)
    { }


    public override Resource Clone()
    {
      return new CalendarResource(this);
    }
  }
}
