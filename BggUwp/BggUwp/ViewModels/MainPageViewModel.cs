using BggUwp.Data;
using BggUwp.Data.Models;
using BggUwp.Data.Models.Abstract;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System;
using BggUwp.Messaging;

namespace BggUwp.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel()
        {
        }

        private HotItemsViewModel _HotItemsVM = new HotItemsViewModel();
        public HotItemsViewModel HotItemsVM
        {
            get
            {
                return _HotItemsVM;
            }
        }

        private CollectionViewModel _CollectionVM = new CollectionViewModel();
        public CollectionViewModel CollectionVM
        {
            get
            {
                return _CollectionVM;
            }
        }

        private PlaysViewModel _PlaysVM = new PlaysViewModel();
        public PlaysViewModel PlaysVM
        {
            get
            {
                return _PlaysVM;
            }
        }

        public void GoToBoardGamePage(object sender, ItemClickEventArgs e) =>
            NavigationService.Navigate(typeof(Views.BoardGamePage), ((BaseItem)e.ClickedItem).BoardGameId);

        public void GoToPlayersPage() => NavigationService.Navigate(typeof(Views.PlayersPage));

        public void GoToSearchPage() => NavigationService.Navigate(typeof(Views.SearchPage));

        public void GoToSettingsPage() => NavigationService.Navigate(typeof(Views.SettingsPage));
    }
}

