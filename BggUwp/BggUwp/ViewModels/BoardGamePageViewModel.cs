using BggUwp.Data;
using BggUwp.Data.Models;
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
        private DataService dataService; 
        Windows.UI.Core.CoreDispatcher dispatcher; 

        public BoardGamePageViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                dataService = new DataService();
                dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
            }
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            int gameId = (int)parameter;
            await Task.CompletedTask;
        }
    }
}
