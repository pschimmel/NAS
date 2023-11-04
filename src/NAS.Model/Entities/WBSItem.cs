using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NAS.Model.Entities
{
  public class WBSItem : NASObject, IComparable, IComparable<WBSItem>
  {
    private string number;
    private string name;
    private int order;
    private WBSItem parent;

    public WBSItem()
    {
      order = 0;
      Activities = new ObservableCollection<Activity>();
      Children = new ObservableCollection<WBSItem>();
    }

    public string Number
    {
      get => number;
      set
      {
        if (number != value)
        {
          number = value;
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
      get => name;
      set
      {
        if (name != value)
        {
          name = value;
          OnPropertyChanged(nameof(Name));
          OnPropertyChanged(nameof(FullName));
        }
      }
    }

    public int Order
    {
      get => order;
      set
      {
        if (order != value)
        {
          order = value;
          OnPropertyChanged(nameof(Order));
        }
      }
    }

    public virtual ICollection<Activity> Activities { get; set; }

    public virtual Schedule Schedule { get; set; }

    public virtual ICollection<WBSItem> Children { get; set; }

    public virtual WBSItem Parent
    {
      get => parent;
      set
      {
        if (parent != value)
        {
          parent = value;
          OnPropertyChanged(nameof(Parent));
        }
      }
    }

    public string FullName
    {
      get
      {
        string number = GetFullNumber();
        if (string.IsNullOrWhiteSpace(number))
        {
          return Name;
        }
        else
        {
          return number + " " + Name;
        }
      }
    }

    private string GetFullNumber()
    {
      if (Parent == null)
      {
        return Number;
      }

      string parentNumber = Parent.GetFullNumber();
      return string.IsNullOrWhiteSpace(parentNumber) ? " ." + Number : parentNumber + "." + Number;
    }

    /// <summary>
    /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param number="obj">An object to compare with this instance.</param>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref number="obj"/>. Zero This instance is equal to <paramref number="obj"/>. Greater than zero This instance is greater than <paramref number="obj"/>.
    /// </returns>
    /// <exception cref="T:System.ArgumentException">
    ///   <paramref number="obj"/> is not the same type as this instance. </exception>
    int IComparable.CompareTo(object obj)
    {
      if (obj == null)
      {
        return -1;
      }

      if (obj is WBSItem other)
      {
        return CompareTo(other);
      }
      else
      {
        throw new ArgumentException("Obj is not the same type as this instance");
      }
    }

    public int CompareTo(WBSItem other)
    {
      return Order.CompareTo(other.Order);
    }

    public override bool Equals(object obj)
    {
      return !(obj is WBSItem other) ? false : Equals(FullName, other.FullName);
    }

    public override int GetHashCode()
    {
      return 7 ^ FullName.GetHashCode();
    }

    /// <summary>
    /// Determines whether the specified activity belongs to the WBS Item.
    /// </summary>
    /// <param number="a">A.</param>
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
      return left is null ? !(right is null) : left.CompareTo(right) < 0;
    }

    public static bool operator <=(WBSItem left, WBSItem right)
    {
      return left is null || left.CompareTo(right) <= 0;
    }

    public static bool operator >(WBSItem left, WBSItem right)
    {
      return !(left is null) && left.CompareTo(right) > 0;
    }

    public static bool operator >=(WBSItem left, WBSItem right)
    {
      return left is null ? right is null : left.CompareTo(right) >= 0;
    }
  }
}
