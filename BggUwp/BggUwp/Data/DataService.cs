using BggApi;
using BggApi.Models;
using BggUwp.Data.Models;
using BggUwp.Data.Models.Abstract;
using BggUwp.Messaging;
using GalaSoft.MvvmLight.Messaging;
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
        private UserDataItem BGGUser { get; set; }

        public static readonly DataService Instance;

        static DataService()
        {
            Instance = Instance ?? new DataService();
            Instance.RetrieveCredentials();
        }

        public async void RetrieveCredentials()
        {
            Windows.Security.Credentials.PasswordCredential credentials = StorageService.RetrieveUserCredentials();
            if (credentials != null)
            {
                BGGUsername = credentials.UserName;
                BGGPassword = credentials.Password;
                BGGUser = new UserDataItem(await Client.LoadUserDetails(BGGUsername));
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
                Messenger.Default.Send(new ProgressMessage()
                {
                    State = ProgressMessage.ProgressState.Started,
                    Message = "Downloading popular games..."
                });
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
                Messenger.Default.Send(new ProgressMessage()
                {
                    State = ProgressMessage.ProgressState.Finished
                });
            }

            return hotItems;
        }

        internal async Task<ObservableCollection<CollectionDataItem>> LoadCollection()
        {
            ObservableCollection<CollectionDataItem> tmpCollection = new ObservableCollection<CollectionDataItem>();
            if (ShouldUpdateData() && !String.IsNullOrEmpty(BGGUsername)) // && has not download for X hours
            {
                Messenger.Default.Send(new ProgressMessage()
                {
                    State = ProgressMessage.ProgressState.Started,
                    Message = "Downloading collection..."
                });
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
                Messenger.Default.Send(new ProgressMessage()
                {
                    State = ProgressMessage.ProgressState.Finished
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
            ObservableCollection<SearchResultDataItem> resultsCollection = new ObservableCollection<SearchResultDataItem>();
            if (IsThereInternetAccess() == true)
            {
                IEnumerable<SearchResult> searchResults = await Client.Search(query, cts);
                foreach (var result in searchResults)
                {
                    resultsCollection.Add(new SearchResultDataItem(result));
                }
            }
            else
            {
                Messenger.Default.Send(new StatusMessage()
                {
                    Status = StatusMessage.StatusType.Error,
                    Message = "There is no internet access"
                });
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
            Messenger.Default.Send(new ProgressMessage()
            {
                State = ProgressMessage.ProgressState.Started,
                Message = "Downloading board game info..."
            });
            var apiBoardGame = await Client.LoadBoardGame(gameId);
            Messenger.Default.Send(new ProgressMessage()
            {
                State = ProgressMessage.ProgressState.Finished,
            });
            return new BoardGameDataItem(apiBoardGame);
        }

        public CollectionDataItem LoadCollectionItemFromStorage(int gameId)
        {
            return StorageService.LoadCollectionItem(gameId);
        }

        public async Task<CollectionDataItem> LoadCollectionItemFromWeb(int gameId)
        {
            if (CanEdit())
            {
                return new CollectionDataItem(await Client.LoadCollectionItem(gameId, BGGUsername, BGGUser.BggUserId));
            }

            return new CollectionDataItem();
        }

        public async Task<string> GetRulesLink(int gameId)
        {
            return await Client.LoadRules(gameId);
        }

        public async Task DownloadPlayersListFromWeb()
        {
            if (CanEdit())
            {
                var apiPlayers = await Client.LoadPlayersList(BGGUsername, BGGPassword);
                foreach (var apiPlayer in apiPlayers)
                {
                    if (RoamingStorageService.Instance.LoadPlayer(apiPlayer.Name) == null)
                    {
                        RoamingStorageService.Instance.AddPlayer(new PlayerDataItem(apiPlayer));
                    }
                }
            }
        }

        public ObservableCollection<PlayerDataItem> LoadPlayers()
        {
            return new ObservableCollection<PlayerDataItem>(RoamingStorageService.Instance.LoadAllPlayers());
        }

        public bool AddPlayer(PlayerDataItem player)
        {
            if (RoamingStorageService.Instance.LoadPlayer(player.Username) != null)
            {
                return false;
            }

            RoamingStorageService.Instance.AddPlayer(player);
            return true;
        }

        public bool RemovePlayer(PlayerDataItem player)
        {
            if (RoamingStorageService.Instance.LoadPlayer(player.Name) == null || player.Id <= 0)
            {
                return false;
            }

            RoamingStorageService.Instance.DeletePlayer(player.Id);
            return true;
        }

        private bool CanEdit()
        {
            if (IsThereInternetAccess() == false)
            {
                Messenger.Default.Send(new StatusMessage()
                {
                    Status = StatusMessage.StatusType.Error,
                    Message = "There is no internet access"
                });
                return false;
            }

            if (String.IsNullOrEmpty(BGGPassword) || String.IsNullOrEmpty(BGGUsername))
            {
                Messenger.Default.Send(new StatusMessage()
                {
                    Status = StatusMessage.StatusType.Error,
                    Message = "You're not logged in"
                });
                return false;
            }

            return true;
        }

        private bool ShouldUpdateData()
        {
            if (settingsService.UpdateDataOnlyOnWiFi == true && IsOnWiFi() == false)
                return false;

            if (IsThereInternetAccess() == false)
                return false;

            return true;
        }

        private bool IsOnWiFi()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();

            if (connectionProfile.IsWlanConnectionProfile)
                return true;

            return false;
        }

        public bool IsThereInternetAccess()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return (connectionProfile != null &&
                    connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
        }

        internal async Task<bool> AddToCollection(int boardGameId)
        {
            if (CanEdit())
            {
                return await Client.AddToCollection(BGGUsername, BGGPassword, boardGameId);
            }

            return false;
        }

        internal async Task<bool> EditCollectionItem(CollectionDataItem collectionItem)
        {
            if (CanEdit())
            {
                // TODO Create Converter
                CollectionItem item = new CollectionItem()
                {
                    CollectionItemId = collectionItem.CollectionItemId,
                    ForTrade = collectionItem.ForTrade,
                    Owned = collectionItem.Owned,
                    WantToBuy = collectionItem.WantToBuy,
                    WantToPlay = collectionItem.WantToPlay,
                    Wishlist = collectionItem.Wishlist,
                    WishlistPriority = collectionItem.WishlistPriority,
                    UserRating = collectionItem.UserRating,
                    UserComment = collectionItem.UserComment
                };

                return await Client.EditCollectionItemStatus(BGGUsername, BGGPassword, item);
            }

            return false;
        }

        internal async Task<bool> RemoveCollectionItem(int collectionItemId)
        {
            if (CanEdit())
            {
                return await Client.RemoveFromCollection(BGGUsername, BGGPassword, collectionItemId);
            }

            return false;
        }

        internal async Task<bool> LogPlay(int gameId, DateTime date, int amount, string comments, int length, PlayDataItem tmp)
        {
            Play play = new Play()
            {   
                BoardGameId = tmp.BoardGameId,
                PlayDate = tmp.PlayDate,
                NumberOfPlays = tmp.NumberOfPlays,
                UserComment = tmp.UserComment,
                Length = tmp.Length,
                Players = new List<PlayerStats>()
            };

            foreach (var player in tmp.Players)
            {
                play.Players.Add((PlayerStats)player);
            }

            if (CanEdit())
            {
                 return await Client.LogPlay(BGGUsername, BGGPassword, play);
            }

            return false;
        }
    }
}
