using System.Collections.ObjectModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class EditFragnetViewModel : DialogContentViewModel
  {
    #region Fields

    private readonly Schedule _schedule;
    private readonly Fragnet _fragnet;
    private Activity _selectedFragnetActivity;
    private ActionCommand _addActivityToFragnetCommand;
    private ActionCommand _removeActivityFromFragnetCommand;

    #endregion

    #region Constructor

    public EditFragnetViewModel(Schedule schedule, Fragnet fragnet)
      : base()
    {
      _schedule = schedule;
      _fragnet = fragnet;
      FragnetActivities = new ObservableCollection<Activity>(schedule.Activities.Where(x => x.Fragnet == fragnet));
      Number = fragnet.Number;
      IsVisible = fragnet.IsVisible;
      Name = fragnet.Name;
      Description = fragnet.Description;
      Identified = fragnet.Identified;
      Submitted = fragnet.Submitted;
      Approved = fragnet.Approved;
      IsDisputable = fragnet.IsDisputable;
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.Fragnet;

    public override string Icon => "EditFragnets";

    public override DialogSize DialogSize => DialogSize.Fixed(500, 300);

    public override HelpTopic HelpTopicKey => HelpTopic.Fragnets;

    #endregion

    #region Properties

    public string Number { get; set; }

    public bool IsVisible { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public DateTime Identified { get; set; }

    public DateTime? Submitted { get; set; }

    public DateTime? Approved { get; set; }

    public bool IsDisputable { get; set; }

    public ObservableCollection<Activity> FragnetActivities { get; }

    public Activity SelectedFragnetActivity
    {
      get => _selectedFragnetActivity;
      set
      {
        if (_selectedFragnetActivity != value)
        {
          _selectedFragnetActivity = value;
          OnPropertyChanged(nameof(SelectedFragnetActivity));
        }
      }
    }

    #endregion

    #region Add Activity to Fragnet

    public ICommand AddActivityToFragnetCommand => _addActivityToFragnetCommand ??= new ActionCommand(AddActivityToFragnet);

    private void AddActivityToFragnet()
    {
      using var vm = new SelectActivityViewModel(_schedule.Activities);
      if (ViewFactory.Instance.ShowDialog(vm) == true && vm.SelectedActivity != null)
      {
        if (vm.SelectedActivity.Fragnet == _fragnet)
        {
          UserNotificationService.Instance.Information(string.Format(NASResources.MessageActivityAlreadyAssignedToFragnet, vm.SelectedActivity, vm.SelectedActivity.Fragnet));
          return;
        }

        FragnetActivities.Add(vm.SelectedActivity);
        SelectedFragnetActivity = vm.SelectedActivity;
      }
    }

    #endregion

    #region Remove Activity from Fragnet

    public ICommand RemoveActivityFromFragnetCommand => _removeActivityFromFragnetCommand ??= new ActionCommand(RemoveActivityFromFragnet, CanRemoveActivityFromFragnet);

    private void RemoveActivityFromFragnet()
    {
      FragnetActivities.Remove(SelectedFragnetActivity);
    }

    private bool CanRemoveActivityFromFragnet()
    {
      return SelectedFragnetActivity != null;
    }

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

    protected override void OnApply()
    {
      _fragnet.Number = Number;
      _fragnet.IsVisible = IsVisible;
      _fragnet.Name = Name;
      _fragnet.Description = Description;
      _fragnet.Identified = Identified;
      _fragnet.Submitted = Submitted;
      _fragnet.Approved = Approved;
      _fragnet.IsDisputable = IsDisputable;
      foreach (var activity in _schedule.Activities)
      {
        if (!FragnetActivities.Contains(activity))
        {
          activity.Fragnet = null;
        }
      }
      foreach (var activity in FragnetActivities)
      {
        activity.Fragnet = _fragnet;
      }

    }

    #endregion
  }
}
