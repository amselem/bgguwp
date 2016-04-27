using BggUwp.Data;
using BggUwp.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;

namespace BggUwp.ViewModels
{
    public class PlayersPageViewModel : ViewModelBase
    {
        public PlayersPageViewModel()
        {
            LoadPlayers();
        }

        private async void LoadPlayers()
        {
            PlayersList = await DataService.Instance.GetPlayersList();
        }

        public ObservableCollection<PlayerDataItem> _PlayersList = new ObservableCollection<PlayerDataItem>();
        public ObservableCollection<PlayerDataItem> PlayersList
        {
            get
            {
                if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                    return DesignDataService.LoadPlayersList();
                }
                return _PlayersList;
            }
            set
            {
                Set(ref _PlayersList, value);
            }
        }

        //    public void GoToBoardGamePage(object sender, ItemClickEventArgs e) =>
        //NavigationService.Navigate(typeof(Views.BoardGamePage), ((BaseItem)e.ClickedItem).BoardGameId);
        public void GoToPlayerPage() => NavigationService.Navigate(typeof(Views.SearchPage));
    }
}
