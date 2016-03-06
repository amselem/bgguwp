using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BGGAPI.Models.Abstract;

namespace BGGAPI.Models
{
    public class BoardGame : BasicBoardGame
    {
        public string Description { get; set; }
        public List<string> Designers { get; set; }
        public List<string> Publishers { get; set; }
        public List<string> Artists { get; set; }
        public List<PlayerPollResult> PlayerPollResults { get; set; }
    }
}
