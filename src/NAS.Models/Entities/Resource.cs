namespace NAS.Models.Entities
{
  public abstract class Resource : NASObject
  {
    private string _name;
    private decimal _costsPerUnit;
    private double? _limit;

    public string Name
    {
      get => _name;
      set
      {
        if (_name != value)
        {
          _name = value;
          OnPropertyChanged(nameof(Name));
        }
      }
    }

    public decimal CostsPerUnit
    {
      get => _costsPerUnit;
      set
      {
        if (_costsPerUnit != value)
        {
          _costsPerUnit = value;
          OnPropertyChanged(nameof(CostsPerUnit));
        }
      }
    }

    public double? Limit
    {
      get => _limit;
      set
      {
        if (_limit != value)
        {
          _limit = value;
          OnPropertyChanged(nameof(Limit));
        }
      }
    }
  }
}
