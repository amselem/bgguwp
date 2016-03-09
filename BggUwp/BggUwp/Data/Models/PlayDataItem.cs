using System;
using System.Collections.Generic;
using Template10.Mvvm;

namespace BggUwp.Data.Models
{
    public class PlayDataItem : ViewModelBase 
    {
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
