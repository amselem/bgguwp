using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BggApi.Models.Abstract;

namespace BggApi.Models
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
