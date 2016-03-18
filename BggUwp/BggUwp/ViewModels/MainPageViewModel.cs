using BggUwp.Data.Models;
using BggUwp.Data.Models.Abstract;
using System.Collections.Generic;
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

        public void GoToBoardGamePage(object sender, ItemClickEventArgs e) =>
    NavigationService.Navigate(typeof(Views.BoardGamePage), ((BaseItem)e.ClickedItem).BoardGameId);
    }
}

