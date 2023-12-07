using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using ES.Tools.Core.MVVM;
using NAS.ViewModel.Base;

namespace NAS.View
{
  /// <summary>
  /// Interaction logic for DialogWindow.xaml
  /// </summary>
  public partial class DialogWindow : IView
  {
    private const string imagePackURI = "pack://application:,,,/NAS.View;component/Images/";

    public DialogWindow()
    {
      InitializeComponent();
    }

    public DialogWindow(UIElement child)
      : this()
    {
      InnerControlBorder.Child = child;
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
          vm.CloseDialog += VM_CloseDialog;
        }
      }
    }

    private void VM_CloseDialog(object sender, EventArgs e)
    {
      DialogResult = true;
    }
  }
}
