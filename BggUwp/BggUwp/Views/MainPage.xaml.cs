using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BggApi;

namespace BggUwp.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        private void HotItems_ItemClick(object sender, ItemClickEventArgs e)
        {
            //var item = e.ClickedItem as HotDataItem;
            //this.Frame.Navigate(typeof(BoardgamePage), item.BoardGameId);
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
                    FilterAppBarButton.Visibility = Visibility.Visible;
                    break;
                case 2:
                    HideAllAppBarButtons();
                    break;
            }
        }

        private void HideAllAppBarButtons()
        {
            FindAppBarButton.Visibility = Visibility.Collapsed;
            FilterAppBarButton.Visibility = Visibility.Collapsed;
        }
    }
}
