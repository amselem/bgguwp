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
using Windows.UI.Xaml.Controls;

namespace BggUwp.ViewModels
{
    public class LogPlayViewModel : ViewModelBase
    {
        public LogPlayViewModel() { }
        public LogPlayViewModel(int gameId)
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                CurrentPlayItem.BoardGameId = gameId;
                LoadData();
            }
        }

        private async void LoadData()
        {
            PlayersList = await DataService.Instance.LoadPlayersList();
        }

        private PlayDataItem _CurrentPlayItem = new PlayDataItem() { PlayDate = DateTime.Now, Players = new ObservableCollection<PlayerStatsDataItem>() };
        public PlayDataItem CurrentPlayItem
        {
            get
            {
                if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                    return DesignDataService.LoadPlay();
                }
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
                FilteredPlayersList = new ObservableCollection<PlayerDataItem>(PlayersList);
            }
        }

        public ObservableCollection<PlayerDataItem> _FilteredPlayersList = new ObservableCollection<PlayerDataItem>();
        public ObservableCollection<PlayerDataItem> FilteredPlayersList
        {
            get
            {
                return _FilteredPlayersList;
            }
            set
            {
                Set(ref _FilteredPlayersList, value);
            }
        }

        public void FilterPlayers(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                FilteredPlayersList = new ObservableCollection<PlayerDataItem>(PlayersList.Where(p => p.Name.Contains(sender.Text)).ToList());
            }
        }

        public void PlayerChoosen(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var player = args.ChosenSuggestion as PlayerDataItem;
            CurrentPlayItem.Players.Add(new PlayerStatsDataItem(player));
            //sender.Text = String.Empty;
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

        public void SelectedDateChanged(CalendarDatePicker cal, CalendarDatePickerDateChangedEventArgs args)
        {
            CurrentPlayItem.PlayDate = args.NewDate.Value.DateTime;
        }
    }
}
