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
            Title = apiResult.BoardGameName;
            if (apiResult.YearPublished > 0)
            {
                Title += " (" + apiResult.YearPublished + ")";
            }

            Id = apiResult.BoardGameId;
            ResultType = typeof(SearchResult);
            IconString = "\uE774";
        }

        public SearchResultDataItem(HotDataItem collectionItem)
        {
            Title = collectionItem.Name;
            if (collectionItem.YearPublished > 0)
            {
                Title += " (" + collectionItem.YearPublished + ")";
            }

            Id = collectionItem.BoardGameId;
            ResultType = typeof(HotDataItem);
            IconString = "\uE734";
        }

        public SearchResultDataItem(CollectionDataItem hotItem)
        {
            Title = hotItem.Name;
            if (hotItem.YearPublished > 0)
            {
                Title += " (" + hotItem.YearPublished + ")";
            }

            Id = hotItem.BoardGameId;
            ResultType = typeof(CollectionDataItem);
            IconString = "\uE8F1";
        }

        public string Title { get; set; }
        public int Id { get; set; }
        public Type ResultType { get; set; }
        public string IconString { get; set; }
    }
}
