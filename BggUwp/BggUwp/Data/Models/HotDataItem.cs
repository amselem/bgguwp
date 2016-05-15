using BggUwp.Data.Models.Abstract;
using BggApi.Models;
using SQLite.Net.Attributes;

namespace BggUwp.Data.Models
{
    public class HotDataItem : BaseItem
    {
        public HotDataItem() {}

        public HotDataItem(HotItem apiItem)
        {
            BoardGameName = apiItem.Name;
            BoardGameId = apiItem.BoardGameId;
            Rank = apiItem.Rank;
            ThumbnailPath = apiItem.BoardGameId.ToString() + "_th.jpg";
            YearPublished = apiItem.YearPublished;
        }

        private int _BoardGameId = 0;
        [PrimaryKey]
        public new int BoardGameId
        {
            get
            {
                return _BoardGameId;
            }
            set
            {
                Set(ref _BoardGameId, value);
            }
        }

        private int _YearPublished = 0;
        public int YearPublished
        {
            get
            {
                return _YearPublished;
            }
            set
            {
                Set(ref _YearPublished, value);
            }
        }

        private int _Ranking = 0;
        public int Rank
        {
            get
            {
                return _Ranking;
            }
            set
            {
                Set(ref _Ranking, value);
            }
        }
    }
}
