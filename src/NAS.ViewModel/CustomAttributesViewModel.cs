using System.Collections.ObjectModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Model.Entities;
using NAS.Resources;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel
{
  public class CustomAttributesViewModel : ValidatingViewModel, IApplyable
  {
    #region Fields

    private readonly Schedule _schedule;
    private CustomAttribute _currentCustomAttribute1;
    private CustomAttribute _currentCustomAttribute2;
    private CustomAttribute _currentCustomAttribute3;

    #endregion

    #region Constructor

    public CustomAttributesViewModel(Schedule schedule)
      : base()
    {
      _schedule = schedule;
      CustomAttribute1Header = schedule.CustomAttribute1Header;
      CustomAttribute2Header = schedule.CustomAttribute2Header;
      CustomAttribute3Header = schedule.CustomAttribute3Header;
      CustomAttributes1 = new ObservableCollection<CustomAttribute>(schedule.CustomAttributes1);
      CustomAttributes2 = new ObservableCollection<CustomAttribute>(schedule.CustomAttributes2);
      CustomAttributes3 = new ObservableCollection<CustomAttribute>(schedule.CustomAttributes3);
      AddCustomAttribute1Command = new ActionCommand(param => AddCustomAttribute1CommandExecute());
      RemoveCustomAttribute1Command = new ActionCommand(param => RemoveCustomAttribute1CommandExecute(), param => RemoveCustomAttribute1CommandCanExecute);
      EditCustomAttribute1Command = new ActionCommand(param => EditCustomAttribute1CommandExecute(), param => EditCustomAttribute1CommandCanExecute);
      AddCustomAttribute2Command = new ActionCommand(param => AddCustomAttribute2CommandExecute());
      RemoveCustomAttribute2Command = new ActionCommand(param => RemoveCustomAttribute2CommandExecute(), param => RemoveCustomAttribute2CommandCanExecute);
      EditCustomAttribute2Command = new ActionCommand(param => EditCustomAttribute2CommandExecute(), param => EditCustomAttribute2CommandCanExecute);
      AddCustomAttribute3Command = new ActionCommand(param => AddCustomAttribute3CommandExecute());
      RemoveCustomAttribute3Command = new ActionCommand(param => RemoveCustomAttribute3CommandExecute(), param => RemoveCustomAttribute3CommandCanExecute);
      EditCustomAttribute3Command = new ActionCommand(param => EditCustomAttribute3CommandExecute(), param => EditCustomAttribute3CommandCanExecute);
    }

    #endregion

    #region Properties

    public override HelpTopic HelpTopicKey => HelpTopic.CustomAttributes;

    public string CustomAttribute1Header { get; set; }

    public string CustomAttribute2Header { get; set; }

    public string CustomAttribute3Header { get; set; }

    public ObservableCollection<CustomAttribute> CustomAttributes1 { get; }

    public ObservableCollection<CustomAttribute> CustomAttributes2 { get; }

    public ObservableCollection<CustomAttribute> CustomAttributes3 { get; }

    public CustomAttribute CurrentCustomAttribute1
    {
      get => _currentCustomAttribute1;
      set
      {
        if (_currentCustomAttribute1 != value)
        {
          _currentCustomAttribute1 = value;
          OnPropertyChanged(nameof(CurrentCustomAttribute1));
        }
      }
    }

    public CustomAttribute CurrentCustomAttribute2
    {
      get => _currentCustomAttribute2;
      set
      {
        if (_currentCustomAttribute2 != value)
        {
          _currentCustomAttribute2 = value;
          OnPropertyChanged(nameof(CurrentCustomAttribute2));
        }
      }
    }

    public CustomAttribute CurrentCustomAttribute3
    {
      get => _currentCustomAttribute3;
      set
      {
        if (_currentCustomAttribute3 != value)
        {
          _currentCustomAttribute3 = value;
          OnPropertyChanged(nameof(CurrentCustomAttribute3));
        }
      }
    }

    #endregion

    #region Add Custom Attribute 1

    public ICommand AddCustomAttribute1Command { get; }

    private void AddCustomAttribute1CommandExecute()
    {
      using var vm = new GetTextViewModel(NASResources.AddAttribute, NASResources.Name);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        CustomAttributes1.Add(new CustomAttribute { Name = vm.Text });
      }
    }

    #endregion

    #region Remove Custom Attribute 1

    public ICommand RemoveCustomAttribute1Command { get; }

    private void RemoveCustomAttribute1CommandExecute()
    {
      if (!_schedule.CanRemoveCustomAttribute1(CurrentCustomAttribute1))
      {
        UserNotificationService.Instance.Error(NASResources.MessageCannotRemoveCustomAttribute);
        return;
      }

      UserNotificationService.Instance.Question(NASResources.MessageDeleteAttribute, () =>
      {
        CustomAttributes1.Remove(CurrentCustomAttribute1);
        CurrentCustomAttribute1 = null;
      });
    }

    private bool RemoveCustomAttribute1CommandCanExecute => CurrentCustomAttribute1 != null;

    #endregion

    #region Edit Custom Attribute 1

    public ICommand EditCustomAttribute1Command { get; }

    private void EditCustomAttribute1CommandExecute()
    {
      var vm = new GetTextViewModel(NASResources.EditAttribute, NASResources.Name, CurrentCustomAttribute1.Name);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        CurrentCustomAttribute1.Name = vm.Text;
      }
    }

    private bool EditCustomAttribute1CommandCanExecute => CurrentCustomAttribute1 != null;

    #endregion

    #region Add Custom Attribute 2

    public ICommand AddCustomAttribute2Command { get; }

    private void AddCustomAttribute2CommandExecute()
    {
      using var vm = new GetTextViewModel(NASResources.AddAttribute, NASResources.Name);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        CustomAttributes2.Add(new CustomAttribute { Name = vm.Text });
      }
    }

    #endregion

    #region Remove Custom Attribute 2

    public ICommand RemoveCustomAttribute2Command { get; }

    private void RemoveCustomAttribute2CommandExecute()
    {
      if (!_schedule.CanRemoveCustomAttribute2(CurrentCustomAttribute2))
      {
        UserNotificationService.Instance.Error(NASResources.MessageCannotRemoveCustomAttribute);
        return;
      }

      UserNotificationService.Instance.Question(NASResources.MessageDeleteAttribute, () =>
      {
        CustomAttributes2.Remove(CurrentCustomAttribute2);
        CurrentCustomAttribute2 = null;
      });
    }

    private bool RemoveCustomAttribute2CommandCanExecute => CurrentCustomAttribute2 != null;

    #endregion

    #region Edit Custom Attribute 2

    public ICommand EditCustomAttribute2Command { get; }

    private void EditCustomAttribute2CommandExecute()
    {
      var vm = new GetTextViewModel(NASResources.EditAttribute, NASResources.Name, CurrentCustomAttribute2.Name);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        CurrentCustomAttribute2.Name = vm.Text;
      }
    }

    private bool EditCustomAttribute2CommandCanExecute => CurrentCustomAttribute2 != null;

    #endregion

    #region Add Custom Attribute 3

    public ICommand AddCustomAttribute3Command { get; }

    private void AddCustomAttribute3CommandExecute()
    {
      using var vm = new GetTextViewModel(NASResources.AddAttribute, NASResources.Name);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        CustomAttributes1.Add(new CustomAttribute { Name = vm.Text });
      }
    }

    #endregion

    #region Remove Custom Attribute 3

    public ICommand RemoveCustomAttribute3Command { get; }

    private void RemoveCustomAttribute3CommandExecute()
    {
      if (!_schedule.CanRemoveCustomAttribute1(CurrentCustomAttribute3))
      {
        UserNotificationService.Instance.Error(NASResources.MessageCannotRemoveCustomAttribute);
        return;
      }

      UserNotificationService.Instance.Question(NASResources.MessageDeleteAttribute, () =>
      {
        CustomAttributes1.Remove(CurrentCustomAttribute3);
        CurrentCustomAttribute3 = null;
      });
    }

    private bool RemoveCustomAttribute3CommandCanExecute => CurrentCustomAttribute3 != null;

    #endregion

    #region Edit Custom Attribute 3

    public ICommand EditCustomAttribute3Command { get; }

    private void EditCustomAttribute3CommandExecute()
    {
      var vm = new GetTextViewModel(NASResources.EditAttribute, NASResources.Name, CurrentCustomAttribute3.Name);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        CurrentCustomAttribute3.Name = vm.Text;
      }
    }

    private bool EditCustomAttribute3CommandCanExecute => CurrentCustomAttribute3 != null;

    #endregion

    #region IValidatable Implementation

    protected override ValidationResult OnValidating()
    {
      return string.IsNullOrWhiteSpace(CustomAttribute1Header)
             ? ValidationResult.Error(NASResources.MessageCustomAttributeHeaderCantBeEmpty)
             : string.IsNullOrWhiteSpace(CustomAttribute2Header)
             ? ValidationResult.Error(NASResources.MessageCustomAttributeHeaderCantBeEmpty)
             : string.IsNullOrWhiteSpace(CustomAttribute3Header)
             ? ValidationResult.Error(NASResources.MessageCustomAttributeHeaderCantBeEmpty)
             : ValidationResult.OK();
    }

    #endregion

    #region Apply

    public void Apply()
    {
      if (Validate().IsOK)
      {
        _schedule.CustomAttribute1Header = CustomAttribute1Header;
        _schedule.CustomAttribute2Header = CustomAttribute2Header;
        _schedule.CustomAttribute3Header = CustomAttribute3Header;
        _schedule.RefreshCustomAttributes(CustomAttributes1, CustomAttributes2, CustomAttributes3);
      }
    }

    #endregion
  }
}
