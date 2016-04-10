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
                if (value != null)
                    IsInCollection = true;
                else
                    IsInCollection = false;
            }
        }

        private bool _IsInCollection;
        public bool IsInCollection
        {
            get
            {
                return _IsInCollection;
            }
            set
            {
                Set(ref _IsInCollection, value);
                OnStatusChanged();
            }
        }

        private void OnStatusChanged()
        {
            AddCommand.RaiseCanExecuteChanged();
            EditCommand.RaiseCanExecuteChanged();
            RemoveCommand.RaiseCanExecuteChanged();

            ShowEditDialogCommand.RaiseCanExecuteChanged();
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

        private DelegateCommand _ShowEditDialogCommand;
        public DelegateCommand ShowEditDialogCommand
        {
            get
            {
                return _ShowEditDialogCommand ??
                    (_ShowEditDialogCommand = new DelegateCommand(ExecuteShowEditDialogCommand, CanExecuteShowEditDialogCommand));
            }
            set
            {
                Set(ref _ShowEditDialogCommand, value);
            }
        }

        private async void ExecuteShowEditDialogCommand()
        {
        }

        private bool CanExecuteShowEditDialogCommand()
        {
            if (CurrentCollectionItem == null)
                return false;

            return true;
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

        private async void ExecuteAddCommand()
        {
            if (!CanExecuteAddCommand())
                return;

            await DataService.Instance.AddToCollection(CurrentBoardGame.BoardGameId);
            Messenger.Default.Send<RefreshDataMessage>(new RefreshDataMessage() { RequestedRefreshScope = RefreshDataMessage.RefreshScope.Collection });
        }

        private bool CanExecuteAddCommand()
        {
            return !IsInCollection;
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

        private async void ExecuteEditCommand()
        {
            if (!CanExecuteEditCommand())
                return;

            await DataService.Instance.EditCollectionItem(CurrentCollectionItem);
            Messenger.Default.Send<RefreshDataMessage>(new RefreshDataMessage() { RequestedRefreshScope = RefreshDataMessage.RefreshScope.Collection });
        }

        private bool CanExecuteEditCommand()
        {
            return IsInCollection;
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

        private async void ExecuteRemoveCommand()
        {
            if (!CanExecuteRemoveCommand())
                return;

            await DataService.Instance.RemoveCollectionItem(CurrentCollectionItem.CollectionItemId);
            Messenger.Default.Send<RefreshDataMessage>(new RefreshDataMessage() { RequestedRefreshScope = RefreshDataMessage.RefreshScope.Collection });
            CurrentCollectionItem = null;
        }

        private bool CanExecuteRemoveCommand()
        {
            return IsInCollection;
        }

        public DelegateCommand LogPlayCommand => new DelegateCommand(async () =>
        {
            await DataService.Instance.LoadBoardGame(CurrentBoardGame.BoardGameId); // TODO Change          
            Messenger.Default.Send<RefreshDataMessage>(new RefreshDataMessage() { RequestedRefreshScope = RefreshDataMessage.RefreshScope.Plays });
        });
    }
}
