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
    public class EditDialogViewModel : ViewModelBase
    {
        public EditDialogViewModel()
        {

        }
        public EditDialogViewModel(int gameId)
        {
            EditableCollectionItem = DataService.Instance.LoadCollectionItemFromStorage(gameId);
            OriginalCollectionItem = new CollectionDataItem(EditableCollectionItem);
        }

        public EditDialogViewModel(CollectionDataItem item)
        {
            OriginalCollectionItem = new CollectionDataItem(item);
            EditableCollectionItem = new CollectionDataItem(item);
        }

        private CollectionDataItem _EditableCollectionItem;
        public CollectionDataItem EditableCollectionItem
        {
            get
            {
                return _EditableCollectionItem;
            }
            set
            {
                Set(ref _EditableCollectionItem, value);
            }
        }

        private CollectionDataItem _OriginalCollectionItem;
        public CollectionDataItem OriginalCollectionItem
        {
            get
            {
                return _OriginalCollectionItem;
            }
            set
            {
                Set(ref _OriginalCollectionItem, value);
            }
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

            if (await DataService.Instance.EditCollectionItem(EditableCollectionItem))
            {
                StorageService.SaveCollectionItem(EditableCollectionItem);
                Messenger.Default.Send<RefreshDataMessage>(new RefreshDataMessage()
                {
                    RequestedRefreshScope = RefreshDataMessage.RefreshScope.Collection,
                    RequestedRefreshType = RefreshDataMessage.RefreshType.Local
                });
                Messenger.Default.Send<RefreshDataMessage>(new RefreshDataMessage()
                {
                    RequestedRefreshScope = RefreshDataMessage.RefreshScope.BoardGame,
                    RequestedRefreshType = RefreshDataMessage.RefreshType.Local
                });
            }
            else
            {
                Messenger.Default.Send(new StatusMessage()
                {
                    Status = StatusMessage.StatusType.Error,
                    Message = "Editing an item was unsuccessful."
                });
            }
        }

        private bool CanExecuteEditCommand()
        {
            return true;
        }
    }
}
