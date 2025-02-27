namespace NAS.Models.Entities
{
  public class WorkResource : Resource
  {
    public WorkResource()
    { }

    protected WorkResource(WorkResource other)
      : base(other)
    { }

    public override Resource Clone()
    {
      return new WorkResource(this);
    }
  }
}
