using ES.Tools.Core.MVVM;
using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class SelectActivityViewModel : DialogContentViewModel
  {
    #region Fields

    private Activity _selectedActivity;
    private string _textFilter;

    #endregion

    #region Constructor

    public SelectActivityViewModel(Schedule schedule)
      : base()
    {
      Activities = new List<Activity>(schedule.Activities);
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.Activity;

    public override string Icon => "Activity";

    public override DialogSize DialogSize => DialogSize.Fixed(350, 350);

    public override HelpTopic HelpTopicKey => HelpTopic.Activity;

    #endregion

    #region Properties

    public List<Activity> Activities { get; }

    public Activity SelectedActivity
    {
      get => _selectedActivity;
      set
      {
        if (_selectedActivity != value)
        {
          _selectedActivity = value;
          OnPropertyChanged(nameof(SelectedActivity));
        }
      }
    }

    public string TextFilter
    {
      get => _textFilter;
      set
      {
        if (_textFilter != value)
        {
          _textFilter = value;
          var view = Activities.GetView();
          if (view != null)
          {
            view.Filter = new Predicate<object>(Filter);
            OnPropertyChanged(nameof(Activities));
          }
          OnPropertyChanged(nameof(TextFilter));
        }
      }
    }

    #endregion

    #region Private Members

    private bool Filter(object obj)
    {
      if (obj is not Activity item)
      {
        return false;
      }

      string t = null;
      if (_textFilter != null)
      {
        t = _textFilter.ToLower();
      }

      return string.IsNullOrWhiteSpace(t) ||
        item.Number != null && item.Number.Contains(t, StringComparison.CurrentCultureIgnoreCase) ||
        item.Name != null && item.Name.Contains(t, StringComparison.CurrentCultureIgnoreCase);
    }

    #endregion
  }
}
