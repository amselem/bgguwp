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
    public class PlaysViewModel : ViewModelBase
    {
        private DataService dataService; 
        Windows.UI.Core.CoreDispatcher dispatcher; 

        public PlaysViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                dataService = new DataService();
                dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
                LoadPlays();
            }
        }

        private ObservableCollection<PlayDataItem> _PlaysList = new ObservableCollection<PlayDataItem>();
        public ObservableCollection<PlayDataItem> PlaysList
        {
            get { return _PlaysList; }
            set
            {
                Set(ref _PlaysList, value);
            }
        }

        public async void LoadPlays()
        {
            var apiPlays = await dataService.LoadPlays();

            foreach (var item in apiPlays)
            {
                PlaysList.Add(new PlayDataItem(item));
            }
        }
    }
}
