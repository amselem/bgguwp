using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BggApi.Models
{
    public class SearchResult
    {
        public string BoardGameName { get; set; }
        public int BoardGameId { get; set; }
        public int YearPublished { get; set; }
    }
}
