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

            CurrentTextFilter.FilterTextChanged += CurrentTextFilter_FilterTextChanged;
            Collection.CollectionChanged += Collection_CollectionChanged;
        }

        public ObservableCollection<CollectionDataItem> _Collection = new ObservableCollection<CollectionDataItem>();
        public ObservableCollection<CollectionDataItem> Collection
        {
            get
            {
                return _Collection;
            }
            set
            {
                Set(ref _Collection, value);
            }
        }

        public ObservableCollection<CollectionDataItem> _FilteredCollection = new ObservableCollection<CollectionDataItem>();
        public ObservableCollection<CollectionDataItem> FilteredCollection
        {
            get
            {
                return _FilteredCollection;
            }
            set
            {
                Set(ref _FilteredCollection, value);
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
                ObservableCollection<CollectionDataItem> tmp = await dataService.LoadCollection();
                if (tmp.Count() > 0)
                {
                    Collection = tmp;
                    RepopulateFilteredCollection(); // Required because previous lines do not fire Collection_CollectionChanged
                }
            });
        }

        private void Collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RepopulateFilteredCollection();
        }

        #region Filtering methods
        private StatusFilter _CurrentStatusFilter { get; set; }
        public StatusFilter CurrentStatusFilter
        {
            get
            {
                if (_CurrentStatusFilter == null)
                    _CurrentStatusFilter = StatusFilters.First();
                return _CurrentStatusFilter;
            }
            set
            {
                if (_CurrentStatusFilter != value)
                {
                    _CurrentStatusFilter = value;
                    RaisePropertyChanged("CurrentStatusFilter");
                    RepopulateFilteredCollection();
                }
            }
        }

        private int _CurrentPlayerFilter { get; set; }
        public int CurrentPlayerFilter
        {
            get
            {
                return _CurrentPlayerFilter;
            }
            set
            {
                if (_CurrentPlayerFilter != value)
                {
                    _CurrentPlayerFilter = value;
                    RaisePropertyChanged("CurrentPlayerFilter");
                    RepopulateFilteredCollection();
                }
            }
        }

        private int _CurrentPlayTimeFilter { get; set; }
        public int CurrentPlayTimeFilter
        {
            get
            {
                return _CurrentPlayTimeFilter;
            }
            set
            {
                if (_CurrentPlayTimeFilter != value)
                {
                    _CurrentPlayTimeFilter = value;
                    RaisePropertyChanged("CurrentPlayTimeFilter");
                    RepopulateFilteredCollection();
                }
            }
        }

        private ExpansionFilter _CurrentExpansionFilter { get; set; }
        public ExpansionFilter CurrentExpansionFilter
        {
            get
            {
                if (_CurrentExpansionFilter == null)
                    _CurrentExpansionFilter = ExpansionFilters.First();

                return _CurrentExpansionFilter;
            }
            set
            {
                if (_CurrentExpansionFilter != value)
                {
                    _CurrentExpansionFilter = value;
                    RaisePropertyChanged("CurrentExpansionFilter");
                    RepopulateFilteredCollection();
                }
            }
        }

        private BoardgameSorter _CurrentCollectionSorter { get; set; }
        public BoardgameSorter CurrentCollectionSorter
        {
            get
            {
                if (_CurrentCollectionSorter == null)
                    CurrentCollectionSorter = CollectionSorters.First();

                return _CurrentCollectionSorter;
            }
            set
            {
                if (_CurrentCollectionSorter != value)
                {
                    _CurrentCollectionSorter = value;
                    RaisePropertyChanged("CurrentCollectionSorter");
                    RepopulateFilteredCollection();
                }
            }
        }

        private TextFilter _CurrentTextFilter = new TextFilter();
        public TextFilter CurrentTextFilter
        {
            get
            {
                if (_CurrentTextFilter == null)
                    _CurrentTextFilter = new TextFilter();

                return _CurrentTextFilter;
            }
            set
            {
                if (_CurrentTextFilter != value)
                {
                    _CurrentTextFilter = value;
                    RaisePropertyChanged("CurrentTextFilter");
                    RepopulateFilteredCollection();
                }
            }
        }

        public int NumberItemsDisplayed
        {
            get { return FilteredCollection.Count; }
        }

        public List<PlayerFilter> PlayerFilters
        {
            get { return PlayerFilter.DefaultFilters; }
        }

        public List<PlayTimeFilter> PlayTimeFilters
        {
            get { return PlayTimeFilter.DefaultFilters; }
        }

        public List<StatusFilter> StatusFilters
        {
            get { return StatusFilter.DefaultFilters; }
        }

        public List<ExpansionFilter> ExpansionFilters
        {
            get { return ExpansionFilter.DefaultFilters; }
        }

        public List<BoardgameSorter> CollectionSorters
        {
            get { return BoardgameSorter.DefaultSorters; }
        }

        private void CurrentTextFilter_FilterTextChanged(object sender, EventArgs e)
        {
            RepopulateFilteredCollection();
        }

        private void RepopulateFilteredCollection()
        {
            var filtered = new List<CollectionDataItem>();
            foreach (var ci in Collection)
            {
                var ShowMe = true;
                if (CurrentPlayerFilter != null)
                    ShowMe = MatchesPlayers(ci);

                if (ShowMe && CurrentPlayTimeFilter != null)
                    ShowMe = MatchesPlaytime(ci);

                if (ShowMe && CurrentStatusFilter != null)
                    ShowMe = CurrentStatusFilter.Matches(ci);

                if (ShowMe && CurrentExpansionFilter != null)
                    ShowMe = CurrentExpansionFilter.Matches(ci);

                if (ShowMe && CurrentTextFilter != null)
                    ShowMe = CurrentTextFilter.Matches(ci);

                if (ShowMe)
                    filtered.Add(ci);
            }

            IOrderedEnumerable<CollectionDataItem> sortedList = CurrentCollectionSorter.Sort(filtered);

            FilteredCollection.Clear();
            foreach (var item in sortedList)
                FilteredCollection.Add(item);

            RaisePropertyChanged("NumberItemsDisplayed");
        }
        #endregion

        public bool MatchesPlayers(CollectionDataItem game)
        {
            var match = true;
            if (game == null)
                match = false;
            else if (CurrentPlayerFilter == 0)
            {
                match = true; //match all;
            }
            else if (game.MinPlayers > CurrentPlayerFilter)
                match = false;
            else if (game.MaxPlayers < CurrentPlayerFilter)
                match = false;

            return match;
        }

        public bool MatchesPlaytime(CollectionDataItem game)
        {
            var match = true;
            if (game == null)
                match = false;
            else if (CurrentPlayTimeFilter == 0)
            {
                match = true; //match all;
            }
            else if (game.PlayingTime == 0)
            {
                match = false;
            }
            else if (game.PlayingTime > CurrentPlayTimeFilter)
                match = false;

            return match;
        }
    }
}
