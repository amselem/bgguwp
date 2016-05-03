using BggUwp.Controls;
using BggUwp.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BggUwp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlayersPage : Page
    {
        public PlayersPage()
        {
            this.InitializeComponent();
            SetCommandsVisibility(PlayersListView);
        }

        private async void AddPlayer_Click(object sender, RoutedEventArgs e)
        {
            AddPlayerDialog tmp = new AddPlayerDialog();
            await tmp.ShowAsync();
        }

        private void SetCommandsVisibility(ListView listView)
        {
            if (listView.SelectionMode == ListViewSelectionMode.Multiple || listView.SelectedItems.Count > 1)
            {
                SelectAppBarButton.Visibility = Visibility.Collapsed;
                CancelSelectionAppBarButton.Visibility = Visibility.Visible;
                AddPlayerAppBarButton.Visibility = Visibility.Collapsed;
                RemovePlayerAppBarButton.Visibility = Visibility.Visible;
                //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else
            {
                SelectAppBarButton.Visibility = Visibility.Visible;
                CancelSelectionAppBarButton.Visibility = Visibility.Collapsed;
                AddPlayerAppBarButton.Visibility = Visibility.Visible;
                RemovePlayerAppBarButton.Visibility = Visibility.Collapsed;
                //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }
        private void SelectPlayers(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            PlayersListView.SelectionMode = ListViewSelectionMode.Multiple;
            SetCommandsVisibility(PlayersListView);
        }

        private void RemovePlayer(object sender, RoutedEventArgs e)
        {
            if (PlayersListView.SelectedIndex != -1)
            {
                // When an item is removed from the underlying collection, the Listview is updated, 
                // hence the this.SelectedItems is updated as well. 
                // It's needed to copy the selected items collection to iterate over other collection that 
                // is not updated.
                List<PlayerDataItem> selectedItems = new List<PlayerDataItem>();
                foreach (PlayerDataItem item in PlayersListView.SelectedItems)
                {
                    selectedItems.Add(item);
                }

                var tmp = this.DataContext as ViewModels.PlayersPageViewModel;
                foreach (PlayerDataItem item in selectedItems)
                {
                    tmp.RemovePlayer(item);
                }
                PlayersListView.SelectionMode = ListViewSelectionMode.None;
                SetCommandsVisibility(PlayersListView);
            }
        }
        private void CancelSelection(object sender, RoutedEventArgs e)
        {
            // If the list is multiple selection mode but there is no items selected, 
            // then the list should return to the initial selection mode.
            if (PlayersListView.SelectedItems.Count == 0)
            {
                PlayersListView.SelectionMode = ListViewSelectionMode.None;
                SetCommandsVisibility(PlayersListView);
            }
            else
            {
                PlayersListView.SelectedItems.Clear();
            }
        }
    }
}
