using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BggApi.Models.Abstract;

namespace BggApi.Models
{
    public class Player
    {
        public string Username { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string StartPosition { get; set; }
        public string Color { get; set; }
        public int Score { get; set; }
        public bool IsNewPlayer { get; set; } // Ideal toggle for creating new player profile
        public bool IsWinner { get; set; }
    }
}
