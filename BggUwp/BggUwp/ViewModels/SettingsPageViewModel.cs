using BggUwp.Data;
using BggUwp.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;

namespace BggUwp.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private SettingsService settingsService;

        public SettingsPageViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                settingsService = SettingsService.Instance;
                if (StorageService.RetrieveUserCredentials() != null)
                {
                    UserName = StorageService.RetrieveUserCredentials().UserName;
                }
            }
        }

        string _userName = default(string);
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                Set(ref _userName, value);
                AccountStatusText = "Logged as " + UserName;
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        string _Password = "default";
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                Set(ref _Password, value);
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        string _AccountStatusText = "Sign in to BGG";
        public string AccountStatusText
        {
            get
            {
                return _AccountStatusText;
            }
            set
            {
                Set(ref _AccountStatusText, value);
            }
        }

        private DelegateCommand _LoginCommand;
        public DelegateCommand LoginCommand
        {
            get
            {
                return _LoginCommand ??
                    (_LoginCommand = new DelegateCommand(ExecuteLoginCommand, CanExecuteLoginCommand));
            }
            set
            {
                Set(ref _LoginCommand, value);
            }
        }

        private void ExecuteLoginCommand()
        {
            if (!CanExecuteLoginCommand())
                return;

            StorageService.SaveUserCredentials(UserName, Password);
            DataService.Instance.RetrieveCredentials();
        }

        private bool CanExecuteLoginCommand()
        {
            if (String.IsNullOrEmpty(UserName))
                return false;

            return true;
        }

        bool _ShouldUpdateOnlyOnWiFi = false;
        public bool ShouldUpdateOnlyOnWiFi
        {
            get
            {
                return _ShouldUpdateOnlyOnWiFi;
            }
            set
            {
                settingsService.UpdateDataOnlyOnWiFi = value;
                Set(ref _ShouldUpdateOnlyOnWiFi, value);
            }
        }
    }
}