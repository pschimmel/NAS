using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using GongSolutions.Wpf.DragDrop;
using NAS.Model.Entities;
using NAS.Model.Enums;
using NAS.Resources;
using NAS.ViewModel.Base;

namespace NAS.ViewModel
{
  public class EditColumnsViewModel : DialogContentViewModel, IDropTarget
  {
    #region Fields

    private ColumnViewModel _selectedEditColumn;

    #endregion

    #region Constructor

    public EditColumnsViewModel(Layout layout)
      : base()
    {
      if (layout != null)
      {
        var visibleColumns = layout.ActivityColumns.OrderBy(x => x.Order).ToList();
        EditColumns = new ObservableCollection<ColumnViewModel>();
        foreach (var item in visibleColumns)
        {
          EditColumns.Add(new ColumnViewModel(item.Property) { IsVisible = true });
        }

        foreach (ActivityProperty item in Enum.GetValues(typeof(ActivityProperty)))
        {
          if (item != ActivityProperty.None && !layout.ActivityColumns.Any(x => x.Property == item))
          {
            EditColumns.Add(new ColumnViewModel(item));
          }
        }
      }

      MoveColumnUpCommand = new ActionCommand(MoveColumnUpCommandExecute, () => MoveColumnUpCommandCanExecute);
      MoveColumnDownCommand = new ActionCommand(MoveColumnDownCommandExecute, () => MoveColumnDownCommandCanExecute);
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.EditColumns;

    public override string Icon => "Table";

    public override DialogSize DialogSize => DialogSize.Fixed(400, 350);

    #endregion

    #region Public Properties

    public ObservableCollection<ColumnViewModel> EditColumns { get; }

    public ColumnViewModel SelectedEditColumn
    {
      get => _selectedEditColumn;
      set
      {
        if (_selectedEditColumn != value)
        {
          _selectedEditColumn = value;
          OnPropertyChanged(nameof(SelectedEditColumn));
        }
      }
    }

    #endregion

    #region Move Column Up

    public ICommand MoveColumnUpCommand { get; }

    private void MoveColumnUpCommandExecute()
    {
      var column = SelectedEditColumn;
      int idx = EditColumns.IndexOf(column);
      EditColumns.RemoveAt(idx);
      EditColumns.Insert(idx - 1, column);
      SelectedEditColumn = column;
    }

    private bool MoveColumnUpCommandCanExecute => SelectedEditColumn != null && EditColumns.IndexOf(SelectedEditColumn) > 0;

    #endregion

    #region Move Column Down

    public ICommand MoveColumnDownCommand { get; }

    private void MoveColumnDownCommandExecute()
    {
      var column = SelectedEditColumn;
      int idx = EditColumns.IndexOf(column);
      EditColumns.RemoveAt(idx);
      EditColumns.Insert(idx + 1, column);
      SelectedEditColumn = column;
    }

    private bool MoveColumnDownCommandCanExecute => SelectedEditColumn != null && EditColumns.IndexOf(SelectedEditColumn) < EditColumns.Count - 1;

    #endregion

    #region Drag and Drop

    public void DragOver(IDropInfo dropInfo)
    {
      if (dropInfo.Data is ColumnViewModel sourceItem
        && dropInfo.TargetItem is ColumnViewModel targetItem
        && sourceItem != targetItem)
      {
        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        dropInfo.Effects = DragDropEffects.Copy;
      }
    }

    public void Drop(IDropInfo dropInfo)
    {
      var sourceItem = dropInfo.Data as ColumnViewModel;
      var targetItem = dropInfo.TargetItem as ColumnViewModel;
      int idx = EditColumns.IndexOf(targetItem);
      _ = EditColumns.Remove(sourceItem);
      EditColumns.Insert(idx, sourceItem);
      SelectedEditColumn = sourceItem;
    }

    #endregion
  }
}
