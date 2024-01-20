using System.Collections.ObjectModel;

namespace NAS.Model.Entities
{
  public class PERTDefinition : NASObject
  {
    private string name;
    private double width;
    private double height;
    private double fontSize;
    private double spacingX;
    private double spacingY;

    public PERTDefinition()
    {
      Initialize();
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    public PERTDefinition(PERTDefinition other)
    {
      Initialize();
      CreateCopy(other);
    }

    public string Name
    {
      get => name;
      set
      {
        if (name != value)
        {
          name = value;
          OnPropertyChanged(nameof(Name));
        }
      }
    }

    public double Width
    {
      get => width;
      set
      {
        if (value < 0)
        {
          value = 0;
        }

        if (width != value)
        {
          width = value;
          OnPropertyChanged(nameof(Width));
        }
      }
    }

    public double Height
    {
      get => height;
      set
      {
        if (value < 0)
        {
          value = 0;
        }

        if (height != value)
        {
          height = value;
          OnPropertyChanged(nameof(Height));
        }
      }
    }

    public double FontSize
    {
      get => fontSize;
      set
      {
        if (value < 3)
        {
          value = 3;
        }

        if (fontSize != value)
        {
          fontSize = value;
          OnPropertyChanged(nameof(FontSize));
        }
      }
    }

    public double SpacingX
    {
      get => spacingX;
      set
      {
        if (value < 0)
        {
          value = 0;
        }

        if (spacingX != value)
        {
          spacingX = value;
          OnPropertyChanged(nameof(SpacingX));
        }
      }
    }

    public double SpacingY
    {
      get => spacingY;
      set
      {
        if (value < 0)
        {
          value = 0;
        }

        if (spacingY != value)
        {
          spacingY = value;
          OnPropertyChanged(nameof(SpacingY));
        }
      }
    }

    public virtual Schedule Schedule { get; set; }

    public virtual ObservableCollection<RowDefinition> RowDefinitions { get; private set; }

    public virtual ObservableCollection<ColumnDefinition> ColumnDefinitions { get; private set; }

    public virtual ObservableCollection<PERTDataItem> Items { get; private set; }

    public virtual ObservableCollection<PERTActivityData> ActivityData { get; private set; }

    public PERTActivityData AddActivityData(Activity activity)
    {
      var result = GetActivityData(activity);
      if (result == null)
      {
        result = new PERTActivityData { ActivityID = activity.ID };
        ActivityData.Add(result);
      }
      return result;
    }

    public PERTActivityData GetActivityData(Activity activity)
    {
      return ActivityData.FirstOrDefault(x => x.ActivityID == activity.ID);
    }

    public void RemoveActivityData(Activity activity)
    {
      var items = ActivityData.Where(x => x.ActivityID == activity.ID).ToList();
      foreach (var item in items)
      {
        _ = ActivityData.Remove(item);
      }
    }

    public int Rows => RowDefinitions.Count;

    public int Columns => ColumnDefinitions.Count;

    private void Initialize()
    {
      fontSize = 12;
      spacingX = 5;
      spacingY = 5;
      height = 100;
      width = 150;
      RowDefinitions = [];
      ColumnDefinitions = [];
      Items = [];
      ActivityData = [];
    }

    private void CreateCopy(PERTDefinition other)
    {
      Name = other.Name;
      Width = other.Width;
      Height = other.Height;
      FontSize = other.FontSize;
      SpacingX = other.SpacingX;
      SpacingY = other.SpacingY;

      foreach (var otherRow in other.RowDefinitions)
      {
        RowDefinitions.Add(new RowDefinition(otherRow) { Definition = this });
      }

      foreach (var otherColumn in other.ColumnDefinitions)
      {
        ColumnDefinitions.Add(new ColumnDefinition(otherColumn) { Definition = this });
      }

      foreach (var otherItem in other.Items)
      {
        Items.Add(new PERTDataItem(otherItem) { Definition = this });
      }

      foreach (var otherData in other.ActivityData)
      {
        ActivityData.Add(new PERTActivityData(otherData));
      }
    }

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
        RowDefinitions.Add(new RowDefinition(otherRow) { Definition = this });
      }

      foreach (var otherColumn in columnDefinitions)
      {
        ColumnDefinitions.Add(new ColumnDefinition(otherColumn) { Definition = this });
      }

      foreach (var otherItem in items)
      {
        Items.Add(new PERTDataItem(otherItem) { Definition = this });
      }
    }
  }
}
