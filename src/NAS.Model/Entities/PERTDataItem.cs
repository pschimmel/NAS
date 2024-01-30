using NAS.Model.Enums;

namespace NAS.Model.Entities
{
  public class PERTDataItem : NASObject
  {
    private ActivityProperty _property;
    private int _row;
    private int _column;
    private int _rowSpan;
    private int _columnSpan;
    private HorizontalAlignment _horizontalAlignment;
    private VerticalAlignment _verticalAlignment;

    public PERTDataItem()
    {
      _property = ActivityProperty.None;
      _row = 0;
      _column = 0;
      _rowSpan = 1;
      _columnSpan = 1;
      _horizontalAlignment = HorizontalAlignment.Center;
      _verticalAlignment = VerticalAlignment.Center;
    }

    public PERTDataItem(ActivityProperty property)
      : this()
    {
      Property = property;
    }

    public PERTDataItem(PERTDataItem other)
    {
      Property = other.Property;
      Column = other.Column;
      ColumnSpan = other.ColumnSpan;
      HorizontalAlignment = other.HorizontalAlignment;
      Row = other.Row;
      RowSpan = other.RowSpan;
      VerticalAlignment = other.VerticalAlignment;
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

    public int Row
    {
      get => _row;
      set
      {
        if (value < 0)
        {
          value = 0;
        }

        if (_row != value)
        {
          _row = value;
          OnPropertyChanged(nameof(Row));
        }
      }
    }

    public int Column
    {
      get => _column;
      set
      {
        if (value < 0)
        {
          value = 0;
        }

        if (_column != value)
        {
          _column = value;
          OnPropertyChanged(nameof(Column));
        }
      }
    }

    public int RowSpan
    {
      get => _rowSpan;
      set
      {
        if (value < 1)
        {
          value = 1;
        }

        if (_rowSpan != value)
        {
          _rowSpan = value;
          OnPropertyChanged(nameof(RowSpan));
        }
      }
    }

    public int ColumnSpan
    {
      get => _columnSpan;
      set
      {
        if (value < 1)
        {
          value = 1;
        }

        if (_columnSpan != value)
        {
          _columnSpan = value;
          OnPropertyChanged(nameof(ColumnSpan));
        }
      }
    }

    public HorizontalAlignment HorizontalAlignment
    {
      get => _horizontalAlignment;
      set
      {
        if (_horizontalAlignment != value)
        {
          _horizontalAlignment = value;
          OnPropertyChanged(nameof(HorizontalAlignment));
        }
      }
    }

    public VerticalAlignment VerticalAlignment
    {
      get => _verticalAlignment;
      set
      {
        if (_verticalAlignment != value)
        {
          _verticalAlignment = value;
          OnPropertyChanged(nameof(VerticalAlignment));
        }
      }
    }
  }
}
