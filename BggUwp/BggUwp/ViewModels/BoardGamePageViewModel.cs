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
                Messenger.Default.Register<RefreshDataMessage>(this, RefreshData);
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
                {
                    IsInCollection = true;
                }
                else
                {
                    IsInCollection = false;
                }
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

        private Uri _RulesLink;
        public Uri RulesLink
        {
            get
            {
                return _RulesLink;
            }
            set
            {
                Set(ref _RulesLink, value);
            }
        }

        private EditDialogViewModel _EditDialogVM;
        public EditDialogViewModel EditDialogVM
        {
            get
            {
                return _EditDialogVM;
            }
            set
            {
                Set(ref _EditDialogVM, value);
            }
        }

        private LogPlayViewModel _LogPlayDialogVM;
        public LogPlayViewModel LogPlayDialogVM
        {
            get
            {
                return _LogPlayDialogVM;
            }
            set
            {
                Set(ref _LogPlayDialogVM, value);
            }
        }

        private void OnStatusChanged()
        {
            AddCommand.RaiseCanExecuteChanged();
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
            CurrentCollectionItem = DataService.Instance.LoadCollectionItemFromStorage(gameId);
            RulesLink = new Uri(await DataService.Instance.GetRulesLink(gameId));
            EditDialogVM = new EditDialogViewModel(gameId);
            LogPlayDialogVM = new LogPlayViewModel(gameId);
            // TODO Implement collection item null scenario
        }

        private void RefreshData(RefreshDataMessage msg)
        {
            if (msg.RequestedRefreshScope == RefreshDataMessage.RefreshScope.BoardGame)
            {
                CurrentCollectionItem = DataService.Instance.LoadCollectionItemFromStorage(CurrentBoardGame.BoardGameId);
            }
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
            CurrentCollectionItem = await DataService.Instance.LoadCollectionItemFromWeb(CurrentBoardGame.BoardGameId);
            StorageService.SaveCollectionItem(CurrentCollectionItem);
            Messenger.Default.Send<RefreshDataMessage>(new RefreshDataMessage()
            {
                RequestedRefreshScope = RefreshDataMessage.RefreshScope.Collection,
                RequestedRefreshType = RefreshDataMessage.RefreshType.Local
            });
        }

        private bool CanExecuteAddCommand()
        {
            return !IsInCollection;
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
            StorageService.RemoveCollectionItem(CurrentCollectionItem);
            Messenger.Default.Send<RefreshDataMessage>(new RefreshDataMessage()
            {
                RequestedRefreshScope = RefreshDataMessage.RefreshScope.Collection,
                RequestedRefreshType = RefreshDataMessage.RefreshType.Local
            });
            CurrentCollectionItem = null;
        }

        private bool CanExecuteRemoveCommand()
        {
            return IsInCollection;
        }
    }
}
