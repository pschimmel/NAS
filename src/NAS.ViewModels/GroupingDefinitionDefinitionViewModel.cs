using System.ComponentModel;
using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.ViewModels.Base;

namespace NAS.ViewModels
{
  public class GroupingDefinitionViewModel : ViewModelBase
  {
    public GroupingDefinitionViewModel(GroupingDefinition groupingDefinition)
    {
      GroupingDefinition = groupingDefinition;
      GroupingDefinition.PropertyChanged += GroupingDefinition_PropertyChanged;
    }

    private void GroupingDefinition_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      OnPropertyChanged(nameof(Name));
    }

    public GroupingDefinition GroupingDefinition { get; }

    public string Name => ActivityPropertyHelper.GetNameOfActivityProperty(GroupingDefinition.Property);

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (disposing)
        GroupingDefinition.PropertyChanged -= GroupingDefinition_PropertyChanged;
    }
  }
}
