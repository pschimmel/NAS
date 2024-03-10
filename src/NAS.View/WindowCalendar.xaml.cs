﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.ViewModel.Base;

namespace NAS
{
  /// <summary>
  /// Interaction logic for WindowEditCalendarDetails.xaml
  /// </summary>
  public partial class WindowCalendar : IView
  {
    public WindowCalendar()
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
      if ((ViewModel as IValidating).Validate().IsOK)
      {
        DialogResult = true;
      }
    }

    private void buttonCancel_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
    }

    private void ComboBox_KeyDown(object sender, KeyEventArgs e)
    {
      if (sender is ComboBox && e.Key == Key.Delete)
      {
        (sender as ComboBox).SelectedItem = null;
      }
    }
  }
}
