﻿using BggUwp.Data;
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
                {
                    IsInCollection = true;
                    EditDialogCollectionItem = new CollectionDataItem(CurrentCollectionItem);
                }
                else
                {
                    IsInCollection = false;
                }
            }
        }

        private CollectionDataItem _EditDialogCollectionItem;
        public CollectionDataItem EditDialogCollectionItem
        {
            get
            {
                return _EditDialogCollectionItem;
            }
            set
            {
                Set(ref _EditDialogCollectionItem, value);
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
            CurrentCollectionItem = DataService.Instance.LoadCollectionItemFromStorage(gameId);
            RulesLink = new Uri(await DataService.Instance.GetRulesLink(gameId));
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

        private async void ExecuteAddCommand()
        {
            if (!CanExecuteAddCommand())
                return;

            await DataService.Instance.AddToCollection(CurrentBoardGame.BoardGameId);
            //await Task.Delay(2500);
            //CurrentCollectionItem = DataService.Instance.LoadCollectionItem(CurrentBoardGame.BoardGameId);
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

            CurrentCollectionItem = EditDialogCollectionItem;
            await DataService.Instance.EditCollectionItem(CurrentCollectionItem);
            StorageService.SaveCollectionItem(CurrentCollectionItem);
            Messenger.Default.Send<RefreshDataMessage>(new RefreshDataMessage()
            {
                RequestedRefreshScope = RefreshDataMessage.RefreshScope.Collection,
                RequestedRefreshType = RefreshDataMessage.RefreshType.Local
            });
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
            await DataService.Instance.LoadBoardGame(CurrentBoardGame.BoardGameId); // TODO Change          
            Messenger.Default.Send<RefreshDataMessage>(new RefreshDataMessage() { RequestedRefreshScope = RefreshDataMessage.RefreshScope.Plays });

            if (CurrentCollectionItem != null)
                CurrentCollectionItem.NumberOfPlays++; // TODO Should reload collection item
        });

        public void SelectedDateChanged(Windows.UI.Xaml.Controls.CalendarDatePicker cal, Windows.UI.Xaml.Controls.CalendarDatePickerDateChangedEventArgs args)
        {
            CurrentPlayItem.PlayDate = args.NewDate.Value.DateTime;
        }
    }
}
