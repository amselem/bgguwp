using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BggApi.Models
{
    public class PlayerStats : Player
    {
        public string StartPosition { get; set; }
        public string Color { get; set; }
        public int Score { get; set; }
        public int Rating { get; set; }
        public bool IsNewPlayer { get; set; } // Ideal toggle for creating new player profile
        public bool IsWinner { get; set; }
    }
}
