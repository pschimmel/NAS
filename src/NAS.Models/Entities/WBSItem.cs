using System.Collections.ObjectModel;

namespace NAS.Models.Entities
{
  public class WBSItem : NASObject, IComparable, IComparable<WBSItem>
  {
    #region Fields

    private string _number;
    private string _name;
    private int _order;
    private WBSItem _parent;

    #endregion

    #region Constructors

    public WBSItem(WBSItem parent = null)
    {
      _order = 0;
      _parent = parent;
      Children = [];
      Children.CollectionChanged += (s, e) =>
      {
        if (e.NewItems != null)
        {
          foreach (var child in e.NewItems.Cast<WBSItem>())
          {
            child.Parent = this;
          }
        }
        if (e.OldItems != null)
        {
          foreach (var child in e.OldItems.Cast<WBSItem>())
          {
            child.Parent = null;
          }
        }
      };
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
          OnPropertyChanged(nameof(FullName));
          foreach (var child in Children)
          {
            child.OnPropertyChanged(nameof(FullName));
          }
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
          OnPropertyChanged(nameof(FullName));
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

    public ObservableCollection<WBSItem> Children { get; }

    public WBSItem Parent
    {
      get => _parent;
      set
      {
        if (_parent != value)
        {
          _parent = value;
          OnPropertyChanged(nameof(Parent));
          OnPropertyChanged(nameof(FullName));
        }
      }
    }

    public string FullName
    {
      get
      {
        string number = GetFullNumber();
        return string.IsNullOrWhiteSpace(number) ? Name : $"{number} {Name}";
      }
    }

    #endregion

    #region Private Methods

    private string GetFullNumber()
    {
      if (Parent == null)
      {
        return Number;
      }

      string parentNumber = Parent.GetFullNumber();
      return string.IsNullOrWhiteSpace(parentNumber) ? $" .{Number}" : $"{parentNumber}.{Number}";
    }

    #endregion

    #region Public Methods

    int IComparable.CompareTo(object obj)
    {
      return obj is not WBSItem other ? -1 : CompareTo(other);
    }

    public int CompareTo(WBSItem other)
    {
      return Order.CompareTo(other.Order);
    }

    public override bool Equals(object obj)
    {
      return obj is WBSItem other && Equals(FullName, other.FullName);
    }

    public override int GetHashCode()
    {
      return 7 ^ FullName.GetHashCode();
    }

    public bool ContainsItem(Activity a)
    {
      if (a.WBSItem == this)
      {
        return true;
      }

      foreach (var item in Children)
      {
        if (item.ContainsItem(a))
        {
          return true;
        }
      }
      return false;
    }

    public override string ToString()
    {
      return FullName;
    }

    public static bool operator ==(WBSItem left, WBSItem right)
    {
      return left is null ? right is null : left.Equals(right);
    }

    public static bool operator !=(WBSItem left, WBSItem right)
    {
      return !(left == right);
    }

    public static bool operator <(WBSItem left, WBSItem right)
    {
      return left is null ? right is not null : left.CompareTo(right) < 0;
    }

    public static bool operator <=(WBSItem left, WBSItem right)
    {
      return left is null || left.CompareTo(right) <= 0;
    }

    public static bool operator >(WBSItem left, WBSItem right)
    {
      return left is not null && left.CompareTo(right) > 0;
    }

    public static bool operator >=(WBSItem left, WBSItem right)
    {
      return left is null ? right is null : left.CompareTo(right) >= 0;
    }

    #endregion

    #region ICloneable

    public WBSItem Clone()
    {
      var clone = new WBSItem();

      foreach (var child in Children)
      {
        var childClone = child.Clone();
        childClone.Parent = clone;
        clone.Children.Add(clone);
      }

      return clone;
    }

    #endregion
  }
}
