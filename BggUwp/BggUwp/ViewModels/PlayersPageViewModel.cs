using BggUwp.Data;
using BggUwp.Data.Models;
using BggUwp.Messaging;
using GalaSoft.MvvmLight.Messaging;
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
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                LoadPlayers();
                Messenger.Default.Register<RefreshDataMessage>(this, RefreshData);
            }

        }

        private void LoadPlayers()
        {
            PlayersList = DataService.Instance.LoadPlayers();
        }

        private void RefreshData(RefreshDataMessage msg)
        {
            if (msg.RequestedRefreshScope == RefreshDataMessage.RefreshScope.Players)
            {
                LoadPlayers();
            }
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

        public void RemovePlayer(PlayerDataItem player)
        {
            if (DataService.Instance.RemovePlayer(player))
            {
                PlayersList.Remove(player);
            }
        }
    }
}
