using System;
using System.Collections.Generic;
using BggApi.Models;
using Template10.Mvvm;
using BggUwp.Data.Models.Abstract;

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

            Players = new List<PlayerStatsDataItem>();
            foreach (var player in apiPlay.Players)
            {
                Players.Add(new PlayerStatsDataItem(player));
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
        public List<PlayerStatsDataItem> Players { get; set; }
    }
}
