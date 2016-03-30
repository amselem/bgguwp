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

        private string _errorMessage = "Type more than 2 characters to perform search";
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                Set(ref _errorMessage, value);
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
                Set(ref _SearchQuery, value);
                // repopulate results
                SearchResultsList.Clear();
                if (value.ToString().Length > 2)
                {
                    ErrorMessage = "Searching..."; // TODO Processing bar
                    ExecuteSearch();
                }
                else
                {
                    ErrorMessage = "Type more than 2 characters to perform search";
                }
            }
        }

        async Task ExecuteSearch()
        {
            string invokedSearchQuery = _SearchQuery;
            var results = await dataService.SearchBgg(invokedSearchQuery);
            if (_SearchQuery == invokedSearchQuery)
            {
                SearchResultsList = results;
                if (SearchResultsList.Count == 0)
                {
                    ErrorMessage = "No results found for " + "\"" + invokedSearchQuery + "\"";
                }
                else
                {
                    ErrorMessage = "Hide";
                }
            }

        }

        private ObservableCollection<SearchResultDataItem> _SearchResultsList = new ObservableCollection<SearchResultDataItem>();
        public ObservableCollection<SearchResultDataItem> SearchResultsList
        {
            get { return _SearchResultsList; }
            set
            {
                Set(ref _SearchResultsList, value);
            }
        }

        public void GoToBoardGamePage(object sender, ItemClickEventArgs e) =>
            NavigationService.Navigate(typeof(Views.BoardGamePage), ((SearchResultDataItem)e.ClickedItem).BoardGameId);
    }
}
