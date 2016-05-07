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
        Windows.UI.Core.CoreDispatcher dispatcher; 

        public HotItemsViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
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

        public void LoadHotItemsList()
        {
            LoadFromStorage();
            LoadFromWeb();
        }

        private void LoadFromStorage()
        {
            HotItemsList.Clear();

            IEnumerable<HotDataItem> storedItems = StorageService.LoadAllHotItems();
            foreach (var item in storedItems)
            {
                HotItemsList.Add(item);
            }
        }

        private async void LoadFromWeb()
        {
            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                ObservableCollection<HotDataItem> tmp = await DataService.Instance.LoadHotItemsList();
                if (tmp.Count() > 0)
                    HotItemsList = tmp;
            });
        }

    }
}
