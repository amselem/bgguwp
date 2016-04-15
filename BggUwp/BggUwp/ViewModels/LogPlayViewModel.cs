using BggUwp.Data;
using BggUwp.Data.Models;
using BggUwp.Messaging;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
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

        public DelegateCommand LogPlayCommand => new DelegateCommand(async () =>
        {
            await DataService.Instance.LogPlay(CurrentPlayItem.BoardGameId, CurrentPlayItem.PlayDate, CurrentPlayItem.NumberOfPlays, CurrentPlayItem.UserComment, CurrentPlayItem.Length);
            Messenger.Default.Send<RefreshDataMessage>(new RefreshDataMessage() { RequestedRefreshScope = RefreshDataMessage.RefreshScope.Plays });
        });

        public void SelectedDateChanged(Windows.UI.Xaml.Controls.CalendarDatePicker cal, Windows.UI.Xaml.Controls.CalendarDatePickerDateChangedEventArgs args)
        {
            CurrentPlayItem.PlayDate = args.NewDate.Value.DateTime;
        }
    }
}
