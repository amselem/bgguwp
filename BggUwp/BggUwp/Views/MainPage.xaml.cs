using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BggApi;
using BggUwp.ViewModels;
using BggUwp.Data;

namespace BggUwp.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        private void Menu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((Pivot)sender).SelectedIndex)
            {
                case 0:
                    HideAllAppBarButtons();
                    FindAppBarButton.Visibility = Visibility.Visible;
                    break;

                case 1:
                    HideAllAppBarButtons();
                    SortAppBarButton.Visibility = Visibility.Visible;
                    FilterAppBarButton.Visibility = Visibility.Visible;
                    break;
                case 2:
                    HideAllAppBarButtons();
                    PlayersAppBarButton.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void HideAllAppBarButtons()
        {
            FindAppBarButton.Visibility = Visibility.Collapsed;
            PlayersAppBarButton.Visibility = Visibility.Collapsed;
            FilterAppBarButton.Visibility = Visibility.Collapsed;
            SortAppBarButton.Visibility = Visibility.Collapsed;
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var context = DataContext as MainPageViewModel;
            var s = sender as MenuFlyoutItem;
            context.CollectionVM.CurrentCollectionSorter = s.DataContext as BoardgameSorter;
            SortFlyout.Hide();
        }

        private void RefreshBarButton_Click(object sender, RoutedEventArgs e)
        {
            var context = DataContext as MainPageViewModel;
            var flag = SettingsService.Instance.UpdateDataOnlyOnWiFi;
            SettingsService.Instance.UpdateDataOnlyOnWiFi = false;
            switch (Menu.SelectedIndex)
            {
                case 0:
                    context.HotItemsVM.LoadHotItemsList();
                    break;

                case 1:
                    context.CollectionVM.LoadCollection();
                    break;
                case 2:
                    context.PlaysVM.LoadPlays();
                    break;
            }
            SettingsService.Instance.UpdateDataOnlyOnWiFi = flag;
        }
    }
}
