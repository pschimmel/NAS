using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class EditWBSItemViewModel : DialogContentViewModel
  {
    #region Fields

    private readonly WBSItem _item;

    #endregion

    #region Constructor

    public EditWBSItemViewModel(WBSItemViewModel item)
      : base()
    {
      _item = item.Item;
      Number = item.Item.Number;
      Name = item.Item.Name;
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.WBS;

    public override string Icon => "WBS";

    public override DialogSize DialogSize => DialogSize.Fixed(400, 150);

    public override HelpTopic HelpTopicKey => HelpTopic.WBS;

    #endregion

    #region Properties

    public string Number { get; set; }

    public string Name { get; set; }

    #endregion

    #region Apply

    protected override void OnApply()
    {
      base.OnApply();
      _item.Number = Number;
      _item.Name = Name;
    }

    #endregion
  }
}
