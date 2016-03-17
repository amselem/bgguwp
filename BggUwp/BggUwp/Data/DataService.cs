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
            BGGUsername = "Belash";
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
                // update collection in background    
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

        private bool CheckInternetAccess()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return (connectionProfile != null &&
                    connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
        }
    }
}
