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
    public class HotItemsViewModel : ViewModelBase
    {
        private DataService dataService; 
        Windows.UI.Core.CoreDispatcher dispatcher; 

        public HotItemsViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                dataService = new DataService();
                dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
                LoadHotItemsList();
            }
        }

        private ObservableCollection<HotDataItem> _HotItemsList = new ObservableCollection<HotDataItem>();
        public ObservableCollection<HotDataItem> HotItemsList
        {
            get
            {
                if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                    return DesignDataService.LoadHotItemsList();
                }

                return _HotItemsList;
            }
            set
            {
                Set(ref _HotItemsList, value);
            }
        }

        private async void LoadHotItemsList()
        {
            HotItemsList.Clear();

            IEnumerable<HotDataItem> storedItems = StorageService.LoadAllHotItems();
            foreach (var item in storedItems)
            {
                HotItemsList.Add(item);
            }

            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                ObservableCollection<HotDataItem> tmp = await dataService.LoadHotItemsList();
                if (tmp.Count() > 0)
                    HotItemsList = tmp;
            });
        }
    }
}
