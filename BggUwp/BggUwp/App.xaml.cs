using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using BggUwp.Data;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace BggUwp
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    sealed partial class App : Template10.Common.BootStrapper
    {
        public App() {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
Microsoft.ApplicationInsights.WindowsCollectors.Session);
            InitializeComponent();
            StorageService.CreateDatabaseIfThereisNone();
        }

        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            await Task.CompletedTask;
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            NavigationService.Navigate(typeof(Views.MainPage));
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                statusBar.BackgroundColor = ((SolidColorBrush)App.Current.Resources["BGGHeaderBlue"]).Color;
                statusBar.BackgroundOpacity = 1;
                //statusBar.ProgressIndicator.Text = "BGG Uwp";
                //statusBar.ProgressIndicator.ShowAsync();
                //statusBar.ProgressIndicator.ProgressValue = 0;
            }
            await Task.CompletedTask;
        }
    }
}

