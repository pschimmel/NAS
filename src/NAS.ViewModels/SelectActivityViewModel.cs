using System;
using System.Collections.Generic;
using NAS.Models.Entities;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class SelectActivityViewModel : ViewModelBase
  {
    #region Fields

    private List<Activity> activities;
    private Activity selectedActivity;
    private string textFilter;

    #endregion

    #region Constructor

    public SelectActivityViewModel(Schedule schedule)
      : base()
    {
      Schedule = schedule;
    }

    #endregion

    #region Public Properties

    public override HelpTopic HelpTopicKey => HelpTopic.Relationship;

    public Schedule Schedule { get; private set; }

    public List<Activity> Activities
    {
      get
      {
        activities ??= new List<Activity>(Schedule.Activities);
        return activities;
      }
    }

    public Activity SelectedActivity
    {
      get => selectedActivity;
      set
      {
        if (selectedActivity != value)
        {
          selectedActivity = value;
          OnPropertyChanged(nameof(SelectedActivity));
        }
      }
    }

    public string TextFilter
    {
      get => textFilter;
      set
      {
        if (textFilter != value)
        {
          textFilter = value;
          OnPropertyChanged(nameof(TextFilter));
          DoFilter();
        }
      }
    }

    #endregion

    #region Private Members

    private void DoFilter()
    {
      var view = ES.Tools.Core.MVVM.ViewModelExtensions.GetView(Activities);
      if (view != null)
      {
        view.Filter = new Predicate<object>(Filter);
        OnPropertyChanged(nameof(Activities));
      }
    }

    private bool Filter(object obj)
    {
      if (obj is not Activity item)
      {
        return false;
      }

      string t = null;
      if (textFilter != null)
      {
        t = textFilter.ToLower();
      }

      return string.IsNullOrWhiteSpace(t) ||
        item.Number != null && item.Number.ToLower().Contains(t) ||
        item.Name != null && item.Name.ToLower().Contains(t);
    }

    #endregion
  }
}
