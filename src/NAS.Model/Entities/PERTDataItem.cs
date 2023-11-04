using NAS.Model.Enums;

namespace NAS.Model.Entities
{
  public class PERTDataItem : NASObject
  {
    private ActivityProperty property;
    private int row;
    private int column;
    private int rowSpan;
    private int columnSpan;
    private HorizontalAlignment horizontalAlignment;
    private VerticalAlignment verticalAlignment;

    public PERTDataItem()
    {
      property = ActivityProperty.None;
      row = 0;
      column = 0;
      rowSpan = 1;
      columnSpan = 1;
      horizontalAlignment = HorizontalAlignment.Center;
      verticalAlignment = VerticalAlignment.Center;
    }

    public PERTDataItem(PERTDefinition definition, ActivityProperty property)
      : this()
    {
      Definition = definition;
      Property = property;
    }

    public PERTDataItem(PERTDataItem other)
    {
      Definition = other.Definition;
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
      get => property;
      set
      {
        if (property != value)
        {
          property = value;
          OnPropertyChanged(nameof(Property));
        }
      }
    }

    public int Row
    {
      get => row;
      set
      {
        if (value < 0)
        {
          value = 0;
        }

        if (row != value)
        {
          row = value;
          OnPropertyChanged(nameof(Row));
        }
      }
    }

    public int Column
    {
      get => column;
      set
      {
        if (value < 0)
        {
          value = 0;
        }

        if (column != value)
        {
          column = value;
          OnPropertyChanged(nameof(Column));
        }
      }
    }

    public int RowSpan
    {
      get => rowSpan;
      set
      {
        if (value < 1)
        {
          value = 1;
        }

        if (rowSpan != value)
        {
          rowSpan = value;
          OnPropertyChanged(nameof(RowSpan));
        }
      }
    }

    public int ColumnSpan
    {
      get => columnSpan;
      set
      {
        if (value < 1)
        {
          value = 1;
        }

        if (columnSpan != value)
        {
          columnSpan = value;
          OnPropertyChanged(nameof(ColumnSpan));
        }
      }
    }

    public HorizontalAlignment HorizontalAlignment
    {
      get => horizontalAlignment;
      set
      {
        if (horizontalAlignment != value)
        {
          horizontalAlignment = value;
          OnPropertyChanged(nameof(HorizontalAlignment));
        }
      }
    }

    public VerticalAlignment VerticalAlignment
    {
      get => verticalAlignment;
      set
      {
        if (verticalAlignment != value)
        {
          verticalAlignment = value;
          OnPropertyChanged(nameof(VerticalAlignment));
        }
      }
    }

    public virtual PERTDefinition Definition { get; set; }
  }
}
