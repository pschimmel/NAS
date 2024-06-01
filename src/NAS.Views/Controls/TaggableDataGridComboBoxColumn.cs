using System.Windows.Controls;

namespace NAS.Views.Controls
{
  internal class TaggableDataGridComboBoxColumn : DataGridComboBoxColumn, ITaggable
  {
    public object Tag { get; set; }
  }
}
