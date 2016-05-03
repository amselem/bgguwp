using BggApi.Models;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;

namespace BggUwp.Data.Models
{
    public class PlayerDataItem : BindableBase
    {
        public PlayerDataItem() { }
        public PlayerDataItem(Player apiPlayer)
        {
            Username = apiPlayer.Username;
            BggUserId = apiPlayer.BggUserId;
            BggPlayerId = apiPlayer.BggPlayerId;
            Name = apiPlayer.Name;
            ProfileColor = "#27ae60";
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Username { get; set; } // only for BGG user player

        public int BggUserId { get; set; }

        public int BggPlayerId { get; set; } // uplayerid field in JSON API

        public string Name { get; set; } // BGG detects non-BGG-user players by name field

        public string ProfileColor { get; set; } 
    }
}
