using System.Windows.Controls;

namespace NAS.View.Controls
{
  internal class TaggableDataGridComboBoxColumn : DataGridComboBoxColumn, ITaggable
  {
    public object Tag { get; set; }
  }
}
