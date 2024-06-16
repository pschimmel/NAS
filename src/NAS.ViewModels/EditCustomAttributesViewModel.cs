using System.Collections.ObjectModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class EditCustomAttributesViewModel : DialogContentViewModel
  {
    #region Fields

    private readonly Schedule _schedule;
    private CustomAttribute _currentCustomAttribute1;
    private CustomAttribute _currentCustomAttribute2;
    private CustomAttribute _currentCustomAttribute3;
    private ActionCommand _addCustomAttribute1Command;
    private ActionCommand _removeCustomAttribute1Command;
    private ActionCommand _editCustomAttribute1Command;
    private ActionCommand _addCustomAttribute2Command;
    private ActionCommand _removeCustomAttribute2Command;
    private ActionCommand _editCustomAttribute2Command;
    private ActionCommand _addCustomAttribute3Command;
    private ActionCommand _removeCustomAttribute3Command;
    private ActionCommand _editCustomAttribute3Command;

    #endregion

    #region Constructor

    public EditCustomAttributesViewModel(Schedule schedule)
      : base()
    {
      _schedule = schedule;
      CustomAttribute1Header = schedule.CustomAttribute1Header;
      CustomAttribute2Header = schedule.CustomAttribute2Header;
      CustomAttribute3Header = schedule.CustomAttribute3Header;
      CustomAttributes1 = new ObservableCollection<CustomAttribute>(schedule.CustomAttributes1);
      CustomAttributes2 = new ObservableCollection<CustomAttribute>(schedule.CustomAttributes2);
      CustomAttributes3 = new ObservableCollection<CustomAttribute>(schedule.CustomAttributes3);
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.EditAttributes;

    public override DialogSize DialogSize => DialogSize.Fixed(300, 300);

    public override HelpTopic HelpTopicKey => HelpTopic.CustomAttributes;

    #endregion

    #region Properties

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

    public ICommand AddCustomAttribute1Command => _addCustomAttribute1Command ??= new ActionCommand(AddCustomAttribute1);

    private void AddCustomAttribute1()
    {
      using var vm = new GetTextViewModel(NASResources.AddAttribute, NASResources.Name);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        CustomAttributes1.Add(new CustomAttribute { Name = vm.Text });
      }
    }

    #endregion

    #region Remove Custom Attribute 1

    public ICommand RemoveCustomAttribute1Command => _removeCustomAttribute1Command ??= new ActionCommand(RemoveCustomAttribute1, CanRemoveCustomAttribute1);

    private void RemoveCustomAttribute1()
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

    private bool CanRemoveCustomAttribute1()
    {
      return CurrentCustomAttribute1 != null;
    }

    #endregion

    #region Edit Custom Attribute 1

    public ICommand EditCustomAttribute1Command => _editCustomAttribute1Command ??= new ActionCommand(EditCustomAttribute1, CanEditCustomAttribute1);

    private void EditCustomAttribute1()
    {
      var vm = new GetTextViewModel(NASResources.EditAttribute, NASResources.Name, CurrentCustomAttribute1.Name);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        CurrentCustomAttribute1.Name = vm.Text;
      }
    }

    private bool CanEditCustomAttribute1()
    {
      return CurrentCustomAttribute1 != null;
    }

    #endregion

    #region Add Custom Attribute 2

    public ICommand AddCustomAttribute2Command => _addCustomAttribute2Command ??= new ActionCommand(AddCustomAttribute2);

    private void AddCustomAttribute2()
    {
      using var vm = new GetTextViewModel(NASResources.AddAttribute, NASResources.Name);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        CustomAttributes2.Add(new CustomAttribute { Name = vm.Text });
      }
    }

    #endregion

    #region Remove Custom Attribute 2

    public ICommand RemoveCustomAttribute2Command => _removeCustomAttribute2Command ??= new ActionCommand(RemoveCustomAttribute2, CanRemoveCustomAttribute2);

    private void RemoveCustomAttribute2()
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

    private bool CanRemoveCustomAttribute2()
    {
      return CurrentCustomAttribute2 != null;
    }

    #endregion

    #region Edit Custom Attribute 2

    public ICommand EditCustomAttribute2Command => _editCustomAttribute2Command ??= new ActionCommand(EditCustomAttribute2, CanEditCustomAttribute2);

    private void EditCustomAttribute2()
    {
      var vm = new GetTextViewModel(NASResources.EditAttribute, NASResources.Name, CurrentCustomAttribute2.Name);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        CurrentCustomAttribute2.Name = vm.Text;
      }
    }

    private bool CanEditCustomAttribute2()
    {
      return CurrentCustomAttribute2 != null;
    }

    #endregion

    #region Add Custom Attribute 3

    public ICommand AddCustomAttribute3Command => _addCustomAttribute3Command ??= new ActionCommand(AddCustomAttribute3);

    private void AddCustomAttribute3()
    {
      using var vm = new GetTextViewModel(NASResources.AddAttribute, NASResources.Name);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        CustomAttributes1.Add(new CustomAttribute { Name = vm.Text });
      }
    }

    #endregion

    #region Remove Custom Attribute 3

    public ICommand RemoveCustomAttribute3Command => _removeCustomAttribute3Command ??= new ActionCommand(RemoveCustomAttribute3, CanRemoveCustomAttribute3);

    private void RemoveCustomAttribute3()
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

    private bool CanRemoveCustomAttribute3()
    {
      return CurrentCustomAttribute3 != null;
    }

    #endregion

    #region Edit Custom Attribute 3

    public ICommand EditCustomAttribute3Command => _editCustomAttribute3Command ??= new ActionCommand(EditCustomAttribute3, CanEditCustomAttribute3);

    private void EditCustomAttribute3()
    {
      var vm = new GetTextViewModel(NASResources.EditAttribute, NASResources.Name, CurrentCustomAttribute3.Name);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        CurrentCustomAttribute3.Name = vm.Text;
      }
    }

    private bool CanEditCustomAttribute3()
    {
      return CurrentCustomAttribute3 != null;
    }

    #endregion

    #region Validation

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

    protected override void OnApply()
    {
      base.OnApply();
      _schedule.CustomAttribute1Header = CustomAttribute1Header;
      _schedule.CustomAttribute2Header = CustomAttribute2Header;
      _schedule.CustomAttribute3Header = CustomAttribute3Header;
      _schedule.CustomAttributes1.Clear();
      _schedule.CustomAttributes2.Clear();
      _schedule.CustomAttributes3.Clear();

      foreach (var customAttribute in CustomAttributes1)
      {
        _schedule.CustomAttributes1.Add(customAttribute);
      }

      foreach (var customAttribute in CustomAttributes2)
      {
        _schedule.CustomAttributes2.Add(customAttribute);
      }

      foreach (var customAttribute in CustomAttributes3)
      {
        _schedule.CustomAttributes3.Add(customAttribute);
      }
    }

    #endregion
  }
}
