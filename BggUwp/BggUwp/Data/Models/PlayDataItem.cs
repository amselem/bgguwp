using System;
using System.Collections.Generic;
using BggApi.Models;
using Template10.Mvvm;
using BggUwp.Data.Models.Abstract;
using System.Collections.ObjectModel;

namespace BggUwp.Data.Models
{
    public class PlayDataItem : BaseItem
    {
        public PlayDataItem() { }

        public PlayDataItem(Play apiPlay)
        {
            PlayId = apiPlay.PlayId;
            BoardGameName = apiPlay.BoardGameName;
            BoardGameId = apiPlay.BoardGameId;
            ThumbnailPath = apiPlay.BoardGameId.ToString() + "_th.jpg";
            NumberOfPlays = apiPlay.NumberOfPlays;
            Length = apiPlay.Length;
            UserComment = apiPlay.UserComment;
            PlayDate = apiPlay.PlayDate;

            Players = new ObservableCollection<PlayerStatsDataItem>();
            PlayersDisplay = String.Empty;
            List<string> playersLabels = new List<string>();
            foreach (var player in apiPlay.Players)
            {
                Players.Add(new PlayerStatsDataItem(player));

                // TODO remove user label
                if (!String.IsNullOrEmpty(player.Username))
                {
                    playersLabels.Add(player.Username);
                }
                else
                {
                    playersLabels.Add(player.Name);
                }
            }

            if (Players.Count == 0)
            {
                PlayersDisplay = "no players";
            }
            else
            {
                PlayersDisplay = string.Join(", ", playersLabels);
            }
        }

        public int PlayId { get; set; }
        public int NumberOfPlays { get; set; }
        public int Length { get; set; }
        public string UserComment { get; set; }
        private DateTime _playDate = DateTime.Today;
        public DateTime PlayDate
        {
            get
            {
                return _playDate;
            }
            set
            {
                Set(ref _playDate, value);
            }
        }
        public ObservableCollection<PlayerStatsDataItem> Players { get; set; }
        public string PlayersDisplay { get; set; }
    }
}
