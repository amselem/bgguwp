using BggUwp.Data.Models;
using BggUwp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BggUwp.Views
{
    public class DesignDataService
    {
        public DesignDataService()
        {
            LoadHotItemsList();
            LoadCollection();
            LoadPlays();
        }

        private HotItemsViewModel _HotItemsVM = new HotItemsViewModel();
        public HotItemsViewModel HotItemsVM
        {
            get
            {
                return _HotItemsVM;
            }
        }

        private CollectionViewModel _CollectionVM = new CollectionViewModel();
        public CollectionViewModel CollectionVM
        {
            get
            {
                return _CollectionVM;
            }
        }

        private PlaysViewModel _PlaysVM = new PlaysViewModel();
        public PlaysViewModel PlaysVM
        {
            get
            {
                return _PlaysVM;
            }
        }

        public void LoadHotItemsList()
        {
            HotDataItem item = new HotDataItem();
            item.Name = "Design time HotItems item";
            item.YearPublished = 1999;
            HotItemsVM.HotItemsList.Add(item);

            HotDataItem item2 = new HotDataItem();
            item2.Name = "Design time HotItems item - second item";
            item2.YearPublished = 2999;
            HotItemsVM.HotItemsList.Add(item2);

            HotDataItem item3 = new HotDataItem();
            item3.Name = "Design time HotItems item - third item";
            item3.YearPublished = 3999;
            HotItemsVM.HotItemsList.Add(item3);
        }

        public void LoadCollection()
        {
            CollectionDataItem item = new CollectionDataItem();
            item.Name = "Design time item";
            item.YearPublished = 1999;
            item.MaxPlayers = 7;
            item.PlayingTime = 30;
            item.Rank = 678;
            item.Owned = true;
            CollectionVM.FilteredCollection.Add(item);

            CollectionDataItem item2 = new CollectionDataItem();
            item2.Name = "Design time item - second item";
            item2.YearPublished = 2999;
            item2.MaxPlayers = 2;
            item2.PlayingTime = 200;
            item2.Rank = 1678;
            item2.Owned = true;
            CollectionVM.FilteredCollection.Add(item2);

            CollectionDataItem item3 = new CollectionDataItem();
            item3.Name = "Design time item - third item";
            item3.YearPublished = 3999;
            item3.MaxPlayers = 15;
            item3.PlayingTime = 20;
            item3.Rank = 58;
            item3.Owned = true;
            CollectionVM.FilteredCollection.Add(item3);
        }

        public void LoadPlays()
        {
            List<PlayDataItem> playsList = new List<PlayDataItem>();
            PlayDataItem play = new PlayDataItem();
            play.BoardGameId = 253648;
            play.BoardGameName = "Imperial Design Data BoardGame Name";
            play.Length = 120;
            play.PlayDate = DateTime.Today;
            play.UserComment = "DULL!";
            play.NumberOfPlays = 1;
            play.Players = new List<PlayerDataItem>();
            PlaysVM.PlaysList.Add(play);

            PlayDataItem play2 = new PlayDataItem();
            play2.BoardGameId = 1255;
            play2.BoardGameName = "Arkham Horror";
            play2.Length = 15;
            play2.PlayDate = DateTime.Today;
            play2.UserComment = "Terryfying experience in dark enivroment.";
            play2.NumberOfPlays = 1;
            play2.Players = new List<PlayerDataItem>();
            PlaysVM.PlaysList.Add(play);
        }
    }
}
