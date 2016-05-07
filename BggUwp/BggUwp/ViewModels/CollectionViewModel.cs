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

namespace BggUwp.ViewModels
{
    public class CollectionViewModel : ViewModelBase
    {
        Windows.UI.Core.CoreDispatcher dispatcher;

        public CollectionViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
                Messenger.Default.Register<RefreshDataMessage>(this, RefreshData);
                LoadCollection();
                CurrentPlayerFilter = SettingsService.Instance.SavedPlayerFilter;
                CurrentPlayTimeFilter = SettingsService.Instance.SavedPlayTimeFilter;
            }

            Collection.CollectionChanged += Collection_CollectionChanged;

            CurrentPlayerFilter.PropertyChanged += AnyFilterChangedEvent;
            CurrentPlayTimeFilter.PropertyChanged += AnyFilterChangedEvent;
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
                if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                    return DesignDataService.LoadCollection();
                }
                return _FilteredCollection;
            }
            set
            {
                Set(ref _FilteredCollection, value);
            }
        }

        public void LoadCollection()
        {
            LoadFromStorage();
            LoadFromWeb();
        }

        private void LoadFromStorage()
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
        }

        private async void LoadFromWeb()
        {
            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                ObservableCollection<CollectionDataItem> tmp = await DataService.Instance.LoadCollection();
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

        private void RefreshData(RefreshDataMessage msg)
        {
            if (msg.RequestedRefreshScope == RefreshDataMessage.RefreshScope.All ||
                msg.RequestedRefreshScope == RefreshDataMessage.RefreshScope.Collection)
            {
                if (msg.RequestedRefreshType == RefreshDataMessage.RefreshType.Local)
                    LoadFromStorage();
                else
                    LoadFromWeb();
            }
        }

        public DelegateCommand ResetFiltersCommand => new DelegateCommand(() =>
        {
            CurrentStatusFilter = StatusFilters.First();
            CurrentExpansionFilter = ExpansionFilters.First();
            CurrentPlayerFilter.Amount = 0;
            CurrentPlayTimeFilter.Amount = 0;
        });

        #region Filtering methods

        private ExpansionFilter _CurrentExpansionFilter { get; set; }
        public ExpansionFilter CurrentExpansionFilter
        {
            get
            {
                if (_CurrentExpansionFilter == null)
                {
                    _CurrentExpansionFilter = ExpansionFilters[SettingsService.Instance.SavedExpansionFilter];
                }

                return _CurrentExpansionFilter;
            }
            set
            {
                if (_CurrentExpansionFilter != value)
                {
                    _CurrentExpansionFilter = value;
                    SettingsService.Instance.SavedExpansionFilter = ExpansionFilters.IndexOf(value);
                    RaisePropertyChanged("CurrentExpansionFilter");
                    RepopulateFilteredCollection();
                }
            }
        }

        private StatusFilter _CurrentStatusFilter { get; set; }
        public StatusFilter CurrentStatusFilter
        {
            get
            {
                if (_CurrentStatusFilter == null)
                {
                    _CurrentStatusFilter = StatusFilters[SettingsService.Instance.SavedStatusFilter];
                }

                return _CurrentStatusFilter;
            }
            set
            {
                if (_CurrentStatusFilter != value)
                {
                    _CurrentStatusFilter = value;
                    SettingsService.Instance.SavedStatusFilter = StatusFilters.IndexOf(value);
                    RaisePropertyChanged("CurrentStatusFilter");
                    RepopulateFilteredCollection();
                }
            }
        }

        private PlayerFilter _CurrentPlayerFilter = new PlayerFilter();
        public PlayerFilter CurrentPlayerFilter
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

        private PlayTimeFilter _CurrentPlayTimeFilter = new PlayTimeFilter();
        public PlayTimeFilter CurrentPlayTimeFilter
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

        public int NumberItemsDisplayed
        {
            get { return FilteredCollection.Count; }
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

        private void AnyFilterChangedEvent(object sender, EventArgs e)
        {
            RepopulateFilteredCollection();
            SettingsService.Instance.SavedPlayTimeFilter = CurrentPlayTimeFilter;
            SettingsService.Instance.SavedPlayerFilter = CurrentPlayerFilter;
        }

        private void RepopulateFilteredCollection()
        {
            var filtered = new List<CollectionDataItem>();
            foreach (var ci in Collection)
            {
                var ShowMe = true;
                if (CurrentPlayerFilter != null)
                    ShowMe = CurrentPlayerFilter.Matches(ci);

                if (ShowMe && CurrentPlayTimeFilter != null)
                    ShowMe = CurrentPlayTimeFilter.Matches(ci);

                if (ShowMe && CurrentStatusFilter != null)
                    ShowMe = CurrentStatusFilter.Matches(ci);

                if (ShowMe && CurrentExpansionFilter != null)
                    ShowMe = CurrentExpansionFilter.Matches(ci);

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
    }
}