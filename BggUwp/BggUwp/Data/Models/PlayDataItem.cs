using System;
using System.Collections.Generic;
using BggApi.Models;
using Template10.Mvvm;

namespace BggUwp.Data.Models
{
    public class PlayDataItem : ViewModelBase
    {
        PlayDataItem() { }

        public PlayDataItem(Play apiPlay)
        {
            PlayId = apiPlay.PlayId;
            BoardGameName = apiPlay.BoardGameName;
            BoardGameId = apiPlay.BoardGameId;
            NumberOfPlays = apiPlay.NumberOfPlays;
            Length = apiPlay.Length;
            UserComment = apiPlay.UserComment;
            PlayDate = apiPlay.PlayDate;

            Players = new List<PlayerDataItem>();
            foreach (var player in apiPlay.Players)
            {
                Players.Add(new PlayerDataItem(player));
            }
        }

        public int PlayId { get; set; }
        public string BoardGameName { get; set; }
        public int BoardGameId { get; set; }
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
        public List<PlayerDataItem> Players { get; set; }
    }
}
