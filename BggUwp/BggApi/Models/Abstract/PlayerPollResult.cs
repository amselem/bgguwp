using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BGGAPI.Models.Abstract;

namespace BGGAPI.Models
{
    public class PlayerPollResult
    {
        public int NumberOfPlayers { get; set; }
        public int Best { get; set; }
        public int Recommended { get; set; }
        public int NotRecommended { get; set; }
        // If 4+ is recommended then NumberOfPlayers is 4 and this var is true
        public bool NumberOfPlayersIsAndHigher { get; set; }
    }
}
