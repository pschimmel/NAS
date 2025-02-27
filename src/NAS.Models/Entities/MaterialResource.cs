namespace NAS.Models.Entities
{
  public class MaterialResource : Resource
  {
    public MaterialResource()
    { }

    public MaterialResource(MaterialResource other)
    {
      Unit = other.Unit;
    }

    public string Unit { get; set; }

    public override Resource Clone()
    {
      return new MaterialResource(this);
    }
  }
}
