using System.ComponentModel;
using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.ViewModels.Base;

namespace NAS.ViewModels
{
  public class SortingDefinitionViewModel : ViewModelBase
  {
    public SortingDefinitionViewModel(SortingDefinition sortingDefinition)
    {
      SortingDefinition = sortingDefinition;
      SortingDefinition.PropertyChanged += SortingDefinition_PropertyChanged;
    }

    private void SortingDefinition_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      OnPropertyChanged(nameof(Name));
    }

    public SortingDefinition SortingDefinition { get; }

    public string Name => $"{ActivityPropertyHelper.GetNameOfActivityProperty(SortingDefinition.Property)} ({SortDirectionHelper.GetNameOfSortDirection(SortingDefinition.Direction)})";

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (disposing)
        SortingDefinition.PropertyChanged -= SortingDefinition_PropertyChanged;
    }
  }
}
