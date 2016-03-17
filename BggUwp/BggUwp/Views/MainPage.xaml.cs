using System;
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
            //switch (((Pivot)sender).SelectedIndex)
            //{
            //    case 0:
            //        FindAppBarButton.Visibility = Visibility.Visible;
            //        FilterAppBarButton.Visibility = Visibility.Collapsed;
            //        break;

            //    case 1:
            //        FindAppBarButton.Visibility = Visibility.Collapsed;
            //        FilterAppBarButton.Visibility = Visibility.Visible;
            //        break;
            //}
        }
    }
}
