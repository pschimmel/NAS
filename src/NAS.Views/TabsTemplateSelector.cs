using System.Windows;
using System.Windows.Controls;
using AvalonDock.Layout;
using NAS.Models.Enums;
using NAS.ViewModels;

namespace NAS.Views
{
  public class TabsTemplateSelector : DataTemplateSelector
  {
    public DataTemplate GanttTemplate { get; set; }

    public DataTemplate PertTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      if (item is Grid)
      {
        return null;
      }

#pragma warning disable IDE0046 // Convert to conditional expression

      var itemAsLayoutContent = item as LayoutContent; // ContentPresenter
      var vm = (item as ScheduleViewModel) ?? itemAsLayoutContent?.Content as ScheduleViewModel;

      if (vm != null && vm.ActiveLayout != null)
      {
        return vm.ActiveLayout.LayoutType == LayoutType.Gantt ? GanttTemplate : PertTemplate;
      }

      return base.SelectTemplate(item, container);

#pragma warning restore IDE0046 // Convert to conditional expression
    }
  }
}
