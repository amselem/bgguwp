using BggApi;
using BggApi.Models;
using BggUwp.Data.Models;
using BggUwp.Data.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.Web.Http;

namespace BggUwp.Data
{
    class DataService
    {
        private readonly BggApiClient Client = new BggApiClient();
        private SettingsService settingsService = SettingsService.Instance;
        private string BGGUsername { get; set; }
        private string BGGPassword { get; set; }

        public static readonly DataService Instance;

        static DataService()
        {
            Instance = Instance ?? new DataService();
            Instance.RetrieveCredentials();
        }

        public void RetrieveCredentials()
        {
            Windows.Security.Credentials.PasswordCredential credentials = StorageService.RetrieveUserCredentials();
            if (credentials != null)
            {
                BGGUsername = credentials.UserName;
                BGGPassword = credentials.Password;
            }
            else
            {
                // TODO Should display error message
            }
        }

        public async Task SaveImage(StorageFolder rootFolder, string url, string fileName)
        {
            StorageFile coverpicFile = await rootFolder.TryGetItemAsync(fileName) as StorageFile;
            if (coverpicFile == null) // TODO or is blank
            {
                try
                {
                    coverpicFile = await rootFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                    HttpClient client = new HttpClient();

                    var buffer = await client.GetBufferAsync(new Uri(url));
                    await FileIO.WriteBufferAsync(coverpicFile, buffer);
                }
                catch
                {

                }
            }
        }

        internal async Task<ObservableCollection<HotDataItem>> LoadHotItemsList()
        {
            ObservableCollection<HotDataItem> hotItems = new ObservableCollection<HotDataItem>();
            if (ShouldUpdateData()) // && has not download for X hours
            {
                // update hotItems in background    
                await Task.Run(async () =>
                {
                    var apihotItems = await Client.LoadHotItems();
                    if (apihotItems != null)
                    {
                        var rootFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("CoverPics", CreationCollisionOption.OpenIfExists);
                        foreach (var item in apihotItems)
                        {
                            var temp = new HotDataItem(item);

                            await SaveImage(rootFolder, item.ThumbnailWeb, temp.ThumbnailPath);
                            hotItems.Add(temp);
                        }

                        StorageService.SaveAllHotItems(hotItems);
                    }
                });
            }

            return hotItems;
        }

        internal async Task<ObservableCollection<CollectionDataItem>> LoadCollection()
        {
            ObservableCollection<CollectionDataItem> tmpCollection = new ObservableCollection<CollectionDataItem>();
            if (ShouldUpdateData()) // && has not download for X hours
            {
                // update collection in background    
                await Task.Run(async () =>
                {
                    var apiCollection = await Client.LoadCollection(BGGUsername);
                    if (apiCollection != null)
                    {
                        var rootFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("CoverPics", CreationCollisionOption.OpenIfExists);
                        foreach (var item in apiCollection)
                        {
                            var temp = new CollectionDataItem(item);
                            await SaveImage(rootFolder, item.ThumbnailWeb, temp.ThumbnailPath);
                            // await SaveImage(item.ImageWeb, temp.Image);
                            tmpCollection.Add(temp);
                        }

                        StorageService.SaveAllCollectionItems(tmpCollection);
                    }
                });
            }

            return tmpCollection;
        }

        public Task<IEnumerable<Play>> LoadPlays()
        {
            return Client.LoadLastPlays(BGGUsername);
        }

        public async Task<ObservableCollection<SearchResultDataItem>> SearchBgg(string query, System.Threading.CancellationTokenSource cts)
        {
            // TODO If no Internet access display error message

            IEnumerable<SearchResult> searchResults = await Client.Search(query, cts);
            ObservableCollection<SearchResultDataItem> resultsCollection = new ObservableCollection<SearchResultDataItem>();
            foreach (var result in searchResults)
            {
                resultsCollection.Add(new SearchResultDataItem(result));
            }

            return resultsCollection;
        }

        public async Task<ObservableCollection<SearchResultDataItem>> SearchLocal(string query)
        {
            List<HotDataItem> resultHotItems = StorageService.SearchInHotItems(query).ToList();
            List<CollectionDataItem> resultCollectionItems = StorageService.SearchInCollection(query).ToList();
            ObservableCollection<SearchResultDataItem> resultsCollection = new ObservableCollection<SearchResultDataItem>();
            foreach (var result in resultCollectionItems)
            {
                resultHotItems.Remove(resultHotItems.Find(item => item.BoardGameId == result.BoardGameId));
                resultsCollection.Add(new SearchResultDataItem(result));
            }
            foreach (var result in resultHotItems)
            {
                resultsCollection.Add(new SearchResultDataItem(result));
            }

            return resultsCollection;
        }

        public async Task<BoardGameDataItem> LoadBoardGame(int gameId)
        {
            var apiBoardGame = await Client.LoadBoardGame(gameId);
            return new BoardGameDataItem(apiBoardGame);
        }

        public CollectionDataItem LoadCollectionItem(int gameId)
        {
            return StorageService.LoadCollectionItem(gameId);
        }

        private bool ShouldUpdateData()
        {
            bool flag = true;

            if (settingsService.UpdateDataOnlyOnWiFi == true && IsOnWiFi() == false)
                flag = false;

            if (IsThereInternetAccess() == false)
                flag = false;

            return flag;
        }

        private bool IsOnWiFi()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();

            if (connectionProfile.IsWlanConnectionProfile)
                return true;

            return false;
        }

        private bool IsThereInternetAccess()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return (connectionProfile != null &&
                    connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
        }

        internal async Task AddToCollection(int boardGameId)
        {
            await Client.AddToCollection(BGGUsername, BGGPassword, boardGameId);
            // TODO Error handling
        }

        internal async Task EditCollectionItem(CollectionDataItem collectionItem)
        {
            // TODO Create Converter
            CollectionItem item = new CollectionItem();
            item.CollectionItemId = collectionItem.CollectionItemId;
            item.ForTrade = collectionItem.ForTrade;
            item.Owned = collectionItem.Owned;
            item.WantToBuy = collectionItem.WantToBuy;
            item.WantToPlay = collectionItem.WantToPlay;
            item.Wishlist = collectionItem.Wishlist;
            item.WishlistPriority = collectionItem.WishlistPriority;

            await Client.EditCollectionItemStatus(BGGUsername, BGGPassword, item);
        }

        internal async Task RemoveCollectionItem(int collectionItemId)
        {
            await Client.RemoveFromCollection(BGGUsername, BGGPassword, collectionItemId);
        }
    }
}
