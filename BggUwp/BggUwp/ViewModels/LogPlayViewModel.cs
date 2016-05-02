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
    public class LogPlayViewModel : ViewModelBase
    {
        public LogPlayViewModel()
        {

        }
        public LogPlayViewModel(int gameId)
        {
            CurrentPlayItem.BoardGameId = gameId;
            LoadData();
        }

        private async void LoadData()
        {
            PlayersList = await DataService.Instance.LoadPlayersList();
        }

        private PlayDataItem _CurrentPlayItem = new PlayDataItem() { PlayDate = DateTime.Now };
        public PlayDataItem CurrentPlayItem
        {
            get
            {
                return _CurrentPlayItem;
            }
            set
            {
                Set(ref _CurrentPlayItem, value);
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

        public DelegateCommand LogPlayCommand => new DelegateCommand(async () =>
        {
            if (await DataService.Instance.LogPlay(CurrentPlayItem.BoardGameId, CurrentPlayItem.PlayDate, CurrentPlayItem.NumberOfPlays, CurrentPlayItem.UserComment, CurrentPlayItem.Length))
            {
                Messenger.Default.Send<RefreshDataMessage>(new RefreshDataMessage() { RequestedRefreshScope = RefreshDataMessage.RefreshScope.Plays });
            }
            else
            {
                Messenger.Default.Send(new StatusMessage()
                {
                    Status = StatusMessage.StatusType.Error,
                    Message = "There was an error while logging play"
                });
            }
        });

        public void SelectedDateChanged(Windows.UI.Xaml.Controls.CalendarDatePicker cal, Windows.UI.Xaml.Controls.CalendarDatePickerDateChangedEventArgs args)
        {
            CurrentPlayItem.PlayDate = args.NewDate.Value.DateTime;
        }
    }
}
