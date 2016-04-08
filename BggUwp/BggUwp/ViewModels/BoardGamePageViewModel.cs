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
using Windows.UI.Xaml.Navigation;

namespace BggUwp.ViewModels
{
    public class BoardGamePageViewModel : ViewModelBase
    {
        Windows.UI.Core.CoreDispatcher dispatcher; 

        public BoardGamePageViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
            }
        }

        private BoardGameDataItem _CurrentBoardGame = new BoardGameDataItem();
        public BoardGameDataItem CurrentBoardGame
        {
            get
            {
                if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                    return DesignDataService.LoadBoardGame();
                }

                return _CurrentBoardGame;
            }
            set
            {
                Set(ref _CurrentBoardGame, value);
                OnStatusChanged();
            }
        }

        private CollectionDataItem _CurrentCollectionItem;
        public CollectionDataItem CurrentCollectionItem
        {
            get
            {
                if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                    return DesignDataService.LoadCollectionItem();
                }

                return _CurrentCollectionItem;
            }
            set
            {
                Set(ref _CurrentCollectionItem, value);
                OnStatusChanged();
            }
        }

        private void OnStatusChanged()
        {
            AddCommand.RaiseCanExecuteChanged();
            EditCommand.RaiseCanExecuteChanged();
            RemoveCommand.RaiseCanExecuteChanged();
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            int gameId = (int)parameter;
            LoadData(gameId);
            await Task.CompletedTask;
        }

        private async void LoadData(int gameId)
        {
            CurrentBoardGame = await DataService.Instance.LoadBoardGame(gameId);
            CurrentCollectionItem = DataService.Instance.LoadCollectionItem(gameId);
            // TODO Implement collection item null scenario
        }

        private DelegateCommand _AddCommand;
        public DelegateCommand AddCommand
        {
            get
            {
                return _AddCommand ??
                    (_AddCommand = new DelegateCommand(ExecuteAddCommand, CanExecuteAddCommand));
            }
            set
            {
                Set(ref _AddCommand, value);
            }
        }

        private void ExecuteAddCommand()
        {
            if (!CanExecuteAddCommand())
                return;

            DataService.Instance.AddToCollection(CurrentBoardGame.BoardGameId);
            Messenger.Default.Send<RefreshDataMessage>(new RefreshDataMessage() { RequestedRefreshScope = RefreshDataMessage.RefreshScope.Collection });
        }

        private bool CanExecuteAddCommand()
        {
            if (CurrentCollectionItem != null)
                return false;

            return true;
        }

        private DelegateCommand _EditCommand;
        public DelegateCommand EditCommand
        {
            get
            {
                return _EditCommand ??
                    (_EditCommand = new DelegateCommand(ExecuteEditCommand, CanExecuteEditCommand));
            }
            set
            {
                Set(ref _EditCommand, value);
            }
        }

        private void ExecuteEditCommand()
        {
            if (!CanExecuteEditCommand())
                return;

            DataService.Instance.EditCollectionItem(CurrentCollectionItem);
            Messenger.Default.Send<RefreshDataMessage>(new RefreshDataMessage() { RequestedRefreshScope = RefreshDataMessage.RefreshScope.Collection });
        }

        private bool CanExecuteEditCommand()
        {
            if (CurrentCollectionItem == null)
                return false;

            return true;
        }

        private DelegateCommand _RemoveCommand;
        public DelegateCommand RemoveCommand
        {
            get
            {
                return _RemoveCommand ??
                    (_RemoveCommand = new DelegateCommand(ExecuteRemoveCommand, CanExecuteRemoveCommand));
            }
            set
            {
                Set(ref _RemoveCommand, value);
            }
        }

        private void ExecuteRemoveCommand()
        {
            if (!CanExecuteRemoveCommand())
                return;

            DataService.Instance.RemoveCollectionItem(CurrentCollectionItem.CollectionItemId);
            Messenger.Default.Send<RefreshDataMessage>(new RefreshDataMessage() { RequestedRefreshScope = RefreshDataMessage.RefreshScope.Collection });
        }

        private bool CanExecuteRemoveCommand()
        {
            if (CurrentCollectionItem == null)
                return false;

            return true;
        }
    }
}
