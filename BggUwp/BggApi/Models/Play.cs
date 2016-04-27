using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BggApi.Models.Abstract;

namespace BggApi.Models
{
    public class Play 
    {
        public int PlayId { get; set; }
        public string BoardGameName { get; set; }
        public int BoardGameId { get; set; }
        public int NumberOfPlays { get; set; }
        public int Length { get; set; }
        public string UserComment { get; set; }
        public DateTime PlayDate { get; set; }
        public List<PlayerStats> Players { get; set; }
    }
}
