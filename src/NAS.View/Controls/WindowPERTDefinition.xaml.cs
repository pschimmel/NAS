using System.Windows;
using System.Windows.Controls;
using ES.Tools.Core.MVVM;
using NAS.Model.Enums;
using NAS.Resources;
using NAS.ViewModel;
using NAS.ViewModel.Base;

namespace NAS.View.Controls
{
  /// <summary>
  /// Interaction logic for WindowPERTDefinition.xaml
  /// </summary>
  public partial class WindowPERTDefinition : IView
  {
    public WindowPERTDefinition()
    {
      InitializeComponent();
    }

    public IViewModel ViewModel
    {
      get => DataContext as IViewModel;
      set
      {
        DataContext = value;
        var viewModel = value as PERTDefinitionViewModel;
        viewModel.Definition.PropertyChanged += (sender, e) => { RefreshTemplate(); };
        viewModel.Definition.RowDefinitions.CollectionChanged += (sender, e) => { RefreshTemplate(); };
        viewModel.Definition.ColumnDefinitions.CollectionChanged += (sender, e) => { RefreshTemplate(); };
        viewModel.Definition.Items.CollectionChanged += (sender, e) => { RefreshTemplate(); };
        RefreshTemplate();
      }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void RefreshTemplate()
    {
      template.ColumnDefinitions.Clear();
      template.RowDefinitions.Clear();
      template.Children.Clear();
      foreach (var col in (DataContext as PERTDefinitionViewModel).Definition.ColumnDefinitions)
      {
        var definition = new ColumnDefinition();
        definition.Width = col.Width.HasValue ? new GridLength(col.Width.Value) : new GridLength(1, GridUnitType.Star);

        template.ColumnDefinitions.Add(definition);
      }
      foreach (var row in (DataContext as PERTDefinitionViewModel).Definition.RowDefinitions)
      {
        var definition = new RowDefinition();
        definition.Height = row.Height.HasValue ? new GridLength(row.Height.Value) : new GridLength(1, GridUnitType.Star);

        template.RowDefinitions.Add(definition);
      }
      foreach (var item in (DataContext as PERTDefinitionViewModel).Definition.Items)
      {
        var tb = new TextBlock();
        template.Children.Add(tb);
        tb.SetValue(Grid.RowProperty, item.Row);
        tb.SetValue(Grid.ColumnProperty, item.Column);
        tb.SetValue(Grid.RowSpanProperty, item.RowSpan);
        tb.SetValue(Grid.ColumnSpanProperty, item.ColumnSpan);
        tb.Text = ActivityPropertyHelper.GetNameOfActivityProperty(item.Property);
        tb.TextWrapping = TextWrapping.Wrap;
        tb.Margin = new Thickness(2);
        tb.TextAlignment = TextAlignment.Center;
        tb.TextTrimming = TextTrimming.CharacterEllipsis;
        tb.FontSize = (DataContext as PERTDefinitionViewModel).Definition.FontSize;
        switch (item.HorizontalAlignment)
        {
          case Model.Enums.HorizontalAlignment.Left:
            tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            break;
          case Model.Enums.HorizontalAlignment.Center:
            tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            break;
          case Model.Enums.HorizontalAlignment.Right:
            tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            break;
        }
        switch (item.VerticalAlignment)
        {
          case Model.Enums.VerticalAlignment.Top:
            tb.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            break;
          case Model.Enums.VerticalAlignment.Center:
            tb.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            break;
          case Model.Enums.VerticalAlignment.Bottom:
            tb.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            break;
        }
      }
    }

    private bool isManualEditCommit;

    private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
      if (!isManualEditCommit)
      {
        isManualEditCommit = true;
        var grid = (DataGrid)sender;
        grid.CommitEdit(DataGridEditingUnit.Row, true);
        isManualEditCommit = false;
      }
      RefreshTemplate();
    }

    private void ButtonOK_Click(object sender, RoutedEventArgs e)
    {
      if (DataContext is IValidatable validating)
      {
        var result = validating.Validate();
        if (!result.IsOK)
        {
          MessageBox.Show(NASResources.MessageCannotCloseWindow + Environment.NewLine + result.Message, NASResources.Stop, MessageBoxButton.OK, MessageBoxImage.Stop);
        }
      }
      else
      {
        DialogResult = true;
      }
    }

    private void ButtonCancel_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
    }
  }
}
