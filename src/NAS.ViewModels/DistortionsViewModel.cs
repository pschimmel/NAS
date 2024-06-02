using System.Collections.ObjectModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Models.Base;
using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class DistortionsViewModel : ViewModelBase, IApplyable
  {
    #region Fields

    private readonly Activity _activity;
    private Distortion _currentDistortion;

    #endregion

    #region Constructor

    public DistortionsViewModel(Activity activity)
      : base()
    {
      _activity = activity;
      Distortions = new ObservableCollection<Distortion>(activity.Distortions);
      AddDistortionCommand = new ActionCommand(AddDistortionCommandExecute, () => AddDistortionCommandCanExecute);
      RemoveDistortionCommand = new ActionCommand(RemoveDistortionCommandExecute, () => RemoveDistortionCommandCanExecute);
      EditDistortionCommand = new ActionCommand(EditDistortionCommandExecute, () => EditDistortionCommandCanExecute);
    }

    #endregion

    #region Properties

    public override HelpTopic HelpTopicKey => HelpTopic.Distortions;

    public ObservableCollection<Distortion> Distortions { get; }

    public Distortion CurrentDistortion
    {
      get => _currentDistortion;
      set
      {
        if (_currentDistortion != value)
        {
          _currentDistortion = value;
          OnPropertyChanged(nameof(CurrentDistortion));
        }
      }
    }

    #endregion

    #region Add Distortion

    public ICommand AddDistortionCommand { get; }

    private void AddDistortionCommandExecute()
    {
      using var vm = new AddDistortionViewModel();
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        Distortion newDistortion = vm.DistortionType switch
        {
          DistortionType.Delay => new Delay(_activity),
          DistortionType.Extension => new Extension(_activity),
          DistortionType.Inhibition => new Inhibition(_activity),
          DistortionType.Interruption => new Interruption(_activity),
          DistortionType.Reduction => new Reduction(_activity),
          _ => throw new ApplicationException("Unknown DistortionType."),
        };

        if (newDistortion != null)
        {
          using var vm2 = new DistortionViewModel(newDistortion);
          if (ViewFactory.Instance.ShowDialog(vm2) == true)
          {
            Distortions.Add(newDistortion);
            CurrentDistortion = newDistortion;
          }
        }
      }
    }

    private bool AddDistortionCommandCanExecute => _activity.ActivityType == ActivityType.Activity;

    #endregion

    #region Remove Distortion

    public ICommand RemoveDistortionCommand { get; }

    private void RemoveDistortionCommandExecute()
    {
      UserNotificationService.Instance.Question(NASResources.MessageDeleteDistortion, () =>
      {
        Distortions.Remove(CurrentDistortion);
        CurrentDistortion = null;
      });
    }

    private bool RemoveDistortionCommandCanExecute => CurrentDistortion != null;

    #endregion

    #region Edit Distortion

    public ICommand EditDistortionCommand { get; }

    private void EditDistortionCommandExecute()
    {
      using var vm = new DistortionViewModel(CurrentDistortion);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool EditDistortionCommandCanExecute => CurrentDistortion != null;


    #endregion

    #region IApplyable Implementation

    public void Apply()
    {
      _activity.RefreshDistortions(Distortions);
    }

    #endregion
  }
}
