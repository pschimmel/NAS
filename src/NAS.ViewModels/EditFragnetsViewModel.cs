using System.Collections.ObjectModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class EditFragnetsViewModel : DialogContentViewModel
  {
    #region Fields

    private readonly Schedule _schedule;
    private readonly Dictionary<Fragnet, List<Activity>> _activitiesOfFragnets;
    private Fragnet _selectedFragnet;
    private ActionCommand _addFragnetCommand;
    private ActionCommand _removeFragnetCommand;
    private ActionCommand _editFragnetCommand;
    private ActionCommand _showFragnetCommand;

    #endregion

    #region Constructor

    public EditFragnetsViewModel(Schedule schedule)
      : base()
    {
      _schedule = schedule;
      _activitiesOfFragnets = new Dictionary<Fragnet, List<Activity>>();
      Fragnets = new ObservableCollection<Fragnet>(_schedule.Fragnets.Select(x => x.Clone()));
      foreach (var fragnet in Fragnets)
      {
        _activitiesOfFragnets.Add(fragnet, new List<Activity>(_schedule.Activities.Where(x => x.Fragnet == fragnet)));
      }
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.Fragnets;

    public override string Icon => "EditFragnets";

    public override DialogSize DialogSize => DialogSize.Fixed(500, 300);

    public override HelpTopic HelpTopicKey => HelpTopic.Fragnets;

    #endregion

    #region Public Members

    public ObservableCollection<Fragnet> Fragnets { get; }

    public Fragnet SelectedFragnet
    {
      get => _selectedFragnet;
      set
      {
        if (_selectedFragnet != value)
        {
          _selectedFragnet = value;
          OnPropertyChanged(nameof(SelectedFragnet));
        }
      }
    }

    #endregion

    #region Add Fragnet

    public ICommand AddFragnetCommand => _addFragnetCommand ??= new ActionCommand(AddFragnet);

    private void AddFragnet()
    {
      var newFragnet = new Fragnet
      {
        IsVisible = true
      };

      using var vm = new EditFragnetViewModel(_schedule, newFragnet);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        Fragnets.Add(newFragnet);
        SelectedFragnet = newFragnet;
        _activitiesOfFragnets.Add(newFragnet, new List<Activity>(vm.FragnetActivities));
      }
    }

    #endregion

    #region Remove Fragnet

    public ICommand RemoveFragnetCommand => _removeFragnetCommand ??= new ActionCommand(RemoveFragnet, CanRemoveFragnet);

    private void RemoveFragnet()
    {
      UserNotificationService.Instance.Question(NASResources.MessageDeleteFragnet, () =>
      {
        Fragnets.Remove(SelectedFragnet);
        _activitiesOfFragnets.Remove(SelectedFragnet);
        SelectedFragnet = null;
      });
    }

    private bool CanRemoveFragnet()
    {
      return SelectedFragnet != null;
    }

    #endregion

    #region Edit Fragnet

    public ICommand EditFragnetCommand => _editFragnetCommand ??= new ActionCommand(EditFragnet, CanEditFragnet);

    private void EditFragnet()
    {
      using var vm = new EditFragnetViewModel(_schedule, SelectedFragnet);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        _activitiesOfFragnets[SelectedFragnet] = new List<Activity>(vm.FragnetActivities);
      }
    }

    private bool CanEditFragnet()
    {
      return SelectedFragnet != null;
    }

    #endregion

    #region Show Fragnet

    public ICommand ShowFragnetCommand => _showFragnetCommand ??= new ActionCommand(ShowFragnet, CanShowFragnet);

    private void ShowFragnet()
    {
      using var vm = new ShowFragnetViewModel(_schedule, SelectedFragnet);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CanShowFragnet()
    {
      return SelectedFragnet != null;
    }

    #endregion

    #region Apply

    protected override void OnApply()
    {
      _schedule.Fragnets.Clear();

      foreach (var activity in _schedule.Activities)
      {
        activity.Fragnet = null;
      }

      foreach (var fragnet in Fragnets)
      {
        _schedule.Fragnets.Add(fragnet);
        var activities = _activitiesOfFragnets[fragnet];
        foreach (var activity in activities)
        {
          activity.Fragnet = fragnet;
        }
      }
    }

    #endregion
  }
}
