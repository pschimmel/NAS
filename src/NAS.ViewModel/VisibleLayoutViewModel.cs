using System.ComponentModel;
using NAS.Model.Entities;
using NAS.Model.Enums;
using NAS.ViewModel.Base;

namespace NAS.ViewModel
{
  public abstract class VisibleLayoutViewModel : ES.Tools.Core.MVVM.ViewModel, IVisibleLayoutViewModel, INotifyPropertyChanged
  {
    #region Fields

    private bool isVisible = false;

    #endregion

    #region Constructor

    protected VisibleLayoutViewModel(Layout layout)
    {
      Layout = layout;
      Layout.PropertyChanged += (s, e) =>
      {
        if (e.PropertyName == nameof(Layout.Name))
        {
          OnPropertyChanged(nameof(Name));
        }
      };
      Schedule = layout.Schedule;
      Initialize();
      AttachEventHandlers();
    }

    protected abstract void Initialize();

    #endregion

    #region Properties

    public Layout Layout { get; }

    public string Name => Layout.Name;

    protected Schedule Schedule { get; }

    public abstract LayoutType LayoutType { get; }

    public bool IsVisible
    {
      get => isVisible;
      set
      {
        if (isVisible != value)
        {
          isVisible = value;
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
