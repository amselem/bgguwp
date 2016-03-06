using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGGAPI.Models.Abstract
{
    public abstract class BasicBoardGame : BasicItem 
    {
        public string ImageWeb { get; set; }
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public int PlayingTime { get; set; }
        public bool IsExpansion { get; set; } // Not explicitly given by API, but present in links
        public decimal AverageRating { get; set; } // Average rating from community
        public decimal GeekRating { get; set; } // BayesAverage field
        public int Rank { get; set; }
        public int NumberOfPlays { get; set; }
    }
}
