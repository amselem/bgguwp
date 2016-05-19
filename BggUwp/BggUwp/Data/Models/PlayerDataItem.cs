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
        public PlayerDataItem()
        {
            ProfileColor = GetRandomHexColor();
        }

        public PlayerDataItem(Player apiPlayer)
        {
            Username = apiPlayer.Username;
            BggUserId = apiPlayer.BggUserId;
            BggPlayerId = apiPlayer.BggPlayerId;
            Name = apiPlayer.Name;
            ProfileColor = GetRandomHexColor();
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Username { get; set; } // only for BGG user player

        public int BggUserId { get; set; }

        public int BggPlayerId { get; set; } // uplayerid field in JSON API

        public string Name { get; set; } // BGG detects non-BGG-user players by name field

        public string ProfileColor { get; set; }

        private static readonly Random getrandom = new Random();
        private string GetRandomHexColor()
        {
            List<string> colorsList = new List<string>()
            {
                "#1abc9c",
                "#16a085",
                "#2ecc71",
                "#27ae60",
                "#3498db",
                "#2980b9",
                "#9b59b6",
                "#8e44ad",
                "#34495e", // black
                "#2c3e50", // black
                "#f39c12",
                "#e67e22",
                "#d35400",
                "#e74c3c",
                "#c0392b",
                "#95a5a6", // grey
                "#7f8c8d"  // grey
            };

            return colorsList[getrandom.Next(0, colorsList.Count)];
        }
    }
}
