using System.Collections.ObjectModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class FragnetsViewModel : ViewModelBase
  {
    #region Fields

    public Schedule _schedule;
    private Fragnet _currentFragnet;

    #endregion

    #region Constructor

    public FragnetsViewModel(Schedule schedule)
      : base()
    {
      _schedule = schedule;
      Fragnets = new ObservableCollection<Fragnet>(_schedule.Fragnets);
      AddFragnetCommand = new ActionCommand(AddFragnetCommandExecute);
      RemoveFragnetCommand = new ActionCommand(RemoveFragnetCommandExecute, () => RemoveFragnetCommandCanExecute);
      EditFragnetCommand = new ActionCommand(EditFragnetCommandExecute, () => EditFragnetCommandCanExecute);
      ShowFragnetCommand = new ActionCommand(ShowFragnetCommandExecute, () => ShowFragnetCommandCanExecute);
    }

    #endregion

    #region Public Members

    public override HelpTopic HelpTopicKey => HelpTopic.Fragnets;

    public ObservableCollection<Fragnet> Fragnets { get; }

    public Fragnet CurrentFragnet
    {
      get => _currentFragnet;
      set
      {
        if (_currentFragnet != value)
        {
          _currentFragnet = value;
          OnPropertyChanged(nameof(CurrentFragnet));
        }
      }
    }

    #endregion

    #region Add Fragnet

    public ICommand AddFragnetCommand { get; }

    private void AddFragnetCommandExecute()
    {
      var newFragnet = new Fragnet();
      newFragnet.IsVisible = true;
      using var vm = new FragnetViewModel(_schedule, newFragnet);

      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        Fragnets.Add(newFragnet);
        _schedule.Fragnets.Add(newFragnet);
        CurrentFragnet = newFragnet;
      }
    }

    #endregion

    #region Remove Fragnet

    public ICommand RemoveFragnetCommand { get; }

    private void RemoveFragnetCommandExecute()
    {
      UserNotificationService.Instance.Question(NASResources.MessageDeleteFragnet, () =>
      {
        Fragnets.Remove(CurrentFragnet);
        _schedule.Fragnets.Remove(CurrentFragnet);

        foreach (var activity in _schedule.Activities)
        {
          if (!Fragnets.Contains(activity.Fragnet))
          {
            activity.Fragnet = null;
          }
        }

        CurrentFragnet = null;
      });
    }

    private bool RemoveFragnetCommandCanExecute => CurrentFragnet != null;

    #endregion

    #region Edit Fragnet

    public ICommand EditFragnetCommand { get; }

    private void EditFragnetCommandExecute()
    {
      using var vm = new FragnetViewModel(_schedule, CurrentFragnet);

      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        vm.Apply();
      }
    }

    private bool EditFragnetCommandCanExecute => CurrentFragnet != null;

    #endregion

    #region Show Fragnet

    public ICommand ShowFragnetCommand { get; }

    private void ShowFragnetCommandExecute()
    {
      using var vm = new ShowFragnetViewModel(_schedule, CurrentFragnet);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool ShowFragnetCommandCanExecute => CurrentFragnet != null;

    #endregion
  }
}
