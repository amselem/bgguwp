using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BggUwp.Controls
{
    public sealed partial class LogPlayDialog : ContentDialog
    {
        public ViewModels.LogPlayViewModel ViewModel
        {
            get
            {
                return (ViewModels.LogPlayViewModel)DataContext;
            }
        }

        public LogPlayDialog()
        {
            DataContext = new ViewModels.LogPlayViewModel();
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            LogPlayDialogCurrent.Hide();
        }
    }
}
