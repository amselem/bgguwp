using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BggApi.Models.Abstract;
using Newtonsoft.Json;

namespace BggApi.Models
{
    public class Player
    {
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; } // only for BGG user player

        [JsonProperty(PropertyName = "userid")]
        public int UserId { get; set; }

        [JsonIgnore]
        public int PlayerId { get; set; } // uplayerid field in JSON API

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; } // BGG detects non-BGG-user players by name field
    }
}
