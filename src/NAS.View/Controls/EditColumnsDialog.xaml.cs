namespace NAS.View.Controls
{
  /// <summary>
  /// Interaction logic for EditColumnsDialog.xaml
  /// </summary>
  public partial class EditColumnsDialog : IDialog
  {
    public EditColumnsDialog()
    {
      InitializeComponent();
    }

    public IViewModel ViewModel
    {
      get => DataContext as IViewModel;
      set => DataContext = value;
    }
  }
}
