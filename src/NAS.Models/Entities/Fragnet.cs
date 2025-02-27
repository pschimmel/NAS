using System.Collections.ObjectModel;

namespace NAS.Models.Entities
{
  public class Fragnet : NASObject
  {
    #region Fields

    private string _number;
    private string _name;
    private string _description;
    private bool _isDisputable;
    private DateTime _identified;
    private DateTime? _approved;
    private DateTime? _submitted;
    private bool _isVisible;

    #endregion

    #region Constructor

    public Fragnet()
    {
      _identified = DateTime.Today;
      _isVisible = true;
      Distortions = [];
    }

    private Fragnet(Fragnet other)
    {
      Number = other.Number;
      Name = other.Name;
      Description = other.Description;
      IsDisputable = other.IsDisputable;
      Identified = other.Identified;
      Approved = other.Approved;
      Submitted = other.Submitted;
      IsVisible = other.IsVisible;

      foreach (var distortion in other.Distortions)
      {
        Distortions.Add(distortion.Clone());
      }
    }

    #endregion

    #region Properties

    public string Number
    {
      get => _number;
      set
      {
        if (_number != value)
        {
          _number = value;
          OnPropertyChanged(nameof(Number));
        }
      }
    }

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

    public string Description
    {
      get => _description;
      set
      {
        if (_description != value)
        {
          _description = value;
          OnPropertyChanged(nameof(Description));
        }
      }
    }

    public bool IsDisputable
    {
      get => _isDisputable;
      set
      {
        if (_isDisputable != value)
        {
          _isDisputable = value;
          OnPropertyChanged(nameof(IsDisputable));
        }
      }
    }

    public DateTime Identified
    {
      get => _identified;
      set
      {
        if (_identified != value)
        {
          _identified = value;
          OnPropertyChanged(nameof(Identified));
        }
      }
    }

    public DateTime? Approved
    {
      get => _approved;
      set
      {
        if (_approved != value)
        {
          _approved = value;
          OnPropertyChanged(nameof(Approved));
        }
      }
    }

    public DateTime? Submitted
    {
      get => _submitted;
      set
      {
        if (_submitted != value)
        {
          _submitted = value;
          OnPropertyChanged(nameof(Submitted));
        }
      }
    }

    public bool IsVisible
    {
      get => _isVisible;
      set
      {
        if (_isVisible != value)
        {
          _isVisible = value;
          OnPropertyChanged(nameof(IsVisible));
        }
      }
    }

    public ObservableCollection<Distortion> Distortions { get; }

    #endregion


    #region ICloneable

    public Fragnet Clone()
    {
      return new Fragnet(this);
    }

    #endregion
  }
}
