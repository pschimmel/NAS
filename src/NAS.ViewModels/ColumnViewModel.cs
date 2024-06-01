using System.Collections.Generic;
using NAS.Models.Enums;

namespace NAS.ViewModels
{
  public class ColumnViewModel : ES.Tools.Core.MVVM.ViewModel
  {
    private bool _isVisible;
    private double? _width;
    private int _order;

    public ColumnViewModel(ActivityProperty property, IEnumerable<object> itemsSource = null)
    {
      Property = property;
      Name = ActivityPropertyHelper.GetNameOfActivityProperty(property);
      IsReadonly = ActivityPropertyHelper.IsPropertyReadonly(property);
      ItemsSource = itemsSource;
    }

    public ColumnViewModel(ColumnViewModel oldVM)
    {
      Property = oldVM.Property;
      Name = oldVM.Name;
      _isVisible = oldVM._isVisible;
      ItemsSource = oldVM.ItemsSource;
    }

    public ActivityProperty Property { get; }

    public string Name { get; }

    public bool IsReadonly { get; }

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

    public double? Width
    {
      get => _width;
      set
      {
        if (_width != value)
        {
          _width = value;
          OnPropertyChanged(nameof(Width));
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

    private IEnumerable<object> ItemsSource { get; }
  }
}
