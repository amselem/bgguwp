using BggApi;
using BggApi.Models;
using BggUwp.Data.Models;
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
        public string BGGUsername { get; set; }
        public string BGGPassword { get; set; }

        public DataService()
        {
            BGGUsername = "UWPTester";
            BGGPassword = "";
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
            if (CheckInternetAccess()) // && has not download for X hours
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

                            await SaveImage(rootFolder, item.ThumbnailWeb, temp.Thumbnail);
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
            if (CheckInternetAccess()) // && has not download for X hours
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
                            await SaveImage(rootFolder, item.ThumbnailWeb, temp.Thumbnail);
                            // await SaveImage(item.ImageWeb, temp.Image);
                            tmpCollection.Add(temp);
                        }

                    StorageService.SaveAllCollectionItems(tmpCollection);
                    }
                });
            }

            return tmpCollection;
            ;
        }

        public Task<IEnumerable<Play>> LoadPlays()
        {
            return Client.LoadLastPlays(BGGUsername);
        }

        public async Task<ObservableCollection<SearchResultDataItem>> SearchBgg(string query)
        {
            IEnumerable<SearchResult> searchResults = await Client.Search(query);
            ObservableCollection<SearchResultDataItem> resultsCollection = new ObservableCollection<SearchResultDataItem>();
            foreach (var result in searchResults)
            {
                resultsCollection.Add(new SearchResultDataItem(result));
            }

            return resultsCollection;
        }

        private bool CheckInternetAccess()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return (connectionProfile != null &&
                    connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
        }
    }
}
