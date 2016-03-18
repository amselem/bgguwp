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
    public class CollectionViewModel : ViewModelBase
    {
        private DataService dataService;
        Windows.UI.Core.CoreDispatcher dispatcher;

        public CollectionViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                dataService = new DataService();
                dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
                LoadCollection();
            }

            this.Collection.CollectionChanged += Collection_CollectionChanged;
        }

        public ObservableCollection<CollectionDataItem> Collection = new ObservableCollection<CollectionDataItem>();

        public ObservableCollection<CollectionDataItem> _FilteredCollection = new ObservableCollection<CollectionDataItem>();
        public ObservableCollection<CollectionDataItem> FilteredCollection
        {
            get
            {
                return _FilteredCollection;
            }
        }

        public async void LoadCollection()
        {
            Collection.Clear();
            IEnumerable<CollectionDataItem> storedItems;

            // retrieve collection
            storedItems = StorageService.LoadAllCollectionItems();
            foreach (var item in storedItems)
            {
                Collection.Add(item);
            }
            RepopulateFilteredCollection(); // Required because previous lines do not fire Collection_CollectionChanged

            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                // TODO Display loading bar
                Collection = await dataService.LoadCollection();
                RepopulateFilteredCollection(); // Required because previous lines do not fire Collection_CollectionChanged
            });
        }

        private void Collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RepopulateFilteredCollection();
        }

        private void RepopulateFilteredCollection()
        {
            FilteredCollection.Clear();

            foreach (var item in Collection)
                FilteredCollection.Add(item);
        }
    }
}
