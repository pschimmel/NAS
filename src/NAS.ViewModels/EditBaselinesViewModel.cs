using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using Microsoft.Win32;
using NAS.Models;
using NAS.Models.Entities;
using NAS.Models.ImportExport;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class EditBaselinesViewModel : DialogContentViewModel
  {
    #region Fields

    private readonly Schedule _schedule;
    private ScheduleViewModel _currentBaseline;
    private readonly List<ScheduleViewModel> _baselinesToAddToLayout;
    private ActionCommand _addBaselineCommand;
    private ActionCommand _removeBaselineCommand;
    private ActionCommand _editBaselineCommand;
    private ActionCommand _importBaselineCommand;
    private ActionCommand _exportBaselineCommand;

    #endregion

    #region Constructor

    public EditBaselinesViewModel(Schedule schedule)
      : base()
    {
      _schedule = schedule;
      Baselines = new ObservableCollection<ScheduleViewModel>(schedule.Baselines.ToList().Select(x => new ScheduleViewModel(x)));
      _baselinesToAddToLayout = new List<ScheduleViewModel>();
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.EditBaselines;

    public override string Icon => "EditBaselines";

    public override DialogSize DialogSize => DialogSize.Fixed(400, 350);

    public override HelpTopic HelpTopicKey => HelpTopic.Baseline;

    #endregion

    #region Public Propeties

    public ObservableCollection<ScheduleViewModel> Baselines { get; }

    public ScheduleViewModel CurrentBaseline
    {
      get => _currentBaseline;
      set
      {
        if (_currentBaseline != value)
        {
          _currentBaseline = value;
          OnPropertyChanged(nameof(CurrentBaseline));
        }
      }
    }

    #endregion

    #region Add Baseline

    public ICommand AddBaselineCommand => _addBaselineCommand ??= new ActionCommand(AddBaseline);

    private void AddBaseline()
    {
      UserNotificationService.Instance.Question(NASResources.MessageCreateBaseline, () =>
      {
        var baseline = _schedule.Clone();
        var vm = new ScheduleViewModel(baseline);

        UserNotificationService.Instance.Question(NASResources.MessageShowBaselineInLayout, () =>
        {
          _baselinesToAddToLayout.Add(vm);
        });

        Baselines.Add(vm);
        CurrentBaseline = vm;
      });
    }

    #endregion

    #region Remove Baseline

    public ICommand RemoveBaselineCommand => _removeBaselineCommand ??= new ActionCommand(RemoveBaseline, CanRemoveBaseline);

    private void RemoveBaseline()
    {
      UserNotificationService.Instance.Question(NASResources.MessageDeleteBaseline, () =>
      {
        Baselines.Remove(CurrentBaseline);
        _baselinesToAddToLayout.Remove(CurrentBaseline);
      });
    }

    private bool CanRemoveBaseline()
    {
      return CurrentBaseline != null;
    }

    #endregion

    #region Edit Baseline

    public ICommand EditBaselineCommand => _editBaselineCommand ??= new ActionCommand(EditBaseline, CanEditBaseline);

    private void EditBaseline()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Baseline);
      var vm = new GetTextViewModel(NASResources.Baseline, NASResources.Name, CurrentBaseline.Schedule.Name);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        CurrentBaseline.Schedule.Name = vm.Text;
      }
    }

    private bool CanEditBaseline()
    {
      return CurrentBaseline != null;
    }

    #endregion

    #region Import Baseline

    public ICommand ImportBaselineCommand => _importBaselineCommand ??= new ActionCommand(ImportBaseline);

    private void ImportBaseline()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Baseline);
      string fileName;
      var filter = new NASFilter();
      var openFileDialog = new OpenFileDialog
      {
        DefaultExt = "." + filter.FileExtension,
        Filter = filter.FilterName + "|*." + filter.FileExtension,
        AddExtension = true,
        CheckFileExists = true
      };
      if (openFileDialog.ShowDialog() == true)
      {
        fileName = openFileDialog.FileName;
      }
      else
      {
        return;
      }

      if (!File.Exists(fileName))
      {
        UserNotificationService.Instance.Error(string.Format(NASResources.MessageFileNotFound, fileName));
        return;
      }
      var baseline = filter.Import(fileName);
      baseline.CreatedDate = DateTime.Now;
      baseline.CreatedBy = Globals.UserName;
      var vm = new ScheduleViewModel(baseline);

      UserNotificationService.Instance.Question(NASResources.Question, () =>
      {
        _baselinesToAddToLayout.Add(vm);
      });

      Baselines.Add(vm);
      CurrentBaseline = vm;
    }

    #endregion

    #region Export Baseline

    public ICommand ExportBaselineCommand => _exportBaselineCommand ??= new ActionCommand(ExportBaseline, CanExportBaseline);

    private void ExportBaseline()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Baseline);
      var filter = new NASFilter();
      var saveFileDialog = new SaveFileDialog
      {
        DefaultExt = "." + filter.FileExtension,
        Filter = filter.FilterName + "|*." + filter.FileExtension,
        AddExtension = true,
        FileName = CurrentBaseline.Schedule.Name
      };
      if (saveFileDialog.ShowDialog() == true)
      {
        filter.Export(CurrentBaseline.Schedule, saveFileDialog.FileName);
      }
    }

    private bool CanExportBaseline()
    {
      return CurrentBaseline != null;
    }

    #endregion

    #region Apply

    protected override void OnApply()
    {
      _schedule.Baselines.Clear();
      foreach (var baseline in Baselines)
      {
        _schedule.Baselines.Add(baseline.Schedule);
        if (_baselinesToAddToLayout.Contains(baseline))
        {
          _schedule.VisibleBaselines.Add(new VisibleBaseline(baseline.Schedule));
        }
      }
    }

    #endregion
  }
}
