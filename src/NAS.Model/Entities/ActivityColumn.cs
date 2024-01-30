using System.Diagnostics;
using NAS.Model.Enums;

namespace NAS.Model.Entities
{
  [DebuggerDisplay("{Property}")]
  public class ActivityColumn : NASObject
  {
    private ActivityProperty _property;
    private double? _columnWidth;
    private int _order;

    public ActivityColumn(ActivityProperty property)
    {
      _property = property;
    }

    public ActivityColumn(ActivityColumn other)
    {
      _columnWidth = other._columnWidth;
      _order = other._order;
      Property = other._property;
    }

    public ActivityProperty Property
    {
      get => _property;
      set
      {
        if (_property != value)
        {
          _property = value;
          OnPropertyChanged(nameof(Property));
        }
      }
    }

    public double? ColumnWidth
    {
      get => _columnWidth;
      set
      {
        if (_columnWidth != value)
        {
          _columnWidth = value;
          OnPropertyChanged(nameof(ColumnWidth));
        }
      }
    }

    public int Order
    {
      get => _order;
      set
      {
        if (_order != value)
        {
          _order = value;
          OnPropertyChanged(nameof(Order));
        }
      }
    }
  }
}
