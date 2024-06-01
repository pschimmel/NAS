using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using ES.Tools.Core.MVVM;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for DialogWindow.xaml
  /// </summary>
  public partial class DialogWindow : IView
  {
    private const string imagePackURI = "pack://application:,,,/NAS.Views;component/Images/";

    public DialogWindow()
    {
      InitializeComponent();
    }

    public IViewModel ViewModel
    {
      get => DataContext as DialogViewModel;
      set
      {
        Debug.Assert(value is DialogViewModel);

        if (value is DialogViewModel vm)
        {
          DataContext = vm;

          Debug.Assert(!string.IsNullOrWhiteSpace(vm.Title));
          Debug.Assert(!string.IsNullOrWhiteSpace(vm.Icon));
          Debug.Assert(vm.DialogSize != null);

          var converter = new ImageSourceConverter();
          Icon = (ImageSource)converter.ConvertFromString($"{imagePackURI}{vm.Icon}.png");

          if (vm.DialogSize != null)
          {
            if (vm.DialogSize.IsAuto)
            {
              SizeToContent = SizeToContent.WidthAndHeight;
            }
            else
            {
              SizeToContent = SizeToContent.Manual;

              if (vm.DialogSize.IsFixed)
              {
                Width = vm.DialogSize.Width;
                Height = vm.DialogSize.Height;
                ResizeMode = ResizeMode.NoResize;
              }
              else
              {
                Width = vm.DialogSize.Width;
                Height = vm.DialogSize.Height;
                ResizeMode = ResizeMode.CanResize;

                if (!double.IsNaN(vm.DialogSize.MinWidth))
                {
                  MinWidth = vm.DialogSize.MinWidth;
                }

                if (!double.IsNaN(vm.DialogSize.MinHeight))
                {
                  MinHeight = vm.DialogSize.MinHeight;
                }

                if (!double.IsNaN(vm.DialogSize.MaxWidth))
                {
                  MaxWidth = vm.DialogSize.MaxWidth;
                }

                if (!double.IsNaN(vm.DialogSize.MaxHeight))
                {
                  MaxHeight = vm.DialogSize.MaxHeight;
                }
              }
            }
          }

          vm.RequestOK += VM_CloseDialogOK;
          vm.RequestCancel += VM_CloseDialogCancel;

          var content = ViewFactory.Instance.CreateDialogContentView(vm.ContentViewModel);
          InnerControlBorder.Child = (UIElement)content;
        }
      }
    }

    private void VM_CloseDialogOK(object sender, EventArgs e)
    {
      DialogResult = true;
    }

    private void VM_CloseDialogCancel(object sender, EventArgs e)
    {
      DialogResult = false;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      base.OnClosing(e);
      if (ViewModel is DialogViewModel vm && !vm.ContentViewModel.OnClosing(DialogResult))
      {
        e.Cancel = true;
      }
    }
  }
}
