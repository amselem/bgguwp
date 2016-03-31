using BggApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BggUwp.Data.Models
{
    public class SearchResultDataItem
    {
        public SearchResultDataItem() { }

        public SearchResultDataItem(SearchResult apiResult)
        {
            BoardGameName = apiResult.BoardGameName;
            BoardGameId = apiResult.BoardGameId;
            YearPublished = apiResult.YearPublished;
        }

        public string BoardGameName { get; set; }
        public int BoardGameId { get; set; }
        public int YearPublished { get; set; }
    }
}
