using System.Collections.ObjectModel;

namespace NAS.Models.Entities
{
  public class PERTDefinition : NASObject
  {
    private string _name;
    private double _width = 150;
    private double _height = 100;
    private double _fontSize = 12;
    private double _spacingX = 5;
    private double _spacingY = 5;

    public PERTDefinition(Schedule schedule = null)
    {
      RowDefinitions = [];
      ColumnDefinitions = [];
      Items = [];
      ActivityData = [];
      Schedule = schedule;
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    public PERTDefinition(PERTDefinition other)
      : this()
    {
      Name = other.Name;
      Width = other.Width;
      Height = other.Height;
      FontSize = other.FontSize;
      SpacingX = other.SpacingX;
      SpacingY = other.SpacingY;
      Schedule = other.Schedule;

      foreach (var otherRow in other.RowDefinitions)
      {
        RowDefinitions.Add(new RowDefinition(otherRow));
      }

      foreach (var otherColumn in other.ColumnDefinitions)
      {
        ColumnDefinitions.Add(new ColumnDefinition(otherColumn));
      }

      foreach (var otherItem in other.Items)
      {
        Items.Add(new PERTDataItem(otherItem));
      }

      foreach (var otherData in other.ActivityData)
      {
        ActivityData.Add(new PERTActivityData(otherData));
      }
    }

    public Schedule Schedule { get; private set; }

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

    public double Width
    {
      get => _width;
      set
      {
        if (value < 0)
        {
          value = 0;
        }

        if (_width != value)
        {
          _width = value;
          OnPropertyChanged(nameof(Width));
        }
      }
    }

    public double Height
    {
      get => _height;
      set
      {
        if (value < 0)
        {
          value = 0;
        }

        if (_height != value)
        {
          _height = value;
          OnPropertyChanged(nameof(Height));
        }
      }
    }

    public double FontSize
    {
      get => _fontSize;
      set
      {
        if (value < 3)
        {
          value = 3;
        }

        if (_fontSize != value)
        {
          _fontSize = value;
          OnPropertyChanged(nameof(FontSize));
        }
      }
    }

    public double SpacingX
    {
      get => _spacingX;
      set
      {
        if (value < 0)
        {
          value = 0;
        }

        if (_spacingX != value)
        {
          _spacingX = value;
          OnPropertyChanged(nameof(SpacingX));
        }
      }
    }

    public double SpacingY
    {
      get => _spacingY;
      set
      {
        if (value < 0)
        {
          value = 0;
        }

        if (_spacingY != value)
        {
          _spacingY = value;
          OnPropertyChanged(nameof(SpacingY));
        }
      }
    }

    public ObservableCollection<RowDefinition> RowDefinitions { get; }

    public ObservableCollection<ColumnDefinition> ColumnDefinitions { get; }

    public ObservableCollection<PERTDataItem> Items { get; }

    public ObservableCollection<PERTActivityData> ActivityData { get; }

    public PERTActivityData AddActivityData(Activity activity)
    {
      var result = GetActivityData(activity);
      if (result == null)
      {
        result = new PERTActivityData(activity);
        ActivityData.Add(result);
      }
      return result;
    }

    public PERTActivityData GetActivityData(Activity activity)
    {
      return ActivityData.FirstOrDefault(x => x.Activity.ID == activity.ID);
    }

    public void RemoveActivityData(Activity activity)
    {
      var items = ActivityData.Where(x => x.Activity.ID == activity.ID).ToList();
      foreach (var item in items)
      {
        ActivityData.Remove(item);
      }
    }

    public int Rows => RowDefinitions.Count;

    public int Columns => ColumnDefinitions.Count;

    public void RefreshData(IEnumerable<RowDefinition> rowDefinitions, IEnumerable<ColumnDefinition> columnDefinitions, IEnumerable<PERTDataItem> items)
    {
      if (rowDefinitions == null)
      {
        throw new ArgumentNullException(nameof(rowDefinitions), "Argument can't be null");
      }

      if (columnDefinitions == null)
      {
        throw new ArgumentNullException(nameof(columnDefinitions), "Argument can't be null");
      }

      if (items == null)
      {
        throw new ArgumentNullException(nameof(items), "Argument can't be null");
      }

      RowDefinitions.Clear();
      ColumnDefinitions.Clear();
      Items.Clear();

      foreach (var otherRow in rowDefinitions)
      {
        RowDefinitions.Add(new RowDefinition(otherRow));
      }

      foreach (var otherColumn in columnDefinitions)
      {
        ColumnDefinitions.Add(new ColumnDefinition(otherColumn));
      }

      foreach (var otherItem in items)
      {
        Items.Add(new PERTDataItem(otherItem));
      }
    }

    public PERTDefinition Clone()
    {
      return new PERTDefinition(this);
    }
  }
}
