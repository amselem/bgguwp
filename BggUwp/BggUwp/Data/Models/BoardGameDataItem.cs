﻿using System.Collections.ObjectModel;
using System.Linq;
using BggUwp.Data.Models.Abstract;
using BggApi.Models;

namespace BggUwp.Data.Models
{
    public class BoardGameDataItem : BoardGameItem
    {
        public BoardGameDataItem() { }
        public BoardGameDataItem(BoardGame apiItem)
        {
            Name = apiItem.Name;
            YearPublished = apiItem.YearPublished;
            BoardGameId = apiItem.BoardGameId;
            Image = apiItem.BoardGameId.ToString() + ".jpg";
            Thumbnail = apiItem.BoardGameId.ToString() + "_th.jpg";
            MinPlayers = apiItem.MinPlayers;
            MaxPlayers = apiItem.MaxPlayers;
            PlayingTime = apiItem.PlayingTime;
            IsExpansion = apiItem.IsExpansion;
            GeekRating = apiItem.GeekRating;
            AverageRating = apiItem.AverageRating;
            Rank = apiItem.Rank;
            NumberOfPlays = apiItem.NumberOfPlays;

            Description = apiItem.Description;

            Designers = new ObservableCollection<string>();
            foreach (string designer in apiItem.Designers)
                Designers.Add(designer);

            Publishers = new ObservableCollection<string>();
            foreach (string publisher in apiItem.Publishers)
                Publishers.Add(publisher);

            Artists = new ObservableCollection<string>();
            foreach (string artist in apiItem.Artists)
                Artists.Add(artist);

            PlayerPollResults = new ObservableCollection<PlayerPollResultDataItem>();
            foreach (PlayerPollResult result in apiItem.PlayerPollResults.OrderBy(x => x.NumberOfPlayers + (x.NumberOfPlayersIsAndHigher ? 1 : 0))) // add one to 4+ , making it 5 and the highest
            {
                PlayerPollResults.Add(new PlayerPollResultDataItem()
                {
                    Best = result.Best,
                    NumberOfPlayers = result.NumberOfPlayers,
                    NumberOfPlayersIsAndHigher = result.NumberOfPlayersIsAndHigher,
                    NotRecommended = result.NotRecommended,
                    Recommended = result.Recommended
                });
            }
        }
        private string _Description = "";
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                Set(ref _Description, value);
            }
        }

        public ObservableCollection<string> Designers { get; set; }
        public ObservableCollection<string> Publishers { get; set; }
        public ObservableCollection<string> Artists { get; set; }

        public ObservableCollection<PlayerPollResultDataItem> PlayerPollResults { get; set; }
    }
}
