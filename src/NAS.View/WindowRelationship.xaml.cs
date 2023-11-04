﻿using System.Windows;
using ES.Tools.Core.MVVM;

namespace NAS
{
  /// <summary>
  /// Interaction logic for WindowRelationship.xaml
  /// </summary>
  public partial class WindowRelationship : IView
  {
    public WindowRelationship()
    {
      InitializeComponent();
    }

    public IViewModel ViewModel
    {
      get => DataContext as IViewModel;
      set => DataContext = value;
    }

    private void buttonOK_Click(object sender, RoutedEventArgs e)
    {
      if ((DataContext as IValidatingViewModel).Validate())
      {
        DialogResult = true;
      }
    }

    private void buttonCancel_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
    }
  }
}
