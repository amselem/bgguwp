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
        public string Username { get; set; } // only for BGG user player
        public int UserId { get; set; }
        public int PlayerId { get; set; } // uplayerid field in JSON API
        public string Name { get; set; } // BGG detects non-BGG-user players by name field
    }
}
