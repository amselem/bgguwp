using BggUwp.Data;
using BggUwp.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Controls;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;

namespace BggUwp.ViewModels
{
    public class SearchPageViewModel : ViewModelBase
    {
        private DataService dataService;
        Windows.UI.Core.CoreDispatcher dispatcher;

        public SearchPageViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                dataService = new DataService();
            }
        }

        private bool _IsSearchStatusMessageVisible = true;
        public bool IsSearchStatusMessageVisible
        {
            get { return _IsSearchStatusMessageVisible; }
            set
            {
                Set(ref _IsSearchStatusMessageVisible, value);
            }
        }

        private static string defaultSearchStatusMessage = "Type more than 2 characters to perform search";
        private string _SearchStatusMessage = defaultSearchStatusMessage;
        public string SearchStatusMessage
        {
            get { return _SearchStatusMessage; }
            set
            {
                Set(ref _SearchStatusMessage, value);
            }
        }

        private string _SearchQuery = string.Empty;
        public string SearchQuery
        {
            get
            {
                if (_SearchQuery == null)
                    _SearchQuery = string.Empty;

                return _SearchQuery;
            }
            set
            {
                IsSearchStatusMessageVisible = true;
                Set(ref _SearchQuery, value);

                GlobalResultsList.Clear();
                LocalResultsList.Clear();
                if (value.ToString().Length > 2)
                {
                    SearchStatusMessage = "Searching...";
                    ExecuteSearch();
                }
                else
                {
                    SearchStatusMessage = defaultSearchStatusMessage;
                }
            }
        }

        System.Threading.CancellationTokenSource cts = new System.Threading.CancellationTokenSource();
        async Task ExecuteSearch()
        {
            string invokedSearchQuery = _SearchQuery;
            var localResults = await dataService.SearchLocal(invokedSearchQuery);
            LocalResultsList = localResults;
            CancelSearchRequest();
            var globalResults = await dataService.SearchBgg(invokedSearchQuery, cts);
            if (_SearchQuery == invokedSearchQuery)
            {
                GlobalResultsList = globalResults;
                if (GlobalResultsList.Count == 0 && LocalResultsList.Count == 0)
                {
                    SearchStatusMessage = "No results found for " + "\"" + invokedSearchQuery + "\"";
                }
                else
                {
                    IsSearchStatusMessageVisible = false;
                }
            }

        }

        private void CancelSearchRequest()
        {
            cts.Cancel();
            cts.Dispose();
            cts = new System.Threading.CancellationTokenSource();
        }

        private ObservableCollection<SearchResultDataItem> _GlobalResultsList = new ObservableCollection<SearchResultDataItem>();
        public ObservableCollection<SearchResultDataItem> GlobalResultsList
        {
            get
            {
                if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                    ObservableCollection<SearchResultDataItem> tmp = new ObservableCollection<SearchResultDataItem>();
                    tmp.Add(new SearchResultDataItem()
                    {
                        Title = "Test (2016)",
                        Id = 15,
                        IconString = "\uE774"
                    });
                    tmp.Add(new SearchResultDataItem()
                    {
                        Title = "Imperial Settlers: The Mad Blue Huge Sheep Attack Scenario (1992)",
                        Id = 15,
                        IconString = "\uE734"
                    });
                    return tmp;
                }
                return _GlobalResultsList;
            }
            set
            {
                Set(ref _GlobalResultsList, value);
            }
        }

        private ObservableCollection<SearchResultDataItem> _LocalResultsList = new ObservableCollection<SearchResultDataItem>();
        public ObservableCollection<SearchResultDataItem> LocalResultsList
        {
            get { return _LocalResultsList; }
            set
            {
                Set(ref _LocalResultsList, value);
            }
        }

        public void GoToBoardGamePage(object sender, ItemClickEventArgs e) =>
            NavigationService.Navigate(typeof(Views.BoardGamePage), ((SearchResultDataItem)e.ClickedItem).Id);
    }

}