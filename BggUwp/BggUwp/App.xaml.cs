using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using BggUwp.Data;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Windows.UI.Popups;

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
            this.UnhandledException += App_UnhandledException;
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

        /// <summary>
        /// Catch unhandled exceptions thrown on the main UI thread and allow 
        /// option for user to continue program. 
        /// The OnDispatcherUnhandledException method below for AppDomain.UnhandledException will handle all other exceptions thrown by any thread.
        /// </summary>
        async void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.Exception == null)
            {
                Application.Current.Exit();
                return;
            }
            e.Handled = true;
            string errorMessage = string.Format("An application error occurred. \n\nError: {0}\n{1}", e.Message, e.Exception.Message);
            //insert code to log exception here
            var messageDialog = new MessageDialog(errorMessage);
            messageDialog.Title = "Error";

            await messageDialog.ShowAsync();
        }
    }
}

