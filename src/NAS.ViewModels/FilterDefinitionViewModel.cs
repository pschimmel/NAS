using System.ComponentModel;
using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.ViewModels.Base;

namespace NAS.ViewModels
{
  public class FilterDefinitionViewModel : ViewModelBase
  {
    private readonly Schedule _schedule;

    public FilterDefinitionViewModel(Schedule schedule, FilterDefinition filterDefinition)
    {
      _schedule = schedule;
      FilterDefinition = filterDefinition;
      FilterDefinition.PropertyChanged += FilterDefinition_PropertyChanged;
    }

    private void FilterDefinition_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      OnPropertyChanged(nameof(Name));
    }

    public FilterDefinition FilterDefinition { get; }

    public string Name
    {
      get
      {
        string s = FilterDefinition.ObjectString;
        if (Guid.TryParse(FilterDefinition.ObjectString, out var id))
        {
          switch (FilterDefinition.Property)
          {
            case ActivityProperty.Fragnet:
              if (_schedule.Fragnets.Any(x => x.ID == id))
              {
                s = _schedule.Fragnets.First(x => x.ID == id).Name;
              }

              break;
            case ActivityProperty.WBSItem:
              var item = FindWBSItem(_schedule.WBSItem, id);
              if (item != null)
              {
                s = item.FullName;
              }

              break;
            case ActivityProperty.CustomAttribute1:
              if (_schedule.CustomAttributes1.Any(x => x.ID == id))
              {
                s = _schedule.CustomAttributes1.First(x => x.ID == id).Name;
              }

              break;
            case ActivityProperty.CustomAttribute2:
              if (_schedule.CustomAttributes2.Any(x => x.ID == id))
              {
                s = _schedule.CustomAttributes2.First(x => x.ID == id).Name;
              }

              break;
            case ActivityProperty.CustomAttribute3:
              if (_schedule.CustomAttributes3.Any(x => x.ID == id))
              {
                s = _schedule.CustomAttributes3.First(x => x.ID == id).Name;
              }
              break;
          }
        }
        return ActivityPropertyHelper.GetNameOfActivityProperty(FilterDefinition.Property) + " " + FilterRelationHelper.GetNameOfFilterRelation(FilterDefinition.Relation) + " " + s;
      }
    }

    private static WBSItem FindWBSItem(WBSItem parent, Guid id)
    {
      if (parent == null)
      {
        return null;
      }

      if (parent.ID == id)
      {
        return parent;
      }

      foreach (var child in parent.Children)
      {
        var foundItem = FindWBSItem(child, id);
        if (foundItem != null)
        {
          return foundItem;
        }
      }
      return null;
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (disposing)
        FilterDefinition.PropertyChanged -= FilterDefinition_PropertyChanged;
    }
  }
}
