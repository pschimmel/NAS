using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class ShowWBSSummaryViewModel : DialogContentViewModel
  {
    #region Constructors

    public ShowWBSSummaryViewModel(Schedule schedule)
    {
      var items = new List<WBSItem>();
      if (schedule.WBSItem != null)
      {
        items.Add(schedule.WBSItem);
        AddSubItems(schedule.WBSItem, items);

        var WBSSummaryItems = new List<WBSSummaryItem>();
        foreach (var item in items)
        {
          WBSSummaryItems.Add(new WBSSummaryItem(schedule.Activities.ToList(), item));
        }
      }
    }

    private static void AddSubItems(WBSItem parent, List<WBSItem> items)
    {
      foreach (var item in parent.Children)
      {
        items.Add(item);
        AddSubItems(item, items);
      }
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.WBS;

    public override string Icon => "WBS";

    public override DialogSize DialogSize => DialogSize.Fixed(550, 375);

    public override HelpTopic HelpTopicKey => HelpTopic.WBS;

    public override IEnumerable<IButtonViewModel> Buttons => new List<IButtonViewModel>
    {
      ButtonViewModel.CreateOKButton()
    };

    #endregion


    #region Properties

    public List<WBSSummaryItem> WBSSummaryItems { get; }

    #endregion
  }
}
