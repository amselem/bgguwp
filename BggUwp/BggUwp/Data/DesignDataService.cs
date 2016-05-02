using BggUwp.Data.Models;
using BggUwp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BggUwp.Data
{
    public static class DesignDataService
    {
        public static ObservableCollection<HotDataItem> LoadHotItemsList()
        {
            ObservableCollection<HotDataItem> data = new ObservableCollection<HotDataItem>();
            HotDataItem item1 = new HotDataItem()
            {
                BoardGameName = "Design time HotItems item",
                YearPublished = 1999
            };
            data.Add(item1);

            HotDataItem item2 = new HotDataItem()
            {
                BoardGameName = "Design time HotItems item - second item",
                YearPublished = 2993
            };
            data.Add(item2);

            HotDataItem item3 = new HotDataItem()
            {
                BoardGameName = "Design time HotItems item - third item",
                YearPublished = 996
            };
            data.Add(item3);

            return data;
        }

        public static ObservableCollection<CollectionDataItem> LoadCollection()
        {
            ObservableCollection<CollectionDataItem> data = new ObservableCollection<CollectionDataItem>();
            CollectionDataItem item1 = new CollectionDataItem()
            {
                BoardGameName = "Design time item",
                YearPublished = 1999,
                MinPlayers = 2,
                MaxPlayers = 7,
                PlayingTime = 30,
                Rank = 678,
                Owned = true
            };
            data.Add(item1);

            CollectionDataItem item2 = new CollectionDataItem()
            {
                BoardGameName = "Design time item - second item",
                YearPublished = 2999,
                MinPlayers = 2,
                MaxPlayers = 2,
                PlayingTime = 200,
                Rank = 1678,
                UserRating = (Double)8.9,
                Owned = true
            };
            data.Add(item2);
            
            CollectionDataItem item3 = new CollectionDataItem()
            {
                BoardGameName = "Design time item - third item",
                YearPublished = 3999,
                MinPlayers = 6,
                MaxPlayers = 15,
                PlayingTime = 20,
                Rank = 58,
                Owned = true
            };
            data.Add(item3);

            return data;
        }

        public static ObservableCollection<PlayDataItem> LoadPlays()
        {
            ObservableCollection<PlayDataItem> data = new ObservableCollection<PlayDataItem>();
            PlayDataItem play1 = new PlayDataItem()
            {
                BoardGameId = 253648,
                BoardGameName = "Imperial Design Data BoardGame Name",
                Length = 120,
                PlayDate = DateTime.Today,
                UserComment = "DULL!",
                NumberOfPlays = 1,
                Players = new ObservableCollection<PlayerStatsDataItem>()
            };
            data.Add(play1);

            PlayDataItem play2 = new PlayDataItem()
            {
                BoardGameId = 1255,
                BoardGameName = "Arkham Horror",
                Length = 15,
                PlayDate = DateTime.Now,
                UserComment = "Terrifying experience in dark environment.",
                NumberOfPlays = 12,
                Players = new ObservableCollection<PlayerStatsDataItem>()
            };
            data.Add(play2);

            return data;
        }

        public static BoardGameDataItem LoadBoardGame()
        {
            BoardGameDataItem bg = new BoardGameDataItem()
            {
                AverageRating = (Double)(7.9),
                BoardGameId = 7923,
                Description = "It is a detailed description of a game, but you can get some additional game info at website.",
                GeekRating = (Double)(6.7),
                MaxPlayers = 5,
                MinPlayers = 2,
                BoardGameName = "Eldritch Horror",
                PlayingTime = 240,
                Rank = 57,
                YearPublished = 1996,
                Artists = new ObservableCollection<string>()
                {
                    "Flenn Ferguson",
                    "Jacqueline Rodriquez",
                    "Nathan Holmes",
                    "Melinda Price",
                    "Judy Lawson"
                },
                Designers = new ObservableCollection<string>()
                {
                    "Myrtle Bailey",
                    "Janice Rodriguez"
                },
                Publishers = new ObservableCollection<string>()
                {
                    "Ethel Schmidt"
                },
                PlayerPollResults = new ObservableCollection<PlayerPollResultDataItem>()
                {
                    new PlayerPollResultDataItem()
                    {
                        Best = 78,
                        NotRecommended = 18,
                        NumberOfPlayers = 1,
                        NumberOfPlayersIsAndHigher = false,
                        Recommended = 152
                    },
                    new PlayerPollResultDataItem()
                    {
                        Best = 10,
                        NotRecommended = 70,
                        NumberOfPlayers = 2,
                        NumberOfPlayersIsAndHigher = true,
                        Recommended = 40
                    }
                }
            };

            return bg;
        }

        public static CollectionDataItem LoadCollectionItem()
        {
            CollectionDataItem ci = new CollectionDataItem()
            {
                BoardGameName = "Eldritch Horror",
                YearPublished = 1996,
                BoardGameId = 7923,
                ThumbnailPath = 7923 + "_th.jpg",
                MaxPlayers = 5,
                MinPlayers = 2,
                PlayingTime = 240,
                IsExpansion = false,
                UserRating = (double)5.5,
                GeekRating = (double)6.7,
                AverageRating = (double)7.9,
                Rank = 57,
                NumberOfPlays = 21,

                CollectionItemId = 17213,
                Owned = true,
                PreviousOwned = false,
                ForTrade = false,
                Want = false,
                WantToPlay = true,
                WantToBuy = false,
                Wishlist =true,
                WishlistPriority = 4,
                PreOrdered = true,
            };

            return ci;
        }

        public static ObservableCollection<SearchResultDataItem> LoadLocalSearchResults()
        {
            ObservableCollection<SearchResultDataItem> searchResults = new ObservableCollection<SearchResultDataItem>();
            searchResults.Add(new SearchResultDataItem()
            {
                Title = "Test collection item (2016)",
                Id = 15,
                IconString = "\uE8F1"
            });
            searchResults.Add(new SearchResultDataItem()
            {
                Title = "Imperial Settlers: The Mad Blue Huge Sheep Attack Scenario (1992)",
                Id = 1272,
                IconString = "\uE734"
            });

            return searchResults;
        }

        public static ObservableCollection<SearchResultDataItem> LoadGlobalSearchResults()
        {
            ObservableCollection<SearchResultDataItem> searchResults = new ObservableCollection<SearchResultDataItem>();
            searchResults.Add(new SearchResultDataItem()
            {
                Title = "Be or not to be - The Game (2016)",
                Id = 8712,
                IconString = "\uE774"
            });

            return searchResults;
        }

        public static ObservableCollection<PlayerDataItem> LoadPlayersList()
        {
            ObservableCollection<PlayerDataItem> players = new ObservableCollection<PlayerDataItem>();
            players.Add(new PlayerDataItem()
            {
                Name = "Mim Daesh",
            });
            players.Add(new PlayerDataItem()
            {
                Name = "John Doe",
                UserId = 19213,
                PlayerId = 1001801,
                Username = "UWPTester"
            });
            players.Add(new PlayerDataItem()
            {
                Name = "Dean Mormo",
                PlayerId = 192
            });

            return players;
        }
    }
}
