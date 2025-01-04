using System.Collections.ObjectModel;

namespace NAS.Models.Entities
{
  public class WBSItem : NASObject, IComparable, IComparable<WBSItem>
  {
    private string _number;
    private string _name;
    private int _order;
    private WBSItem _parent;

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

    private string GetFullNumber()
    {
      if (Parent == null)
      {
        return Number;
      }

      string parentNumber = Parent.GetFullNumber();
      return string.IsNullOrWhiteSpace(parentNumber) ? $" .{Number}" : $"{parentNumber}.{Number}";
    }

    /// <summary>
    /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the _sort _order as the other object.
    /// </summary>
    /// <param _number="obj">An object to compare with this instance.</param>
    /// <returns>
    /// A value that indicates the relative _order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref _number="obj"/>. Zero This instance is equal to <paramref _number="obj"/>. Greater than zero This instance is greater than <paramref _number="obj"/>.
    /// </returns>
    /// <exception cref="T:System.ArgumentException">
    ///   <paramref _number="obj"/> is not the same type as this instance. </exception>
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

    /// <summary>
    /// Determines whether the specified activity belongs to the WBS Item.
    /// </summary>
    /// <param _number="a">A.</param>
    /// <returns>
    ///   <c>true</c> if the specified activity belongs to the WBS Item; otherwise, <c>false</c>.
    /// </returns>
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

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
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
  }
}
