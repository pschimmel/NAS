using System.Collections.ObjectModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Model.Entities;
using NAS.Resources;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel
{
  public class FragnetViewModel : ValidatingViewModel, IApplyable
  {
    #region Fields

    private readonly Schedule _schedule;
    private readonly Fragnet _fragnet;
    private Activity _currentFragnetActivity;

    #endregion

    #region Constructor

    public FragnetViewModel(Schedule schedule, Fragnet fragnet)
      : base()
    {
      _schedule = schedule;
      _fragnet = fragnet;
      FragnetActivities = new ObservableCollection<Activity>(_fragnet.Activities);
      Number = fragnet.Number;
      Name = fragnet.Name;
      AddActivityToFragnetCommand = new ActionCommand(AddActivityToFragnetCommandExecute);
      RemoveActivityFromFragnetCommand = new ActionCommand(RemoveActivityFromFragnetCommandExecute, () => RemoveActivityFromFragnetCommandCanExecute);
    }

    #endregion

    #region Public Properties

    public string Number { get; set; }

    public string Name { get; set; }

    public ObservableCollection<Activity> FragnetActivities { get; }

    public Activity CurrentFragnetActivity
    {
      get => _currentFragnetActivity;
      set
      {
        if (_currentFragnetActivity != value)
        {
          _currentFragnetActivity = value;
          OnPropertyChanged(nameof(CurrentFragnetActivity));
        }
      }
    }

    #endregion

    #region Add Activity to Fragnet

    public ICommand AddActivityToFragnetCommand { get; }

    private void AddActivityToFragnetCommandExecute()
    {
      using var vm = new SelectActivityViewModel(_schedule);
      if (ViewFactory.Instance.ShowDialog(vm) == true && vm.SelectedActivity != null)
      {
        if (vm.SelectedActivity.Fragnet == _fragnet)
        {
          UserNotificationService.Instance.Information(string.Format(NASResources.MessageActivityAlreadyAssignedToFragnet, vm.SelectedActivity, vm.SelectedActivity.Fragnet));
          return;
        }

        FragnetActivities.Add(vm.SelectedActivity);
        CurrentFragnetActivity = vm.SelectedActivity;
      }
    }

    #endregion

    #region Remove Activity from Fragnet

    public ICommand RemoveActivityFromFragnetCommand { get; }

    private void RemoveActivityFromFragnetCommandExecute()
    {
      _ = FragnetActivities.Remove(CurrentFragnetActivity);
    }

    private bool RemoveActivityFromFragnetCommandCanExecute => CurrentFragnetActivity != null;

    #endregion

    #region IValidatable Implementation

    protected override ValidationResult OnValidating()
    {
      var result = ValidationResult.OK();

      if (string.IsNullOrWhiteSpace(Number))
      {
        result = result.Merge(ValidationResult.Error(NASResources.PleaseEnterNumber));
      }

      if (string.IsNullOrWhiteSpace(Name))
      {
        result = result.Merge(ValidationResult.Error(NASResources.PleaseEnterName));
      }

      return result;
    }

    #endregion

    #region Apply

    public void Apply()
    {
      if (Validate().IsOK)
      {
        _fragnet.Number = Number;
        _fragnet.Name = Name;
        _fragnet.RefreshActibities(FragnetActivities);
      }
    }

    #endregion
  }
}
