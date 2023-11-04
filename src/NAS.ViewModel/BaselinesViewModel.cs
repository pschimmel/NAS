using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using Microsoft.Win32;
using NAS.Model;
using NAS.Model.Entities;
using NAS.Model.ImportExport;
using NAS.Resources;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel
{
  public class BaselinesViewModel : ViewModelBase
  {
    #region Fields

    private readonly Schedule _schedule;
    private Schedule _currentBaseline;

    #endregion

    #region Constructor

    public BaselinesViewModel(Schedule schedule)
      : base()
    {
      _schedule = schedule;
      Baselines = new ObservableCollection<Schedule>(schedule.Baselines);
      AddBaselineCommand = new ActionCommand(AddBaselineCommandExecute);
      ImportBaselineCommand = new ActionCommand(ImportBaselineCommandExecute);
      ExportBaselineCommand = new ActionCommand(ExportBaselineCommandExecute, () => ExportBaselineCommandCanExecute);
      RemoveBaselineCommand = new ActionCommand(RemoveBaselineCommandExecute, () => RemoveBaselineCommandCanExecute);
      EditBaselineCommand = new ActionCommand(EditBaselineCommandExecute, () => EditBaselineCommandCanExecute);
    }

    #endregion

    #region Public Members

    public override HelpTopic HelpTopicKey => HelpTopic.Baseline;

    public ObservableCollection<Schedule> Baselines { get; }

    public Schedule CurrentBaseline
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

    public ICommand AddBaselineCommand { get; }

    private void AddBaselineCommandExecute()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Baseline);

      UserNotificationService.Instance.Question(NASResources.MessageCreateBaseline, () =>
      {
        bool addToLayout = false;

        UserNotificationService.Instance.Question(NASResources.MessageShowBaselineInLayout, () =>
        {
          addToLayout = true;
        });

        var baseline = _schedule.AddBaseline(addToLayout);
        Baselines.Add(baseline);
        CurrentBaseline = baseline;
      });
    }

    #endregion

    #region Import Baseline

    public ICommand ImportBaselineCommand { get; }

    private void ImportBaselineCommandExecute()
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
      bool addToLayout = false;

      UserNotificationService.Instance.Question(NASResources.Question, () =>
      {
        addToLayout = true;
      });

      _schedule.AddBaseline(baseline, addToLayout);
      Baselines.Add(baseline);
      CurrentBaseline = baseline;
    }

    #endregion

    #region Export Baseline

    public ICommand ExportBaselineCommand { get; }

    private void ExportBaselineCommandExecute()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Baseline);
      var filter = new NASFilter();
      var saveFileDialog = new SaveFileDialog
      {
        DefaultExt = "." + filter.FileExtension,
        Filter = filter.FilterName + "|*." + filter.FileExtension,
        AddExtension = true,
        FileName = CurrentBaseline.Name
      };
      if (saveFileDialog.ShowDialog() == true)
      {
        filter.Export(CurrentBaseline, saveFileDialog.FileName);
      }
    }

    private bool ExportBaselineCommandCanExecute => CurrentBaseline != null;

    #endregion

    #region Remove Baseline

    public ICommand RemoveBaselineCommand { get; }

    private void RemoveBaselineCommandExecute()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Baseline);
      UserNotificationService.Instance.Question(NASResources.MessageDeleteBaseline, () =>
      {
        _schedule.RemoveBaseline(CurrentBaseline);
      });
    }

    private bool RemoveBaselineCommandCanExecute => CurrentBaseline != null;

    #endregion

    #region Edit Baseline

    public ICommand EditBaselineCommand { get; }

    private void EditBaselineCommandExecute()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Baseline);
      var vm = new GetTextViewModel(NASResources.Baseline, NASResources.Name, CurrentBaseline.Name);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        CurrentBaseline.Name = vm.Text;
      }
    }

    private bool EditBaselineCommandCanExecute => CurrentBaseline != null;

    #endregion
  }
}
