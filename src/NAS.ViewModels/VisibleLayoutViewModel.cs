using System.ComponentModel;
using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.ViewModels.Base;

namespace NAS.ViewModels
{
  public abstract class VisibleLayoutViewModel : ES.Tools.Core.MVVM.ViewModel, IVisibleLayoutViewModel, INotifyPropertyChanged
  {
    #region Fields

    protected Schedule _schedule;
    private bool _isVisible = false;


    #endregion

    #region Constructor

    protected VisibleLayoutViewModel(Schedule schedule, Layout layout)
    {
      _schedule = schedule;
      Layout = layout;
      Layout.PropertyChanged += (s, e) =>
      {
        if (e.PropertyName == nameof(Layout.Name))
        {
          OnPropertyChanged(nameof(Name));
        }
      };

      Initialize();
      AttachEventHandlers();
    }

    protected abstract void Initialize();

    #endregion

    #region Properties

    public Layout Layout { get; }

    public string Name => Layout.Name;

    public abstract LayoutType LayoutType { get; }

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

    public string ActivityStandardColor => Layout.ActivityStandardColor;

    public string ActivityCriticalColor => Layout.ActivityCriticalColor;

    public string ActivityDoneColor => Layout.ActivityDoneColor;

    public string MilestoneStandardColor => Layout.MilestoneStandardColor;

    public string MilestoneCriticalColor => Layout.MilestoneCriticalColor;

    public string MilestoneDoneColor => Layout.MilestoneDoneColor;

    public bool ShowRelationships
    {
      get => Layout.ShowRelationships;
      set
      {
        if (Layout.ShowRelationships != value)
        {
          Layout.ShowRelationships = value;
        }
      }
    }

    public FilterCombinationType FilterCombination => Layout.FilterCombination;

    #endregion

    #region Event Handlers

    protected virtual void AttachEventHandlers()
    {
      Layout.PropertyChanged += Layout_PropertyChanged;
    }

    private void Layout_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      OnPropertyChanged(e.PropertyName);
    }

    #endregion
  }
}
