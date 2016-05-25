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

        private CollectionDataItem _CurrentCollectionItem = new CollectionDataItem();
        public CollectionDataItem CurrentCollectionItem
        {
            get
            {
                if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                    IsInCollection = true;
                    return DesignDataService.LoadCollectionItem();
                }

                return _CurrentCollectionItem;
            }
            set
            {
                Set(ref _CurrentCollectionItem, value);
                if (CurrentCollectionItem.CollectionItemId > 0)
                {
                    IsInCollection = true;
                }
                else
                {
                    IsInCollection = false;
                }
            }
        }

        private bool _IsInCollection = false;
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
            Tuple<int, int> parameters = (Tuple<int, int>)parameter;
            if (DataService.Instance.IsThereInternetAccess())
            {
                LoadData(parameters.Item1, parameters.Item2);
            }
            else
            {
                Messenger.Default.Send(new StatusMessage()
                {
                    Status = StatusMessage.StatusType.Error,
                    Message = "There is no internet access"
                });
            }

            await Task.CompletedTask;
        }

        private async void LoadData(int gameId, int collectionId)
        {
            LogPlayDialogVM = new LogPlayViewModel(gameId);
            EditDialogVM = new EditDialogViewModel(gameId);
            CurrentCollectionItem = DataService.Instance.LoadCollectionItemFromStorage(collectionId);
            CurrentBoardGame = await DataService.Instance.LoadBoardGame(gameId);
            RulesLink = new Uri(await DataService.Instance.GetRulesLink(gameId));

            // in case of desynchronized local data
            CurrentCollectionItem = await DataService.Instance.LoadCollectionItemFromWeb(gameId, collectionId);
            if (!String.IsNullOrEmpty(CurrentCollectionItem.BoardGameName))
            {
                CurrentBoardGame.BoardGameName = CurrentCollectionItem.BoardGameName;
                CurrentBoardGame.ImageWebLink = CurrentCollectionItem.ImageWebLink;
            }
            EditDialogVM = new EditDialogViewModel(CurrentCollectionItem);
        }

        private void RefreshData(RefreshDataMessage msg)
        {
            if (msg.RequestedRefreshScope == RefreshDataMessage.RefreshScope.BoardGame)
            {
                CurrentCollectionItem = DataService.Instance.LoadCollectionItemFromStorage(CurrentBoardGame.BoardGameId);
            }
        }

        public DelegateCommand GoToBggCommand => new DelegateCommand(async () =>
        {
            if (CurrentBoardGame != null)
            {
                Uri Url = new Uri(CurrentBoardGame.VisitURL);
                var result = await Windows.System.Launcher.LaunchUriAsync(Url);
            }
        });

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

            if (await DataService.Instance.AddToCollection(CurrentBoardGame.BoardGameId))
            {
                CurrentCollectionItem = await DataService.Instance.LoadCollectionItemFromWeb(CurrentBoardGame.BoardGameId);
                StorageService.SaveCollectionItem(CurrentCollectionItem);
                EditDialogVM = new EditDialogViewModel(CurrentCollectionItem);
                Messenger.Default.Send<RefreshDataMessage>(new RefreshDataMessage()
                {
                    RequestedRefreshScope = RefreshDataMessage.RefreshScope.Collection,
                    RequestedRefreshType = RefreshDataMessage.RefreshType.Local
                });
            }
            else
            {
                Messenger.Default.Send(new StatusMessage()
                {
                    Status = StatusMessage.StatusType.Error,
                    Message = "Adding an item was unsuccessful."
                });
            }
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

            if (await DataService.Instance.RemoveCollectionItem(CurrentCollectionItem.CollectionItemId))
            {
                StorageService.RemoveCollectionItem(CurrentCollectionItem);
                Messenger.Default.Send<RefreshDataMessage>(new RefreshDataMessage()
                {
                    RequestedRefreshScope = RefreshDataMessage.RefreshScope.Collection,
                    RequestedRefreshType = RefreshDataMessage.RefreshType.Local
                });
                CurrentCollectionItem = new CollectionDataItem();
            }
            else
            {
                Messenger.Default.Send(new StatusMessage()
                {
                    Status = StatusMessage.StatusType.Error,
                    Message = "Removing an item was unsuccessful."
                });
            }
        }

        private bool CanExecuteRemoveCommand()
        {
            return IsInCollection;
        }
    }
}
