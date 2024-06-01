using System;
using ES.Tools.Core.MVVM;

namespace NAS.ViewModels.Base
{
  public class SelectableItem<T> : NotifyObject
  {
    public event EventHandler SelectionChanged;

    private bool isSelected = false;
    private string name;

    public SelectableItem(T item, string name = null)
    {
      if (item == null)
      {
        throw new ArgumentNullException(nameof(item));
      }

      Item = item;
      this.name = name ?? item.ToString();
    }

    public T Item { get; }

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

    public bool IsSelected
    {
      get => isSelected;
      set
      {
        if (isSelected != value)
        {
          isSelected = value;
          OnPropertyChanged(nameof(IsSelected));
          SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
      }
    }

    public override string ToString()
    {
      return Name;
    }

    public override bool Equals(object obj)
    {
      return obj is SelectableItem<T> other && Item.Equals(other.Item);
    }

    public override int GetHashCode()
    {
      return Item.GetHashCode();
    }
  }
}
