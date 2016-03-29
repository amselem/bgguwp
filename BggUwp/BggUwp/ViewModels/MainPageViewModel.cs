using BggUwp.Data;
using BggUwp.Data.Models;
using BggUwp.Data.Models.Abstract;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BggUwp.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private DataService dataService;

        public MainPageViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                dataService = new DataService();
            }
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

        public void GoToSearchPage() =>
            NavigationService.Navigate(typeof(Views.SearchPage));

    }
}

