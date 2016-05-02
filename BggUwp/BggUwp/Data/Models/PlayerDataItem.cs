using BggApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;

namespace BggUwp.Data.Models
{
    public class PlayerDataItem : ViewModelBase
    {
        public PlayerDataItem() { }
        public PlayerDataItem(Player apiPlayer)
        {
            Username = apiPlayer.Username;
            UserId = apiPlayer.UserId;
            PlayerId = apiPlayer.PlayerId;
            Name = apiPlayer.Name;
            ProfileColor = "#27ae60";
        }

        public string Username { get; set; } // only for BGG user player

        public int UserId { get; set; }

        public int PlayerId { get; set; } // uplayerid field in JSON API

        public string Name { get; set; } // BGG detects non-BGG-user players by name field

        public string ProfileColor { get; set; } 
    }
}
