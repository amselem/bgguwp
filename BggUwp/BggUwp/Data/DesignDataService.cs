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
                Name = "Design time HotItems item",
                YearPublished = 1999
            };
            data.Add(item1);

            HotDataItem item2 = new HotDataItem()
            {
                Name = "Design time HotItems item - second item",
                YearPublished = 2993
            };
            data.Add(item2);

            HotDataItem item3 = new HotDataItem()
            {
                Name = "Design time HotItems item - third item",
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
                Name = "Design time item",
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
                Name = "Design time item - second item",
                YearPublished = 2999,
                MinPlayers = 2,
                MaxPlayers = 2,
                PlayingTime = 200,
                Rank = 1678,
                UserRating = (Decimal)8.9,
                Owned = true
            };
            data.Add(item2);
            
            CollectionDataItem item3 = new CollectionDataItem()
            {
                Name = "Design time item - third item",
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
                Players = new List<PlayerDataItem>()
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
                Players = new List<PlayerDataItem>()
            };
            data.Add(play2);

            return data;
        }
    }
}
