using System.Collections.ObjectModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class EditDistortionsViewModel : DialogContentViewModel
  {
    #region Fields

    private readonly Schedule _schedule;
    private Distortion _selectedDistortion;
    private ActionCommand _addDistortionCommand;
    private ActionCommand _removeDistortionCommand;
    private ActionCommand _editDistortionCommand;

    #endregion

    #region Constructor

    public EditDistortionsViewModel(Schedule schedule, Activity activity)
      : base()
    {
      ArgumentNullException.ThrowIfNull(schedule);
      ArgumentNullException.ThrowIfNull(activity);
      _schedule = schedule;
      Activity = activity;
      Distortions = new ObservableCollection<Distortion>(activity.Distortions.Select(x => x.Clone()));
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.Distortions;

    public override string Icon => "Distortion";

    public override DialogSize DialogSize => DialogSize.Fixed(350, 350);

    public override HelpTopic HelpTopicKey => HelpTopic.Distortions;

    #endregion

    #region Properties

    public Activity Activity { get; }

    public ObservableCollection<Distortion> Distortions { get; }

    public Distortion SelectedDistortion
    {
      get => _selectedDistortion;
      set
      {
        if (_selectedDistortion != value)
        {
          _selectedDistortion = value;
          OnPropertyChanged(nameof(SelectedDistortion));
        }
      }
    }

    #endregion

    #region Add Distortion

    public ICommand AddDistortionCommand => _addDistortionCommand ??= new ActionCommand(AddDistortion, CanAddDistortion);

    private void AddDistortion()
    {
      using var vm = new AddDistortionViewModel();
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        Distortion newDistortion = vm.DistortionType switch
        {
          DistortionType.Delay => new Delay(),
          DistortionType.Extension => new Extension(),
          DistortionType.Inhibition => new Inhibition(),
          DistortionType.Interruption => new Interruption(),
          DistortionType.Reduction => new Reduction(),
          _ => throw new ApplicationException("Unknown DistortionType."),
        };

        if (newDistortion != null)
        {
          using var vm2 = new EditDistortionViewModel(_schedule, newDistortion);
          if (ViewFactory.Instance.ShowDialog(vm2) == true)
          {
            Distortions.Add(newDistortion);
            SelectedDistortion = newDistortion;
          }
        }
      }
    }

    private bool CanAddDistortion()
    {
      return Activity.ActivityType == ActivityType.Activity;
    }

    #endregion

    #region Remove Distortion

    public ICommand RemoveDistortionCommand => _removeDistortionCommand ??= new ActionCommand(RemoveDistortion, CanRemoveDistortion);

    private void RemoveDistortion()
    {
      UserNotificationService.Instance.Question(NASResources.MessageDeleteDistortion, () =>
      {
        Distortions.Remove(SelectedDistortion);
        SelectedDistortion = null;
      });
    }

    private bool CanRemoveDistortion()
    {
      return SelectedDistortion != null;
    }

    #endregion

    #region Edit Distortion

    public ICommand EditDistortionCommand => _editDistortionCommand ??= new ActionCommand(EditDistortion, CanEditDistortion);

    private void EditDistortion()
    {
      using var vm = new EditDistortionViewModel(_schedule, SelectedDistortion);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CanEditDistortion()
    {
      return SelectedDistortion != null;
    }

    #endregion

    #region Apply

    protected override void OnApply()
    {
      Activity.Distortions.Clear();
      foreach (var distortion in Distortions)
      {
        Activity.Distortions.Add(distortion);
      }
    }

    #endregion
  }
}
