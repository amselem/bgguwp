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
    public class AddPlayerDialogViewModel : ViewModelBase
    {
        public AddPlayerDialogViewModel()
        {
            CurrentPlayer = new PlayerDataItem();
        }

        private PlayerDataItem _CurrentPlayer;
        public PlayerDataItem CurrentPlayer
        {
            get
            {
                return _CurrentPlayer;
            }
            set
            {
                Set(ref _CurrentPlayer, value);
            }
        }

        private DelegateCommand _SaveCommand;
        public DelegateCommand SaveCommand
        {
            get
            {
                return _SaveCommand ??
                    (_SaveCommand = new DelegateCommand(ExecuteSaveCommand, CanExecuteSaveCommand));
            }
            set
            {
                Set(ref _SaveCommand, value);
            }
        }

        private void ExecuteSaveCommand()
        {
            if (!CanExecuteSaveCommand())
                return;

            if (!DataService.Instance.AddPlayer(CurrentPlayer))
            {
                Messenger.Default.Send(new StatusMessage()
                {
                    Status = StatusMessage.StatusType.Error,
                    Message = "Adding an player was unsuccessful."
                });
            }
        }

        private bool CanExecuteSaveCommand()
        {
            return String.IsNullOrEmpty(CurrentPlayer.Name);
        }
    }
}
