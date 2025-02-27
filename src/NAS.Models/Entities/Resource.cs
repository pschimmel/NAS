namespace NAS.Models.Entities
{
  public abstract class Resource : NASObject
  {
    #region Fields

    private string _name;
    private decimal _costsPerUnit;
    private double? _limit;

    #endregion

    #region Constructor

    protected Resource()
    { }

    protected Resource(Resource other)
    {
      Name = other.Name;
      CostsPerUnit = other.CostsPerUnit;
      Limit = other.Limit;
    }

    #endregion

    #region Properties

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

    #endregion

    #region ICloneable

    public abstract Resource Clone();

    #endregion
  }
}
